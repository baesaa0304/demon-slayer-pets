using System;
using System.Drawing;

sealed partial class Pet
{
    static Skill[] InosukeSkills()
    {
        const string beast = "짐승의 호흡";
        return new[] {
            new Skill("제3엄니 · 물어뜯기 (喰い裂き)", DrawInosukeBite, 1.08, group: beast),
            new Skill("제7형 · 공간식각", DrawSpatialAwareness, 1.55, group: beast),
            new Skill("도약 쌍검", DrawInosukeSkill),
            new Skill("멧돼지 가면 벗기", DrawInosukeUnmask, 2.0),
            new Skill("유연성 · 몸 접기", DrawInosukeFlexibility, 1.35)
        };
    }

    static void DrawInosukeBite(Graphics g, double phase)
    {
        double charge = Pulse(phase, 0, 0.22, 0.36);
        double attack = Pulse(phase, 0.22, 0.53, 0.94);
        double slash = Progress(phase, 0.22, 0.82);
        var state = g.Save();
        g.TranslateTransform((float)(6 * attack - 5 * charge), (float)(-6 * attack));
        g.RotateTransform((float)(15 * attack - 7 * charge));
        g.ScaleTransform(0.88f, (float)(1 - 0.21 * charge));
        DrawCharacter(g, 3, (int)(5 * attack), false);
        double first = -18 - 134 * slash;
        double second = -162 + 134 * slash;
        DrawInosukeSword(g, -14, -28, first);
        DrawInosukeSword(g, 14, -28, second);
        if (phase > 0.31 && phase < 0.78)
        {
            using (var wind = new Pen(Color.LightSteelBlue, 3))
            {
                int tail = 134;
                g.DrawArc(wind, -62, -78, 91, 80, (int)first, tail);
                g.DrawArc(wind, -29, -78, 91, 80, (int)second - tail, tail);
            }
        }
        g.Restore(state);
    }

    static void DrawSpatialAwareness(Graphics g, double phase)
    {
        var state = g.Save();
        g.ScaleTransform(1, 0.76f);
        DrawCharacter(g, 3, 0, false);
        g.Restore(state);
        using (var echo = new Pen(Color.LightCyan, 2))
        {
            for (int i = 0; i < 3; i++)
            {
                int radius = 12 + (int)(51 * ((phase + i / 3.0) % 1));
                g.DrawEllipse(echo, -radius, -16, radius * 2, 13);
            }
        }
    }

    static void DrawInosukeFlexibility(Graphics g, double phase)
    {
        double fold = Pulse(phase, 0.06, 0.45, 0.92);
        g.ScaleTransform((float)(1 + 0.34 * fold), (float)(1 - 0.55 * fold));
        g.RotateTransform((float)(21 * fold));
        DrawCharacter(g, 3, (int)(7 * fold), false);
    }
}
