using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


sealed partial class Pet : Form
{
    static readonly string[] Names = { "Tanjiro", "Nezuko", "Zenitsu", "Inosuke", "Giyu" };
    static readonly Color Clear = Color.Magenta;
    readonly int kind;
    readonly Timer timer = new Timer { Interval = 33 };
    readonly Stopwatch clock = Stopwatch.StartNew();
    double x, last;
    int direction;
    Skill activeSkill;
    double actionUntil;
    static readonly Random SkillRandom = new Random();
    bool automaticSkills = true;
    double nextAutomaticSkill;

    Pet(int index, Rectangle area, bool alwaysOnTop)
    {
        kind = index;
        Text = Names[index];
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = alwaysOnTop;
        BackColor = TransparencyKey = Clear;
        DoubleBuffered = true;
        ClientSize = new Size(144, 136);
        x = area.Left + area.Width * (index + 1) / (Names.Length + 1.0) - Width / 2;
        direction = index % 2 == 0 ? 1 : -1;
        Location = new Point((int)x, area.Top - Height);
        ContextMenuStrip = new ContextMenuStrip();
        ContextMenuStrip.Items.Add("돌려보내기", null, (sender, args) => Close());
        ScheduleAutomaticSkill(0);
        timer.Tick += (sender, args) => Tick();
        timer.Start();
    }

    protected override bool ShowWithoutActivation { get { return true; } }

    protected override CreateParams CreateParams
    {
        get
        {
            const int toolWindow = 0x80;
            const int noActivate = 0x08000000;
            var parameters = base.CreateParams;
            parameters.ExStyle |= toolWindow | noActivate;
            return parameters;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            timer.Dispose();
            if (ContextMenuStrip != null) ContextMenuStrip.Dispose();
        }
        base.Dispose(disposing);
    }

    bool Walking(double time) { return (time + kind * 2) % 7 < 4.5; }

    static double Advance(double position, double distance, double min, double max, ref int facing)
    {
        position += distance * facing;
        if (position >= max) facing = -1;
        else if (position <= min) facing = 1;
        return Math.Max(min, Math.Min(max, position));
    }

    void Tick()
    {
        // shortcut: pets use the primary work area, add monitor selection if needed.
        Rectangle area = Screen.PrimaryScreen.WorkingArea;
        double now = clock.Elapsed.TotalSeconds;
        double elapsed = Math.Min(now - last, 0.1);
        last = now;
        if (dragging) return;

        if (UpdateFall(area.Bottom - Height, now, elapsed))
        {
            Invalidate();
            return;
        }

        if (hurtUntil > 0)
        {
            double hurtPhase = Progress(now, hurtUntil - HurtDuration, hurtUntil);
            x = Math.Max(area.Left - 44, Math.Min(area.Right - Width + 44, KnockbackX(knockbackStart, direction, hurtPhase)));
            Left = (int)Math.Round(x);
            if (hurtPhase >= 1) hurtUntil = 0;
            Invalidate();
            return;
        }

        if (attackTarget != null && UpdateEncounter(now, elapsed)) return;

        if (clickSkillQueued && now >= actionUntil)
        {
            PlayNextClickSkill();
            return;
        }

        if (UpdateEncounter(now, elapsed)) return;

        if (automaticSkills && !mouseHeld && now >= actionUntil && now >= nextAutomaticSkill)
        {
            var skills = Skills[kind];
            Play(skills[SkillRandom.Next(skills.Length)]);
            return;
        }

        double phase = now < actionUntil ? SkillPhase(now) : 0;
        double speed = now < actionUntil
            ? (activeSkill.Speed(phase))
            : (Walking(now) ? 42 + kind * 5 : 0);
        x = Advance(x, elapsed * speed, area.Left - 44, area.Right - Width + 44, ref direction);
        Location = new Point((int)x, area.Bottom - Height);
        Invalidate();
    }

    void Play(Skill skill)
    {
        clickSkillQueued = false;
        attackTarget = null;
        activeSkill = skill;
        actionUntil = clock.Elapsed.TotalSeconds + skill.Duration;
        ScheduleAutomaticSkill(actionUntil);
        Invalidate();
    }

    void ScheduleAutomaticSkill(double after)
    {
        nextAutomaticSkill = after + SkillRandom.Next(12, 25);
    }

    void SetAutomaticSkills(bool enabled)
    {
        automaticSkills = enabled;
        ScheduleAutomaticSkill(clock.Elapsed.TotalSeconds);
    }

    double SkillPhase(double now) { return 1 - (actionUntil - now) / activeSkill.Duration; }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        double now = clock.Elapsed.TotalSeconds;
        bool falling = !dragging && Top < Screen.PrimaryScreen.WorkingArea.Bottom - Height;
        bool landing = !dragging && now < landingUntil;
        bool hurt = !dragging && now < hurtUntil;
        bool acting = !hurt && !dragging && !falling && !landing && now < actionUntil;
        bool walking = !hurt && !dragging && !falling && !landing && !acting && Walking(now);
        int stride = walking ? (int)(Math.Sin(now * 14) * 3) : 0;
        g.TranslateTransform(ClientSize.Width / 2, ClientSize.Height - 2 - Math.Abs(stride));
        g.ScaleTransform(direction, 1);

        if (falling) DrawFalling(g, now);
        else if (landing) DrawLanding(g, 1 - (landingUntil - now) / LandingDuration);
        else if (hurt) DrawHurtPet(g, true, 0, false);
        else if (acting && attackTarget != null) DrawEncounterAttack(g, SkillPhase(now));
        else if (acting) activeSkill.Draw(g, SkillPhase(now));
        else if (now < scratchUntil) DrawHurtPet(g, false, stride, kind == 2 && !walking);
        else DrawCharacter(g, kind, stride, kind == 2 && !walking);
    }

    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--check")
        {
            if (!CanHitDemon(100, 150, 1, 0.5) || CanHitDemon(100, 158, 1, 0.5) ||
                CanHitDemon(100, 150, -1, 0.5) || CanHitDemon(100, 150, 1, 0.3)) return 1;
            if (KnockbackX(100, 1, 1) != 28 || KnockbackX(100, -1, 1) != 172 ||
                KnockbackX(100, 1, 0.5) >= 82) return 1;
            int facing = 1;
            if (Advance(9, 4, 0, 10, ref facing) != 10 || facing != -1) return 1;
            if (Advance(10, 12, 0, 10, ref facing) != 0 || facing != 1) return 1;
            if (ZenitsuDashing(0.2) || !ZenitsuDashing(0.5) || ZenitsuDashing(0.8)) return 1;
            if (Skills.Length != Names.Length) return 1;
            using (var bitmap = new Bitmap(144, 136))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                foreach (var character in Skills)
                foreach (var skill in character)
                {
                    graphics.Clear(Color.Transparent);
                    graphics.ResetTransform();
                    graphics.TranslateTransform(72, 134);
                    skill.Draw(graphics, 0.48);
                    bool drawn = false;
                    for (int y = 10; y < bitmap.Height && !drawn; y += 3)
                    for (int x = 0; x < bitmap.Width && !drawn; x += 3)
                        drawn = bitmap.GetPixel(x, y).A != 0;
                    if (!drawn || skill.Duration <= 0) return 1;
                }
            }
            return 0;
        }

        Application.EnableVisualStyles();
        RunTray();
        return 0;
    }
}