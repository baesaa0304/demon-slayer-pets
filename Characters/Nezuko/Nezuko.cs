using System.Drawing;

sealed partial class Pet
{
    static void DrawNezukoSkill(Graphics g, double phase)
    {
        double duck = Pulse(phase, 0, 0.28, 1);
        double retreat = Progress(phase, 0.16, 0.33) - Progress(phase, 0.69, 0.96);
        int boxX = 26 - (int)(26 * Progress(phase, 0.04, 0.32));
        var pose = g.Save();
        g.TranslateTransform((float)(boxX * retreat), 0);
        g.ScaleTransform((float)(1 - 0.38 * retreat), (float)(1 - 0.62 * duck - 0.28 * retreat));
        DrawCharacter(g, 1, (int)(3 * duck), false);
        g.Restore(pose);

        if (phase >= 0.14 && phase <= 0.85)
        {
            Box(g, Ink, boxX - 21, -37, 42, 37);
            Box(g, Color.SaddleBrown, boxX - 18, -34, 36, 31);
            Box(g, Color.Peru, boxX - 14, -32, 5, 28);
            Box(g, Color.Peru, boxX + 9, -32, 5, 28);
            Box(g, Ink, boxX - 18, -34, 36, 6);
            int lid = (int)(9 * (Pulse(phase, 0.14, 0.23, 0.36) + Pulse(phase, 0.57, 0.71, 0.85)));
            Box(g, Ink, boxX - 22, -38 - lid, 44, 7);
            Box(g, Color.Peru, boxX - 19, -36 - lid, 38, 3);
        }
    }

    static void DrawNezukoKick(Graphics g, double phase)
    {
        double crouch = Pulse(phase, 0, 0.21, 0.35);
        double leap = Pulse(phase, 0.21, 0.51, 0.87);
        double kick = Pulse(phase, 0.38, 0.54, 0.7);
        var pose = g.Save();
        g.TranslateTransform((float)(16 * leap), (float)(-34 * leap));
        g.RotateTransform((float)(-30 * kick));
        g.ScaleTransform(1, (float)(1 - 0.24 * crouch));
        DrawCharacter(g, 1, (int)(4 * leap), false);
        if (kick > 0)
        {
            int toe = 10 + (int)(27 * kick);
            using (var leg = new Pen(Ink, 10)) g.DrawLine(leg, 8, -13, toe, -12);
            using (var wrap = new Pen(Color.MistyRose, 5)) g.DrawLine(wrap, 10, -13, toe - 3, -12);
            if (kick > 0.45)
            {
                DrawBodyLightning(g, phase, Color.DeepPink, Color.MistyRose);
                DrawPinkSparks(g, toe, -12, kick);
            }
        }
        g.Restore(pose);
    }

    static void DrawNezukoBurst(Graphics g, double phase)
    {
        double gather = Pulse(phase, 0, 0.26, 0.47);
        double burst = Pulse(phase, 0.30, 0.51, 0.86);
        var pose = g.Save();
        g.TranslateTransform((float)(8 * burst - 6 * gather), 0);
        g.RotateTransform((float)(12 * burst - 10 * gather));
        DrawCharacter(g, 1, (int)(4 * burst), false);

        int hand = 13 + (int)(15 * burst);
        int y = -26 - (int)(18 * gather + 6 * burst);
        using (var sleeve = new Pen(Ink, 10)) g.DrawLine(sleeve, 13, -29, hand, y);
        using (var cloth = new Pen(Color.HotPink, 6)) g.DrawLine(cloth, 13, -29, hand, y);
        Oval(g, Skin, hand - 3, y - 3, 6, 6);
        if (burst > 0.15) DrawBodyLightning(g, phase, Color.DeepPink, Color.MistyRose);
        g.Restore(pose);

        if (burst > 0.15) DrawPinkSparks(g, 34, -40, burst);
    }

    static void DrawPinkSparks(Graphics g, int x, int y, double strength)
    {
        var state = g.Save();
        g.TranslateTransform(x, y);
        float scale = (float)(0.4 + 0.6 * strength);
        g.ScaleTransform(scale, scale);

        Point[] arc = { P(-13, 5), P(-3, 0), P(-8, -7), P(6, -12), P(2, -22), P(15, -31) };
        Point[] branch = { P(1, -10), P(13, -8), P(11, -16), P(24, -21) };
        using (var flame = new Pen(Color.DeepPink, 5))
        using (var flash = new Pen(Color.MistyRose, 2))
        {
            g.DrawLines(flame, arc);
            g.DrawLines(flame, branch);
            g.DrawLines(flash, arc);
            g.DrawLines(flash, branch);
        }
        Box(g, Color.HotPink, 19, -30, 3, 3);
        Box(g, Color.MistyRose, 26, -11, 3, 3);
        g.Restore(state);
    }
}