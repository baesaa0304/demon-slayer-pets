using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

sealed partial class Pet
{
    const int DemonReach = 57;
    static readonly System.Collections.Generic.List<Demon> Demons = new System.Collections.Generic.List<Demon>();
    static readonly Action<Graphics, double>[][] EncounterAttacks =
    {
        new Action<Graphics, double>[] { DrawNezukoBurst, DrawNezukoKick },
        new Action<Graphics, double>[] { DrawZenitsuSkill, DrawZenitsuSixfold },
        new Action<Graphics, double>[] { DrawInosukeBite, DrawInosukeSkill },
        new Action<Graphics, double>[] { DrawGiyuTide }
    };
    static bool IsEncounterSkill(int character, Skill skill)
    {
        if (character == 0) return skill.Group == WaterGroup || skill.Group == SunGroup;
        return Array.IndexOf(EncounterAttacks[character - 1], skill.Draw) >= 0;
    }

    static readonly Color[] EncounterColors = { Color.OrangeRed, Color.DeepPink, Color.Gold, Color.LightSkyBlue, Color.DeepSkyBlue };
    Skill encounterSkill;

    Color EncounterColor
    {
        get { return activeSkill.Group == WaterGroup ? Color.DeepSkyBlue : EncounterColors[kind]; }
    }

    Demon attackTarget;
    bool attackLanded;
    const double HurtDuration = 0.52;
    double hurtUntil, scratchUntil, knockbackStart;

    enum DemonKind { Man, Woman, Child, Hand, Swamp, Drum, Spider, Bald }

    bool UpdateEncounter(double now, double elapsed)
    {
        if (attackTarget != null)
        {
            var target = attackTarget;
            if (now >= actionUntil)
            {
                attackTarget = null;
                if (attackLanded && !target.IsDisposed && target.CanCounter && target.Fighter == this)
                {
                    var defense = Array.Find(Skills[kind], skill => skill.Draw == DrawGiyuCalm);
                    if (defense != null)
                    {
                        Play(defense);
                        return true;
                    }

                    hurtUntil = now + HurtDuration;
                    scratchUntil = now + 4;
                    knockbackStart = x;
                    Invalidate();
                    return true;
                }
                return false;
            }

            double phase = SkillPhase(now);
            if (!target.IsDisposed && !target.Defeated && target.Fighter == this)
            {
                int strikeGap = (target.CenterX - CenterX) * direction;
                double advance = Math.Min(Math.Max(0, strikeGap - 16), activeSkill.Speed(phase) * elapsed);
                if (advance > 0)
                {
                    x += direction * advance;
                    Left = (int)Math.Round(x);
                }

                double impact = activeSkill.Draw == DrawZenitsuSixfold ? 0.90 : 0.48;
                if (!attackLanded && CanHitDemon(CenterX, target.CenterX, direction, phase, impact))
                {
                    attackLanded = true;
                    target.TakeHit(direction, EncounterColor);
                }
            }
            Invalidate();
            return true;
        }

        Demon encounter = null;
        int nearest = int.MaxValue;
        foreach (var demon in Demons)
        {
            if (demon.IsDisposed || demon.Defeated || demon.Fighter != this) continue;
            int distance = Math.Abs(demon.CenterX - CenterX);
            if (distance >= nearest) continue;
            nearest = distance;
            encounter = demon;
        }

        if (encounter == null ||
            mouseHeld || now < actionUntil)
            return false;

        int gap = encounter.CenterX - CenterX;
        direction = gap < 0 ? -1 : 1;
        if (Math.Abs(gap) <= DemonReach)
        {
            Play(encounterSkill);
            attackTarget = encounter;
            attackLanded = false;
        }
        else
        {
            x += direction * Math.Min(Math.Abs(gap) - DemonReach, 155 * elapsed);
            Left = (int)Math.Round(x);
            Invalidate();
        }
        return true;
    }

    void DrawEncounterAttack(Graphics g, double phase)
    {
        double step = activeSkill.Speed(phase) > 0 ? 0 : Pulse(phase, 0.23, 0.48, 0.79);
        var pose = g.Save();
        g.TranslateTransform((float)(12 * step), 0);
        activeSkill.Draw(g, phase);
        g.Restore(pose);

        // Reset the mirrored drawing transform so Korean text reads normally in either direction.
        var label = g.Save();
        g.ResetTransform();
        string title = activeSkill.Name.Split('(')[0].Trim();
        TextRenderer.DrawText(g, title, SystemFonts.DefaultFont,
            new Rectangle(0, 0, ClientSize.Width, 20), Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        g.Restore(label);
    }

    void DrawHurtPet(Graphics g, bool flinching, int stride, bool sleeping)
    {
        double phase = 1 - (hurtUntil - clock.Elapsed.TotalSeconds) / HurtDuration;
        if (flinching)
        {
            g.TranslateTransform(0, (float)(-20 * Math.Sin(Math.PI * Math.Max(0, phase))));
            g.RotateTransform((float)(-25 * (1 - phase)));
        }
        DrawCharacter(g, kind, stride, flinching || sleeping);

        int cheekY = kind == 3 ? -43 : -41;
        using (var scratch = new Pen(Color.IndianRed, 1))
        {
            g.DrawLine(scratch, -13, cheekY, -9, cheekY + 3);
            g.DrawLine(scratch, -14, cheekY + 3, -11, cheekY + 5);
        }
        if (flinching && phase < 0.65)
        {
            int travel = (int)(20 * Math.Max(0, phase));
            Box(g, Color.Firebrick, 17 + travel, -48 - travel, 3, 2);
            Box(g, Color.Crimson, 23 + travel, -43 - travel / 2, 2, 2);
            Box(g, Color.IndianRed, 15 + travel, -39 + travel / 2, 2, 2);
        }
    }

    static double KnockbackX(double start, int facing, double phase)
    {
        return start - facing * 72 * (1 - Math.Pow(1 - phase, 3));
    }

    int CenterX { get { return Left + Width / 2; } }

    static bool CanHitDemon(int attacker, int demon, int facing, double phase, double impact = 0.48)
    {
        int distance = (demon - attacker) * facing;
        return distance >= 0 && distance <= DemonReach && phase >= impact;
    }

    sealed class Demon : Form
    {
        readonly Stopwatch life = Stopwatch.StartNew();
        double defeatedAt = -1, hitAt = -1;
        readonly DemonKind variant;
        public bool IsWave { get; private set; }
        int health, hitDirection;
        Color hitColor;

        public Demon(Rectangle area, int center, bool alwaysOnTop, DemonKind variant, Pet fighter, bool wave)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = alwaysOnTop;
            BackColor = TransparencyKey = Clear;
            DoubleBuffered = true;
            this.variant = variant;
            IsWave = wave;
            Fighter = fighter;
            health = variant == DemonKind.Hand ? HandDemonHealth : variant == DemonKind.Drum ? 3 : variant == DemonKind.Spider ? 2 : 1;
            ClientSize = variant == DemonKind.Hand ? new Size(176, 148) : new Size(84, 92);
            Location = new Point(Math.Max(area.Left, Math.Min(area.Right - Width, center - Width / 2)), area.Bottom - Height);
        }

        public Pet Fighter { get; set; }
        public int CenterX { get { return Left + Width / 2; } }
        public bool CanCounter { get { return (variant == DemonKind.Hand || variant == DemonKind.Drum || variant == DemonKind.Spider) && !Defeated; } }
        public bool Defeated { get { return defeatedAt >= 0; } }
        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                var parameters = base.CreateParams;
                parameters.ExStyle |= 0x08000080;
                return parameters;
            }
        }

        public void TakeHit(int facing, Color color)
        {
            if (Defeated) return;
            hitDirection = facing;
            hitColor = color;
            hitAt = life.Elapsed.TotalSeconds;
            if (--health == 0) defeatedAt = hitAt;
        }

        public void UpdateScene()
        {
            double now = life.Elapsed.TotalSeconds;
            if (Defeated ? now - defeatedAt >= 0.62 : now >= (IsWave ? 90 : variant == DemonKind.Hand ? 30 : 18))
                Close();
            else
                Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            double now = life.Elapsed.TotalSeconds;
            g.TranslateTransform(ClientSize.Width / 2, ClientSize.Height - 3);
            var pose = g.Save();
            DrawBody(g, now);
            g.Restore(pose);

            double age = now - hitAt;
            if (hitAt >= 0 && age < 0.22)
            {
                int x = -hitDirection * (variant == DemonKind.Hand ? 25 : 13);
                int y = variant == DemonKind.Hand ? -52 : variant == DemonKind.Child ? -32 : -43;
                int spread = 8 + (int)(15 * Progress(age, 0, 0.22));
                using (var glow = new Pen(hitColor, 5))
                using (var core = new Pen(Color.Ivory, 2))
                {
                    g.DrawLine(glow, x - hitDirection * spread, y + 11, x + hitDirection * spread, y - 11);
                    g.DrawLine(core, x - hitDirection * spread, y + 11, x + hitDirection * spread, y - 11);
                    g.DrawLine(core, x - 6, y - 7, x + 6, y + 7);
                }
            }
        }

        void DrawBody(Graphics g, double now)
        {
            if (Defeated)
            {
                double phase = Progress(now, defeatedAt, defeatedAt + 0.62);
                for (int i = 0; i < 19; i++)
                {
                    int x = (i % 7 - 3) * (4 + (int)(6 * phase));
                    int y = -7 - i * 3 - (int)(phase * (12 + i % 5 * 4));
                    int size = Math.Max(1, (int)((1 - phase) * (3 + i % 4)));
                    Box(g, phase < 0.22 ? Color.OrangeRed : Color.SlateGray, x, y, size, size);
                }
                return;
            }

            g.TranslateTransform(0, (float)(-2 - 2 * Math.Sin(now * 6)));
            if (variant == DemonKind.Hand)
            {
                DrawHandDemon(g, now, now - hitAt < 0.16);
                return;
            }

            if (variant == DemonKind.Spider)
            {
                DrawSpiderDemon(g, now);
                return;
            }

            if (variant == DemonKind.Child) g.ScaleTransform(0.74f, 0.72f);
            Color robe = variant == DemonKind.Woman ? Color.DarkMagenta :
                variant == DemonKind.Child ? Color.DarkSlateBlue : Color.DarkCyan;
            Color pale = Color.FromArgb(236, 216, 211);
            if (variant == DemonKind.Bald) robe = Color.SaddleBrown;
            switch (variant)
            {
                case DemonKind.Swamp:
                    robe = Color.MidnightBlue;
                    pale = Color.LightSteelBlue;
                    Oval(g, Ink, -38, -13, 76, 14);
                    Oval(g, Color.DarkSlateBlue, -31, -11, 62, 9);
                    break;
                case DemonKind.Drum:
                    robe = Color.DarkRed;
                    break;
            }
            Color hair = Color.FromArgb(35, 29, 43);
            Color marks = Color.FromArgb(147, 46, 71);

            Oval(g, Ink, -17, -10, 14, 10);
            Oval(g, Ink, 4, -10, 14, 10);
            Shape(g, Ink, P(-14,-34), P(14,-34), P(25,-20), P(19,-11), P(14,-14),
                P(13,-4), P(4,-7), P(-2,-4), P(-8,-8), P(-16,-5), P(-17,-13), P(-24,-16));
            Shape(g, robe, P(-13,-32), P(14,-31), P(22,-20), P(18,-15),
                P(13,-17), P(12,-8), P(4,-11), P(-2,-7), P(-8,-12), P(-14,-9), P(-15,-16), P(-21,-17));
            Shape(g, Color.DarkSlateGray, P(-9,-31), P(4,-23), P(12,-31), P(6,-14), P(-3,-14));
            Box(g, Color.Maroon, -12, -17, 24, 4);
            Shape(g, pale, P(-18,-24), P(-29,-20), P(-31,-15), P(-26,-16), P(-23,-13), P(-16,-19));
            Shape(g, pale, P(18,-25), P(28,-21), P(32,-16), P(27,-17), P(24,-14), P(16,-19));

            if (variant == DemonKind.Bald || variant == DemonKind.Child)
            {
                DrawShavedDemonHead(g, pale, variant == DemonKind.Child);
            }
            else
            {
                Shape(g, Ink, P(-25,-39), P(-28,-58), P(-23,-70), P(-14,-75), P(-9,-79),
                    P(-1,-76), P(7,-81), P(11,-76), P(21,-77), P(21,-71), P(28,-63),
                    P(25,-49), P(27,-38), P(15,-31), P(-16,-32));
                if (variant == DemonKind.Woman)
                    Shape(g, hair, P(-24,-62), P(22,-66), P(28,-46), P(27,-19), P(17,-24),
                        P(13,-38), P(-12,-39), P(-16,-21), P(-28,-17), P(-23,-35));

                Oval(g, Color.FromArgb(173, 140, 149), -21, -68, 43, 39);
                Oval(g, pale, -19, -67, 39, 35);
                Shape(g, hair, P(-25,-53), P(-24,-67), P(-14,-74), P(-8,-77), P(-1,-73),
                    P(8,-79), P(13,-73), P(20,-75), P(21,-69), P(26,-62), P(21,-46),
                    P(17,-56), P(14,-62), P(8,-56), P(7,-65), P(-3,-58), P(-4,-65),
                    P(-13,-55), P(-14,-62));
                Shape(g, Color.FromArgb(78, 49, 65), P(-21,-67), P(-11,-74), P(-4,-72),
                    P(-12,-69), P(-17,-62));
                Shape(g, Color.FromArgb(78, 49, 65), P(3,-72), P(8,-76), P(12,-71), P(19,-70), P(14,-67));
                if (variant == DemonKind.Woman)
                {
                    Oval(g, Color.Plum, 14, -68, 9, 8);
                    Box(g, Color.Gold, 17, -65, 3, 3);
                }
            }

            using (var lines = new Pen(marks, 2))
            {
                g.DrawLines(lines, new[] { P(-16,-55), P(-19,-50), P(-16,-44), P(-18,-39) });
                g.DrawLines(lines, new[] { P(14,-56), P(17,-52), P(15,-46), P(19,-40) });
            }
            DrawDemonFace(g, variant == DemonKind.Child, variant == DemonKind.Woman);
            if (variant == DemonKind.Drum)
            {
                for (int x = -18; x <= 6; x += 12)
                {
                    Oval(g, Ink, x, -29, 13, 13);
                    Oval(g, Color.BurlyWood, x + 2, -27, 9, 9);
                    Box(g, Color.Maroon, x + 5, -24, 3, 3);
                }
            }
        }
    }
}
