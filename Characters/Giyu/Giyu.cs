using System.Drawing;

sealed partial class Pet
{
    static Skill[] GiyuSkills()
    {
        return new[] {
            new Skill("제4형 · 들이친 파도", DrawGiyuTide, 1.1, group: WaterGroup),
            new Skill("제11형 · 잔잔한 물결 (凪)", DrawGiyuCalm, 1.8, group: WaterGroup)
        };
    }

    static void DrawGiyu(Graphics g)
    {
        Shape(g, Color.DarkOliveGreen, P(0,-35), P(15,-35), P(21,-18), P(15,-12), P(0,-12));
        Box(g, Color.DarkGoldenrod, 5, -29, 6, 6);
        Box(g, Color.DarkGoldenrod, 12, -22, 5, 5);
        Box(g, Ink, -4, -34, 9, 21);
        Box(g, Color.Ivory, -4, -20, 9, 2);
        Shape(g, Ink, P(-20,-62), P(-23,-36), P(-31,-32), P(-21,-47), P(-25,-64), P(-14,-74),
            P(2,-79), P(15,-74), P(24,-65), P(23,-47), P(18,-39), P(-18,-40));
        Oval(g, Skin, -19, -67, 38, 35);
        Shape(g, Color.FromArgb(35,42,60), P(-24,-65), P(-15,-73), P(1,-77), P(16,-72), P(23,-65),
            P(19,-48), P(13,-59), P(7,-56), P(4,-65), P(-4,-55), P(-7,-64), P(-19,-51));
        DrawEye(g, -14, Color.SteelBlue, false);
        DrawEye(g, 5, Color.SteelBlue, false);
        Box(g, Color.Sienna, -2, -39, 6, 1);
    }

    static void DrawGiyuTide(Graphics g, double phase)
    {
        double cut = Pulse(phase, 0.2, 0.51, 0.92);
        g.RotateTransform((float)(17 * cut));
        DrawCharacter(g, 4, (int)(5 * cut), false);
        Weapon(g, 11, -27, -120 + 155 * cut, Color.Maroon, Color.LightCyan);
        if (cut > 0.15)
        {
            using (var water = new Pen(Color.DeepSkyBlue, 6))
                g.DrawArc(water, -35, -77, 88, 73, -138 + (int)(120 * cut), 135);
            using (var foam = new Pen(Color.LightCyan, 2))
                g.DrawArc(foam, -33, -75, 84, 69, -128 + (int)(120 * cut), 115);
        }
    }

    static void DrawGiyuCalm(Graphics g, double phase)
    {
        DrawCharacter(g, 4, 0, false);
        Weapon(g, 10, -27, -10, Color.Maroon, Color.LightCyan);
        for (int i = 0; i < 3; i++)
        {
            double ripple = Progress(phase, 0.16 + i * 0.15, 0.84 + i * 0.05);
            if (ripple <= 0 || ripple >= 1) continue;
            int radius = 12 + (int)(51 * ripple);
            using (var water = new Pen(i == 0 ? Color.DeepSkyBlue : Color.LightCyan, 2))
                g.DrawEllipse(water, -radius, -16 - i * 6, radius * 2, 11);
        }
        if (phase >= 0.25 && phase <= 0.55)
        {
            int radius = 34 + (int)(19 * Progress(phase, 0.25, 0.55));
            using (var guard = new Pen(Color.LightCyan, 2))
                g.DrawArc(guard, -radius, -76, radius * 2, 67, -68, 138);
        }
    }
}
