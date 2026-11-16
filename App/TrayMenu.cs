using System;
using System.Drawing;
using System.Windows.Forms;

sealed partial class Pet
{
    sealed class Skill
    {
        public readonly string Name;
        public readonly Action<Graphics, double> Draw;
        public readonly double Duration;
        readonly Func<double, bool> dashing;
        readonly double dashSpeed;
        public readonly string Group;

        public Skill(string name, Action<Graphics, double> draw, double duration = 0.95,
            Func<double, bool> dashing = null, double dashSpeed = 540, string group = null)
        {
            Name = name;
            Draw = draw;
            Duration = duration;
            this.dashing = dashing;
            this.dashSpeed = dashSpeed;
            Group = group;
        }

        public double Speed(double phase) { return dashing != null && dashing(phase) ? dashSpeed : 0; }
    }

    static readonly Skill[][] Skills =
    {
        TanjiroSkills(), NezukoSkills(), ZenitsuSkills(), InosukeSkills(), GiyuSkills()
    };

    static void AssignWaveFighters(Pet[] pets)
    {
        var fighters = Array.FindAll(pets, pet => pet != null && !pet.IsDisposed);
        if (fighters.Length == 0) return;

        int next = 0;
        foreach (var demon in Demons)
        {
            if (!demon.IsWave || demon.Defeated) continue;
            demon.Fighter = fighters[next];
            next = (next + 1) % fighters.Length;
        }
    }

    static ContextMenuStrip CreateTrayMenu(Pet[] pets, Timer encounters)
    {
        var labels = new[] { "탄지로", "네즈코", "젠이츠", "이노스케", "기유" };
        var menu = new ContextMenuStrip();
        var spawn = new ToolStripMenuItem("캐릭터 불러오기") { Name = "spawn" };
        var play = new ToolStripMenuItem("기술 재생") { Enabled = false };
        var dismiss = new ToolStripMenuItem("캐릭터 돌려보내기") { Enabled = false };
        var autoSkills = new ToolStripMenuItem("자동 기술 재생") { Name = "autoSkills", Checked = true, CheckOnClick = true };
        autoSkills.CheckedChanged += (sender, args) =>
        {
            foreach (var pet in pets)
                if (pet != null) pet.SetAutomaticSkills(autoSkills.Checked);
        };
        var alwaysOnTop = new ToolStripMenuItem("항상 위에 표시") { Checked = true, CheckOnClick = true };
        alwaysOnTop.CheckedChanged += (sender, args) =>
        {
            foreach (var pet in pets)
            {
                if (pet == null) continue;
                pet.TopMost = alwaysOnTop.Checked;
                if (!pet.TopMost) pet.SendToBack();
            }
        };

        var battle = new ToolStripMenuItem("혈귀 전투 기술 선택") { Name = "battle" };
        var spawnDemon = new ToolStripMenuItem("혈귀 소환") { Enabled = false };
        var autoDemons = new ToolStripMenuItem("혈귀 자동 등장") { Checked = true, CheckOnClick = true };
        int preferredFighter = -1;
        var encounterClock = System.Diagnostics.Stopwatch.StartNew();
        var demonNames = new[] { "남자 혈귀", "여자 혈귀", "어린 혈귀", "손 혈귀 · 강적", "늪 혈귀", "북 혈귀 · 강적", "거미 혈귀", "빡빡이 혈귀" };
        Action<DemonKind?> spawnEncounter = variant =>
        {
            if (Demons.Count != 0) return;
            var pet = preferredFighter < 0 ? null : pets[preferredFighter];
            if (pet == null) pet = Array.Find(pets, current => current != null);
            if (pet == null) return;
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            int offset = pet.CenterX < area.Left + area.Width / 2 ? 210 : -210;
            int count = variant.HasValue ? 1 : demonNames.Length;
            int spacing = Math.Min(160, area.Width / count);
            int left = area.Left + (area.Width - count * spacing) / 2;
            spawnDemon.Enabled = false;
            encounterClock.Restart();
            for (int i = 0; i < count; i++)
            {
                int center = variant.HasValue ? pet.CenterX + offset : left + i * spacing + spacing / 2;
                var demon = new Demon(area, center, alwaysOnTop.Checked, variant ?? (DemonKind)i, pet, !variant.HasValue);
                Demons.Add(demon);
                demon.FormClosed += (sender, args) =>
                {
                    Demons.Remove(demon);
                    spawnDemon.Enabled = play.Enabled && Demons.Count == 0;
                };
                demon.Show();
            }
            if (!variant.HasValue) AssignWaveFighters(pets);
        };
        spawnDemon.DropDownItems.Add("혈귀 전체 소환", null, (sender, args) => spawnEncounter(null));
        for (int i = 0; i < demonNames.Length; i++)
        {
            var variant = (DemonKind)i;
            spawnDemon.DropDownItems.Add(demonNames[i], null, (sender, args) => spawnEncounter(variant));
        }
        encounters.Tick += (sender, args) =>
        {
            for (int i = Demons.Count - 1; i >= 0; i--) Demons[i].UpdateScene();
            if (autoDemons.Checked && spawnDemon.Enabled && encounterClock.Elapsed.TotalSeconds >= 45)
                spawnEncounter((DemonKind)SkillRandom.Next(demonNames.Length));
        };
        autoDemons.CheckedChanged += (sender, args) => encounterClock.Restart();
        alwaysOnTop.CheckedChanged += (sender, args) =>
        {
            foreach (var demon in Demons) demon.TopMost = alwaysOnTop.Checked;
        };

        for (int i = 0; i < pets.Length; i++)
        {
            int index = i;
            var spawnItem = new ToolStripMenuItem(labels[index]);
            var playItem = new ToolStripMenuItem(labels[index]) { Enabled = false };
            var dismissItem = new ToolStripMenuItem(labels[index]) { Enabled = false };
            foreach (var skill in Skills[index])
            {
                var target = playItem;
                if (skill.Group != null)
                {
                    target = playItem.DropDownItems[skill.Group] as ToolStripMenuItem;
                    if (target == null)
                    {
                        target = new ToolStripMenuItem(skill.Group) { Name = skill.Group };
                        playItem.DropDownItems.Add(target);
                    }
                }
                target.DropDownItems.Add(skill.Name, null, (sender, args) =>
                {
                    if (pets[index] != null) pets[index].Play(skill);
                });
            }

            Skill selectedAttack = null;
            var battleItem = new ToolStripMenuItem(labels[index]);
            foreach (var skill in Skills[index])
            {
                if (!IsEncounterSkill(index, skill)) continue;
                if (selectedAttack == null) selectedAttack = skill;
                string title = skill.Group == null ? skill.Name : skill.Group + " · " + skill.Name;
                var option = new ToolStripMenuItem(title) { Checked = skill == selectedAttack };
                option.Click += (sender, args) =>
                {
                    preferredFighter = index;
                    selectedAttack = skill;
                    if (pets[index] != null)
                        foreach (var demon in Demons)
                            if (!demon.IsWave) demon.Fighter = pets[index];
                    foreach (ToolStripMenuItem choice in battleItem.DropDownItems)
                        choice.Checked = choice == option;
                    if (pets[index] != null) pets[index].encounterSkill = skill;
                };
                battleItem.DropDownItems.Add(option);
            }
            battle.DropDownItems.Add(battleItem);

            spawnItem.Click += (sender, args) =>
            {
                if (pets[index] != null) return;
                var pet = new Pet(index, Screen.PrimaryScreen.WorkingArea, alwaysOnTop.Checked);
                pet.encounterSkill = selectedAttack;
                pets[index] = pet;
                pet.SetAutomaticSkills(autoSkills.Checked);
                pet.FormClosed += (closedSender, closedArgs) =>
                {
                    pets[index] = null;
                    spawnItem.Enabled = true;
                    playItem.Enabled = dismissItem.Enabled = false;
                    play.Enabled = dismiss.Enabled = Array.Exists(pets, current => current != null);
                    foreach (var demon in Demons.ToArray())
                    {
                        var replacement = Array.Find(pets, current => current != null);
                        if (replacement == null) demon.Close();
                        else if (demon.Fighter == pet) demon.Fighter = replacement;
                    }
                    AssignWaveFighters(pets);
                    spawnDemon.Enabled = play.Enabled && Demons.Count == 0;
                };
                pet.Show();
                AssignWaveFighters(pets);
                if (!pet.TopMost) pet.SendToBack();
                spawnItem.Enabled = false;
                playItem.Enabled = dismissItem.Enabled = play.Enabled = dismiss.Enabled = true;
                spawnDemon.Enabled = Demons.Count == 0;
            };
            dismissItem.Click += (sender, args) => { if (pets[index] != null) pets[index].Close(); };
            spawn.DropDownItems.Add(spawnItem);
            play.DropDownItems.Add(playItem);
            dismiss.DropDownItems.Add(dismissItem);
        }

        menu.Items.AddRange(new ToolStripItem[] { spawn, play, dismiss, spawnDemon, autoDemons, battle, autoSkills, alwaysOnTop });
        menu.Items.Add("종료", null, (sender, args) => Application.Exit());
        return menu;
    }

    static void RunTray()
    {
        var pets = new Pet[Names.Length];
        using (var encounters = new Timer { Interval = 33 })
        using (var menu = CreateTrayMenu(pets, encounters))
        using (var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath))
        using (var tray = new NotifyIcon())
        {
            tray.Text = "Demon Slayer Pets";
            tray.Icon = icon;
            tray.ContextMenuStrip = menu;
            tray.Visible = true;
            ((ToolStripMenuItem)menu.Items["spawn"]).DropDownItems[0].PerformClick();
            encounters.Start();
            Application.Run();
        }
    }
}
