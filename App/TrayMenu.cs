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

        public Skill(string name, Action<Graphics, double> draw, double duration = 0.95,
            Func<double, bool> dashing = null)
        {
            Name = name;
            Draw = draw;
            Duration = duration;
            this.dashing = dashing;
        }

        public double Speed(double phase) { return dashing != null && dashing(phase) ? 540 : 0; }
    }

    static readonly Skill[][] Skills =
    {
        new[] {
            new Skill("물의 호흡 제1형 · 수면 베기", DrawTanjiroSkill),
            new Skill("물의 호흡 제2형 · 물방아", DrawWaterWheel, 1.15),
            new Skill("히노카미 카구라 · 원무", DrawFireDance, 1.30),
            new Skill("제12형 · 불꽃 연격 (창작)", DrawFireCombo, 2.15)
        },
        new[] {
            new Skill("상자에 숨기", DrawNezukoSkill),
            new Skill("도약 발차기", DrawNezukoKick, 1.05),
            new Skill("분홍 불꽃", DrawNezukoBurst, 1.1)
        },
        new[] {
            new Skill("번개의 호흡 제1형 · 벽력일섬", DrawZenitsuSkill, 0.95, ZenitsuDashing),
            new Skill("벽력일섬 · 연속 돌진", DrawZenitsuRush, 1.55, ZenitsuRushDashing)
        },
        new[] {
            new Skill("짐승의 호흡 · 도약 쌍검", DrawInosukeSkill),
            new Skill("짐승의 호흡 · 교차 베기", DrawInosukeCross, 1.1),
            new Skill("멧돼지 가면 벗기", DrawInosukeUnmask, 2.0)
        }
    };

    static ContextMenuStrip CreateTrayMenu(Pet[] pets)
    {
        var labels = new[] { "탄지로", "네즈코", "젠이츠", "이노스케" };
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

        for (int i = 0; i < pets.Length; i++)
        {
            int index = i;
            var spawnItem = new ToolStripMenuItem(labels[index]);
            var playItem = new ToolStripMenuItem(labels[index]) { Enabled = false };
            var dismissItem = new ToolStripMenuItem(labels[index]) { Enabled = false };
            foreach (var skill in Skills[index])
                playItem.DropDownItems.Add(skill.Name, null, (sender, args) => { if (pets[index] != null) pets[index].Play(skill); });

            spawnItem.Click += (sender, args) =>
            {
                if (pets[index] != null) return;
                var pet = new Pet(index, Screen.PrimaryScreen.WorkingArea, alwaysOnTop.Checked);
                pets[index] = pet;
                pet.SetAutomaticSkills(autoSkills.Checked);
                pet.FormClosed += (closedSender, closedArgs) =>
                {
                    pets[index] = null;
                    spawnItem.Enabled = true;
                    playItem.Enabled = dismissItem.Enabled = false;
                    play.Enabled = dismiss.Enabled = Array.Exists(pets, current => current != null);
                };
                pet.Show();
                if (!pet.TopMost) pet.SendToBack();
                spawnItem.Enabled = false;
                playItem.Enabled = dismissItem.Enabled = play.Enabled = dismiss.Enabled = true;
            };
            dismissItem.Click += (sender, args) => { if (pets[index] != null) pets[index].Close(); };
            spawn.DropDownItems.Add(spawnItem);
            play.DropDownItems.Add(playItem);
            dismiss.DropDownItems.Add(dismissItem);
        }

        menu.Items.AddRange(new ToolStripItem[] { spawn, play, dismiss, autoSkills, alwaysOnTop });
        menu.Items.Add("종료", null, (sender, args) => Application.Exit());
        return menu;
    }

    static void RunTray()
    {
        var pets = new Pet[Names.Length];
        using (var menu = CreateTrayMenu(pets))
        using (var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath))
        using (var tray = new NotifyIcon())
        {
            tray.Text = "Demon Slayer Pets";
            tray.Icon = icon;
            tray.ContextMenuStrip = menu;
            tray.Visible = true;
            ((ToolStripMenuItem)menu.Items["spawn"]).DropDownItems[0].PerformClick();
            Application.Run();
        }
    }
}
