using System;
using System.Drawing;

sealed partial class Pet
{
    const string WaterGroup = "물의 호흡";
    const string SunGroup = "히노카미 카구라";
    const string TanjiroOtherGroup = "감각·수련";

    static Skill[] TanjiroSkills()
    {
        return new[] {
            new Skill("제1형 · 수면 베기", DrawTanjiroSkill, group: WaterGroup),
            new Skill("제2형 · 물방아", DrawWaterWheel, 1.15, group: WaterGroup),
            Water("제3형 · 굽이춤 (流流舞い)", -145, 170, 12, 2),
            Water("제4형 · 들이친 파도 (打ち潮)", -110, 156, 5, 4),
            Water("제5형 · 가뭄의 단비 (干天の慈雨)", -92, 102, 0, 1),
            Water("제6형 · 비틀린 소용돌이 (ねじれ渦)", -180, 312, 8, 1, spin: 1),
            Water("제7형 · 물방울 파문 찌르기", -12, 0, 0, 1),
            Water("제8형 · 폭포수 (滝壺)", -108, 172, 27, 1),
            Water("제9형 · 수류 물보라 (水流飛沫・乱)", -29, 34, 18, 3, dash: true),
            Water("제10형 · 생생유전", -165, 262, 18, 3, spin: 1),
            Water("응용 · 가로 물방아 (横水車)", -172, 306, 5, 1, spin: 1),
            Water("응용 · 물방울 파문 찌르기·곡", -41, 33, 0, 1),
            new Skill("원무 (円舞)", DrawFireDance, 1.30, group: SunGroup),
            new Skill("후각 · 빈틈의 실", DrawOpeningThread, 1.25, group: TanjiroOtherGroup),
            new Skill("전집중 호흡 수련", DrawTotalConcentration, 1.55, group: TanjiroOtherGroup)
        };
    }

    static Skill Water(string name, int angle, int sweep, int lift, int hits, int spin = 0, bool dash = false)
    {
        return new Skill(name, (g, phase) => DrawBreathingForm(g, phase, false, angle, sweep, lift, hits, spin),
            0.95 + hits * 0.18, dash ? (Func<double, bool>)BreathingDash : null, 410, WaterGroup);
    }

    static bool BreathingDash(double phase) { return phase >= 0.34 && phase < 0.58; }

    static void DrawBreathingForm(Graphics g, double phase, bool fire, int start, int sweep, int lift, int hits, int spin)
    {
        double burst = Pulse(phase, 0.16, 0.55, 0.92);
        double charge = Pulse(phase, 0, 0.20, 0.35);
        double combo = Progress(phase, 0.23, 0.8) * hits;
        double swing = Math.Min(1, combo - Math.Min(hits - 1, (int)combo));
        double angle = start + sweep * swing;
        var state = g.Save();
        g.TranslateTransform((float)(10 * burst - 6 * charge), (float)(-lift * burst));
        g.ScaleTransform(0.87f, (float)(1 - 0.16 * charge));
        if (spin != 0)
        {
            g.TranslateTransform(0, -35);
            g.RotateTransform((float)(spin * 360 * Progress(phase, 0.22, 0.81)));
            g.TranslateTransform(0, 35);
        }
        else
        {
            g.RotateTransform((float)(13 * burst - 9 * charge));
        }

        Color glow = fire ? Color.OrangeRed : Color.DeepSkyBlue;
        Color core = fire ? Color.Gold : Color.LightCyan;
        DrawCharacter(g, 0, (int)(6 * burst), false);
        DrawTanjiroSword(g, angle, core);
        if (phase > 0.25 && phase < 0.86)
        {
            if (Math.Abs(sweep) < 45)
            {
                Point tip = SwordPoint(angle, 62);
                using (var edge = new Pen(glow, 6)) g.DrawLine(edge, SwordPoint(angle, 36), tip);
                using (var light = new Pen(core, 2)) g.DrawLine(light, SwordPoint(angle, 39), tip);
                using (var ring = new Pen(core, 2)) g.DrawEllipse(ring, tip.X - 9, tip.Y - 13, 13, 25);
            }
            else
            {
                int trail = Math.Min(Math.Abs(sweep), 50 + (int)(120 * burst));
                int end = (int)angle;
                using (var rim = new Pen(glow, fire ? 9 : 6)) g.DrawArc(rim, -42, -77, 90, 90, end - trail, trail);
                using (var light = new Pen(core, 2)) g.DrawArc(light, -40, -75, 86, 86, end - trail + 8, trail - 8);
                for (int i = 0; i < 4; i++)
                {
                    Point spark = SwordPoint(end - 20 * i - 12, 48 + i % 2 * 6);
                    Oval(g, i % 2 == 0 ? core : glow, spark.X, spark.Y, fire ? 4 : 3, 3);
                }
            }
        }
        g.Restore(state);
    }

    static void DrawOpeningThread(Graphics g, double phase)
    {
        DrawTanjiroSkill(g, phase);
        if (phase >= 0.10 && phase < 0.44)
        {
            using (var thread = new Pen(Color.White, 1)) g.DrawLine(thread, 6, -45, 65, -31);
            Oval(g, Color.LightCyan, 60, -35, 7, 7);
        }
    }

    static void DrawTotalConcentration(Graphics g, double phase)
    {
        double breath = Pulse(phase, 0.12, 0.48, 0.95);
        g.ScaleTransform(1, (float)(0.91 + 0.09 * breath));
        DrawCharacter(g, 0, 0, phase < 0.6);
        int radius = 16 + (int)(44 * Progress(phase, 0.28, 0.92));
        using (var air = new Pen(Color.LightCyan, 2))
            g.DrawArc(air, -radius, -radius - 34, 2 * radius, 2 * radius, 202, 125);
    }

}
