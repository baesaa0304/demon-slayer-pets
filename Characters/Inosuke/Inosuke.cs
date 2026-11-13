using System;
using System.Drawing;

sealed partial class Pet
{
    static void DrawInosukeSkill(Graphics g, double phase)
    {
        double crouch = Pulse(phase, 0, 0.19, 0.32);
        double jump = Pulse(phase, 0.23, 0.48, 0.76);
        double swing = Progress(phase, 0.37, 0.58) - Progress(phase, 0.72, 1);
        g.TranslateTransform(0, (float)(-30 * jump));
        var pose = g.Save();
        g.ScaleTransform(1, (float)(1 - 0.24 * crouch));
        g.RotateTransform((float)(9 * swing));
        DrawCharacter(g, 3, (int)(5 * jump), false);
        DrawInosukeSword(g, -14, -26, -115 - 85 * swing);
        DrawInosukeSword(g, 14, -26, -65 + 85 * swing);
        g.Restore(pose);

        if (phase >= 0.42 && phase <= 0.67)
        {
            using (var slash = new Pen(Color.LightSteelBlue, 3))
            {
                g.DrawArc(slash, -48, -63, 54, 61, 110, 105);
                g.DrawArc(slash, -6, -63, 54, 61, -35, 105);
            }
        }
    }

    static void DrawInosukeCross(Graphics g, double phase)
    {
        double cross = Pulse(phase, 0.19, 0.41, 0.55);
        double open = Pulse(phase, 0.41, 0.62, 0.97);
        var pose = g.Save();
        g.TranslateTransform((float)(11 * open), 0);
        g.RotateTransform((float)(-9 * cross + 20 * open));
        g.ScaleTransform(1, (float)(1 - 0.16 * cross));
        DrawCharacter(g, 3, (int)(5 * open), false);
        DrawInosukeSword(g, -13, -29, -132 + 100 * cross - 35 * open);
        DrawInosukeSword(g, 13, -29, -48 - 100 * cross + 35 * open);
        g.Restore(pose);
        if (open > 0.25)
        {
            using (var slash = new Pen(Color.LightSteelBlue, 4))
            {
                g.DrawLine(slash, -27, -64, 38, -24);
                g.DrawLine(slash, -21, -25, 41, -62);
            }
        }
    }

    static void DrawInosukeSword(Graphics g, int shoulderX, int shoulderY, double angle, bool held = true)
    {
        double radians = angle * Math.PI / 180;
        int side = Math.Sign(shoulderX);
        int handX = shoulderX + (int)(18 * Math.Cos(radians));
        int handY = shoulderY + (int)(18 * Math.Sin(radians));
        if (held) DrawInosukeArm(g, shoulderX, shoulderY, handX, handY);

        var grip = g.Save();
        g.TranslateTransform(handX, handY);
        g.RotateTransform((float)angle);
        g.ScaleTransform(1, side);
        Shape(g, Ink, P(-7,-3), P(7,-4), P(9,0), P(13,-5), P(16,-1), P(20,-5),
            P(23,-2), P(29,-4), P(32,0), P(28,5), P(5,5), P(2,3), P(-7,3));
        Shape(g, Color.LightSteelBlue, P(5,-2), P(9,2), P(13,-3), P(16,1), P(20,-3),
            P(23,0), P(29,-2), P(30,0), P(27,3), P(5,3));
        using (var edge = new Pen(Color.AliceBlue, 2)) g.DrawLine(edge, 6, 2, 27, 2);
        Box(g, Color.SaddleBrown, -6, -2, 10, 4);
        Box(g, Color.BurlyWood, 3, -4, 2, 8);
        if (held) Oval(g, Skin, -4, -3, 7, 7);
        g.Restore(grip);
    }

    static void DrawInosukeArm(Graphics g, int shoulderX, int shoulderY, int handX, int handY)
    {
        using (var outline = new Pen(Ink, 11)) g.DrawLine(outline, shoulderX, shoulderY, handX, handY);
        using (var arm = new Pen(Color.Peru, 8)) g.DrawLine(arm, shoulderX, shoulderY, handX, handY);
        using (var light = new Pen(Skin, 5)) g.DrawLine(light, shoulderX, shoulderY - 1, handX, handY - 1);
    }

    static void DrawInosukeUnmask(Graphics g, double phase)
    {
        double lift = Progress(phase, 0.08, 0.33) - Progress(phase, 0.73, 0.97);
        if (lift <= 0)
        {
            DrawCharacter(g, 3, 0, false);
            return;
        }

        int maskX = (int)(36 * lift);
        int maskY = (int)(-22 * lift);
        DrawCharacter(g, 3, 0, false, true);
        DrawInosukeArm(g, 15, -29, maskX - 12, maskY - 34);
        var mask = g.Save();
        g.TranslateTransform(maskX, maskY - 40);
        g.RotateTransform((float)(25 * lift));
        g.ScaleTransform((float)(1 - 0.3 * lift), (float)(1 - 0.3 * lift));
        g.TranslateTransform(0, 40);
        DrawBoarMask(g);
        g.Restore(mask);
        Oval(g, Skin, maskX - 15, maskY - 38, 7, 7);

        DrawInosukeSparkle(g, -34, -66, 8 * Pulse(phase, 0.22, 0.36, 0.51));
        DrawInosukeSparkle(g, -20, -85, 6 * Pulse(phase, 0.31, 0.45, 0.58));
        DrawInosukeSparkle(g, 8, -90, 8 * Pulse(phase, 0.39, 0.52, 0.67));
        DrawInosukeSparkle(g, -32, -49, 7 * Pulse(phase, 0.48, 0.61, 0.77));
        DrawInosukeSparkle(g, 25, -77, 6 * Pulse(phase, 0.57, 0.70, 0.83));
    }

    static void DrawInosukeSparkle(Graphics g, int x, int y, double size)
    {
        int radius = (int)Math.Round(size);
        if (radius < 2) return;

        Shape(g, Color.Goldenrod, P(x,y-radius-1), P(x+2,y-2), P(x+radius,y), P(x+2,y+2),
            P(x,y+radius+1), P(x-2,y+2), P(x-radius,y), P(x-2,y-2));
        Shape(g, Color.LemonChiffon, P(x,y-radius+1), P(x+1,y-1), P(x+radius-1,y), P(x+1,y+1),
            P(x,y+radius-1), P(x-1,y+1), P(x-radius+1,y), P(x-1,y-1));
        Box(g, Color.White, x, y, 1, 1);
    }

    static void DrawInosukeFace(Graphics g)
    {
        Color blueBlack = Color.FromArgb(32, 39, 62);
        Color blueTip = Color.FromArgb(67, 91, 132);
        Shape(g, Ink, P(-23,-63), P(-17,-72), P(-5,-77), P(10,-77), P(22,-70), P(25,-55),
            P(23,-36), P(16,-28), P(8,-37), P(-12,-35), P(-20,-29), P(-26,-40));
        Shape(g, blueBlack, P(-21,-62), P(-16,-70), P(-5,-75), P(9,-75), P(20,-69), P(23,-55),
            P(21,-37), P(15,-32), P(8,-42), P(-13,-38), P(-20,-33), P(-24,-41));
        Shape(g, blueTip, P(-23,-49), P(-16,-54), P(-12,-41), P(-20,-34), P(-23,-41));
        Shape(g, blueTip, P(16,-50), P(22,-49), P(21,-38), P(15,-33), P(12,-41));
        Shape(g, Skin, P(-17,-58), P(-11,-64), P(8,-64), P(17,-55), P(17,-44), P(10,-35),
            P(1,-32), P(-10,-37), P(-17,-45));
        Shape(g, blueBlack, P(-19,-58), P(-11,-70), P(12,-68), P(20,-59), P(17,-48), P(11,-58),
            P(8,-63), P(2,-56), P(0,-63), P(-8,-57), P(-6,-64), P(-15,-54));
        DrawEye(g, -14, Color.SeaGreen, false);
        DrawEye(g, 5, Color.SeaGreen, false);
        Oval(g, Color.FromArgb(233, 170, 137), 0, -44, 2, 2);
        using (var smile = new Pen(Color.Sienna, 2)) g.DrawArc(smile, -2, -42, 9, 6, 12, 145);
    }

    static void DrawBoarMask(Graphics g)
    {
        Shape(g, Ink, P(-24,-69), P(-12,-65), P(12,-65), P(23,-71), P(24,-48),
            P(18,-36), P(0,-30), P(-20,-37), P(-26,-50));
        Shape(g, Color.SlateGray, P(-21,-66), P(-11,-59), P(12,-61), P(20,-67),
            P(21,-48), P(15,-37), P(0,-33), P(-17,-39), P(-23,-49));
        Shape(g, Color.RosyBrown, P(-20,-62), P(-18,-54), P(-13,-57));
        Shape(g, Color.RosyBrown, P(19,-62), P(12,-56), P(19,-55));
        Shape(g, Color.LightSlateGray, P(-15,-55), P(-3,-65), P(14,-58), P(20,-49),
            P(10,-40), P(-13,-42), P(-20,-49));
        Oval(g, Ink, -14, -52, 10, 10);
        Oval(g, Ink, 6, -52, 10, 10);
        Oval(g, Color.LightSkyBlue, -12, -50, 6, 6);
        Oval(g, Color.LightSkyBlue, 8, -50, 6, 6);
        Shape(g, Color.Ivory, P(-12,-39), P(-8,-39), P(-11,-46));
        Shape(g, Color.Ivory, P(14,-39), P(10,-39), P(13,-46));
        Oval(g, Ink, -9, -44, 21, 12);
        Oval(g, Color.LightPink, -8, -43, 19, 10);
        Oval(g, Ink, -4, -40, 3, 4);
        Oval(g, Ink, 5, -40, 3, 4);
    }
}
