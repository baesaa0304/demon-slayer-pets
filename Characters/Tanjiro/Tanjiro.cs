using System;
using System.Drawing;

sealed partial class Pet
{
    static void DrawTanjiroSkill(Graphics g, double phase)
    {
        double windup = Pulse(phase, 0, 0.25, 0.44);
        double cut = Progress(phase, 0.25, 0.56);
        double recover = Progress(phase, 0.65, 1);
        var pose = g.Save();
        g.TranslateTransform((float)(-6 * windup + 9 * (cut - recover)), 0);
        g.RotateTransform((float)(-12 * windup + 13 * (cut - recover)));
        DrawCharacter(g, 0, (int)(4 * (cut - recover)), false);
        DrawTanjiroSword(g, -65 - 63 * windup + 105 * (cut - recover), Color.LightCyan);
        g.Restore(pose);

        if (phase >= 0.31 && phase <= 0.65)
        {
            int sweep = (int)(140 * Progress(phase, 0.31, 0.65));
            using (var water = new Pen(Color.DeepSkyBlue, 5)) g.DrawArc(water, -43, -75, 86, 72, -145 + sweep, 110);
            using (var foam = new Pen(Color.LightCyan, 2)) g.DrawArc(foam, -41, -72, 82, 66, -145 + sweep, 110);
        }
    }

    static void DrawWaterWheel(Graphics g, double phase)
    {
        double leap = Pulse(phase, 0.07, 0.48, 0.91);
        double spin = Progress(phase, 0.18, 0.79);
        var pose = g.Save();
        g.TranslateTransform((float)(30 * (spin - phase)), (float)(-34 * leap - 36));
        g.RotateTransform((float)(360 * spin));
        g.TranslateTransform(0, 36);
        DrawCharacter(g, 0, 4, false);
        DrawTanjiroSword(g, -35, Color.LightCyan);
        g.Restore(pose);

        if (spin > 0 && spin < 1)
        {
            int y = -68 - (int)(26 * leap);
            using (var water = new Pen(Color.DeepSkyBlue, 5)) g.DrawArc(water, -34, y, 68, 64, (int)(360 * spin), 245);
            using (var foam = new Pen(Color.LightCyan, 2)) g.DrawArc(foam, -31, y + 3, 62, 58, (int)(360 * spin), 215);
        }
    }

    static void DrawFireDance(Graphics g, double phase) { DrawFireCut(g, phase, 1); }

    static void DrawFireCombo(Graphics g, double phase)
    {
        double progress = phase * 3;
        int cut = (int)progress;
        DrawFireCut(g, progress - cut, cut == 1 ? -1 : 1);
    }

    static Point SwordPoint(double angle, double distance)
    {
        double radians = angle * Math.PI / 180;
        return P(3 + (int)Math.Round(distance * Math.Cos(radians)),
            -32 + (int)Math.Round(distance * Math.Sin(radians)));
    }

    static void DrawTanjiroSword(Graphics g, double angle, Color blade)
    {
        Point backHand = SwordPoint(angle, 7);
        Point frontHand = SwordPoint(angle, 15);
        Point guard = SwordPoint(angle, 20);
        Point tip = SwordPoint(angle, 45);
        using (var outline = new Pen(Ink, 8))
        using (var sleeve = new Pen(Color.SeaGreen, 5))
        {
            g.DrawLine(outline, P(-13, -30), backHand);
            g.DrawLine(outline, P(13, -30), frontHand);
            g.DrawLine(sleeve, P(-13, -30), backHand);
            g.DrawLine(sleeve, P(13, -30), frontHand);
        }
        using (var edge = new Pen(Ink, 5)) g.DrawLine(edge, backHand, tip);
        using (var steel = new Pen(blade, 3)) g.DrawLine(steel, guard, tip);
        double perpendicular = (angle + 90) * Math.PI / 180;
        int x = (int)Math.Round(4 * Math.Cos(perpendicular));
        int y = (int)Math.Round(4 * Math.Sin(perpendicular));
        using (var tsuba = new Pen(Color.Goldenrod, 2))
            g.DrawLine(tsuba, guard.X - x, guard.Y - y, guard.X + x, guard.Y + y);
        Oval(g, Skin, backHand.X - 3, backHand.Y - 3, 6, 6);
        Oval(g, Skin, frontHand.X - 3, frontHand.Y - 3, 6, 6);
    }

    static void DrawFireCut(Graphics g, double phase, int side)
    {
        double coil = Pulse(phase, 0, 0.24, 0.41);
        double cut = Progress(phase, 0.24, 0.64);
        double follow = Pulse(phase, 0.24, 0.56, 0.98);
        double angle = -120 - 62 * coil + 168 * cut - 110 * Progress(phase, 0.72, 1);
        var pose = g.Save();
        g.ScaleTransform(side, 1);
        g.TranslateTransform((float)(12 * follow - 7 * coil), (float)(-16 * follow));
        g.RotateTransform((float)(25 * follow - 20 * coil));

        double flame = Pulse(phase, 0.30, 0.58, 0.9);
        if (flame > 0)
        {
            int tail = (int)(70 + 142 * flame);
            int head = (int)angle;
            using (var rim = new Pen(Color.Firebrick, 12)) g.DrawArc(rim, -42, -77, 90, 90, head - tail, tail);
            using (var fire = new Pen(Color.OrangeRed, 8)) g.DrawArc(fire, -42, -77, 90, 90, head - tail + 10, tail - 10);
            using (var light = new Pen(Color.Gold, 4)) g.DrawArc(light, -42, -77, 90, 90, head - tail + 22, tail - 22);
            for (int i = 0; i < 5; i++)
            {
                double emberAngle = angle - 20 - i * 23;
                Point basePoint = SwordPoint(emberAngle, 42 + i % 2 * 4);
                Point crest = SwordPoint(emberAngle - 8, 51 + i % 2 * 3);
                Point next = SwordPoint(emberAngle + 7, 45);
                Shape(g, i % 2 == 0 ? Color.OrangeRed : Color.Gold, basePoint, crest, next);
            }
        }

        DrawCharacter(g, 0, (int)(5 * follow), false);
        DrawTanjiroSword(g, angle, flame > 0 ? Color.LemonChiffon : Color.LightSteelBlue);
        g.Restore(pose);
    }
}