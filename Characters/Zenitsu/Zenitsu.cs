using System.Drawing;

sealed partial class Pet
{
    static bool ZenitsuDashing(double phase) { return phase >= 0.36 && phase < 0.62; }

    static void DrawZenitsuSkill(Graphics g, double phase)
    {
        double crouch = Pulse(phase, 0, 0.32, 0.46);
        double lunge = Pulse(phase, 0.32, 0.44, 0.75);
        if (ZenitsuDashing(phase))
        {
            Point[] trail = { P(-67, -18), P(-46, -29), P(-48, -19), P(-25, -31), P(-13, -29), P(6, -36) };
            using (var bolt = new Pen(Color.Gold, 5)) g.DrawLines(bolt, trail);
            using (var core = new Pen(Color.LemonChiffon, 2))
            {
                g.DrawLines(core, trail);
                g.DrawLine(core, -59, -38, -35, -39);
            }
        }

        var pose = g.Save();
        g.TranslateTransform((float)(-5 * crouch + 7 * lunge), (float)(-29 - 5 * lunge));
        g.RotateTransform((float)(-5 * crouch + 45 * lunge));
        g.ScaleTransform(1, (float)(1 - 0.32 * crouch - 0.14 * lunge));
        g.TranslateTransform(0, 29);
        DrawCharacter(g, 2, (int)(6 * lunge), true);
        Weapon(g, 13, -26, -195 + 145 * lunge, Color.DarkOrange, Color.LemonChiffon);
        if (ZenitsuDashing(phase)) DrawBodyLightning(g, phase, Color.Gold, Color.LemonChiffon);
        g.Restore(pose);
    }

    static double RushPhase(double phase) { return (phase * 3) % 1; }

    static bool ZenitsuRushDashing(double phase) { return ZenitsuDashing(RushPhase(phase)); }

    static void DrawZenitsuRush(Graphics g, double phase)
    {
        DrawZenitsuSkill(g, RushPhase(phase));
        if (ZenitsuRushDashing(phase))
            using (var flash = new Pen(Color.LightCyan, 2))
                g.DrawLines(flash, new[] { P(-65, -15), P(-46, -30), P(-49, -17), P(-31, -28) });
    }
}
