using System;
using System.Drawing;
using System.Drawing.Drawing2D;

sealed partial class Pet
{
    const int HandDemonHealth = 5;

    static void DrawHandDemon(Graphics g, double now, bool staggered)
    {
        Color skin = staggered ? Color.Khaki : Color.FromArgb(181, 181, 131);
        Color shade = Color.FromArgb(113, 119, 80);
        Color nail = Color.FromArgb(128, 45, 59);

        for (int side = -1; side <= 1; side += 2)
        {
            var pose = g.Save();
            g.ScaleTransform(side, 1);
            for (int i = 0; i < 4; i++)
            {
                int wrist = 48 + i % 2 * 14;
                int y = -113 + i * 27 + (int)((staggered ? 10 : 4) * Math.Sin(now * 6 + i + side));
                DrawDemonArm(g, shade, skin, nail,
                    P(21,-94 + i * 18), P(43 + i * 3,y + 18), P(wrist,y), i == 0 ? -45 : 40);
            }
            g.Restore(pose);
        }

        Shape(g, Ink, P(-39,-109), P(34,-111), P(49,-82), P(52,-39), P(39,-13), P(-41,-13), P(-54,-50));
        Oval(g, Color.FromArgb(61, 66, 47), -47, -108, 94, 95);
        DrawDemonArm(g, skin, shade, nail, P(-28,-53), P(-40,-25), P(-38,-21), 79);
        DrawDemonArm(g, skin, shade, nail, P(25,-49), P(38,-24), P(38,-21), 105);
        DrawDemonArm(g, skin, shade, nail, P(-37,-77), P(-23,-52), P(17,-56), -4);
        DrawDemonArm(g, skin, shade, nail, P(38,-57), P(22,-36), P(-24,-34), 168);
        DrawDemonArm(g, shade, skin, nail, P(-36,-32), P(-8,-18), P(25,-26), -18);
        DrawDemonArm(g, skin, shade, nail, P(36,-104), P(32,-80), P(-19,-76), 183);

        Oval(g, Ink, -20, -126, 40, 49);
        Oval(g, shade, -17, -123, 34, 40);
        for (int side = -1; side <= 1; side += 2)
        {
            var eye = g.Save();
            g.ScaleTransform(side, 1);
            Shape(g, Ink, P(2,-100), P(4,-106), P(9,-110), P(14,-109), P(17,-104),
                P(16,-99), P(12,-103), P(8,-103));
            Shape(g, Color.MistyRose, P(3,-101), P(6,-106), P(10,-108), P(14,-106),
                P(16,-101), P(12,-104), P(8,-104));
            Box(g, nail, 10, -108, 2, 4);
            g.Restore(eye);
        }

        DrawDemonArm(g, skin, shade, nail, P(-39,-101), P(-28,-124), P(0,-125), 90);
        DrawDemonArm(g, skin, shade, nail, P(29,-104), P(24,-88), P(4,-89), -151);
    }

    static void DrawDemonArm(Graphics g, Color skin, Color shade, Color nail,
        Point shoulder, Point elbow, Point wrist, int angle)
    {
        using (var outline = new Pen(Ink, 19) { LineJoin = LineJoin.Round })
        using (var flesh = new Pen(skin, 15) { LineJoin = LineJoin.Round })
        using (var crease = new Pen(shade, 1))
        {
            Point[] arm = { shoulder, elbow, wrist };
            g.DrawLines(outline, arm);
            g.DrawLines(flesh, arm);
            g.DrawLine(crease, elbow.X - 4, elbow.Y + 2, elbow.X + 4, elbow.Y + 5);
        }

        var hand = g.Save();
        g.TranslateTransform(wrist.X, wrist.Y);
        g.RotateTransform(angle);
        Shape(g, Ink, P(-5,-7), P(8,-9), P(12,-9), P(13,-6), P(19,-5), P(19,3),
            P(15,7), P(9,6), P(5,11), P(0,9), P(2,5), P(-5,7));
        Shape(g, skin, P(-5,-6), P(7,-7), P(10,-5), P(17,-4), P(17,3),
            P(14,5), P(8,4), P(4,9), P(2,8), P(4,4), P(-5,6));
        using (var fingers = new Pen(shade, 1))
        {
            g.DrawLine(fingers, 11, -2, 17, -2);
            g.DrawLine(fingers, 11, 1, 17, 1);
            g.DrawLine(fingers, 10, 3, 15, 4);
        }
        Box(g, nail, 16, -4, 2, 2);
        Box(g, nail, 16, -1, 2, 2);
        Box(g, nail, 15, 2, 2, 2);
        g.Restore(hand);
    }

    static void DrawShavedDemonHead(Graphics g, Color skin, bool child)
    {
        Oval(g, Ink, -25, -60, 50, 19);
        Oval(g, Color.RosyBrown, -23, -57, 46, 13);
        Oval(g, Ink, -22, -79, 44, 50);
        Oval(g, Color.RosyBrown, -20, -77, 41, 46);
        Oval(g, skin, -18, -76, 37, 42);
        if (child)
        {
            Shape(g, Color.SlateGray, P(-15,-68), P(-9,-74), P(-2,-75), P(-5,-70), P(-8,-67));
            Box(g, Color.SlateGray, -3, -74, 3, 2);
            Box(g, Color.SlateGray, 3, -73, 2, 2);
        }
        else
        {
            using (var crease = new Pen(Color.RosyBrown, 1))
                g.DrawArc(crease, -12, -70, 23, 9, 205, 135);
        }
    }

    static void DrawDemonFace(Graphics g, bool child, bool woman)
    {
        Color iris = child ? Color.LightSteelBlue : Color.DarkRed;
        for (int side = -1; side <= 1; side += 2)
        {
            var eye = g.Save();
            g.ScaleTransform(side, 1);
            Shape(g, Ink, P(2,-49), P(7,-55), P(17,-58), P(16,-48), P(11,-44), P(5,-44));
            Shape(g, iris, P(4,-49), P(9,-54), P(15,-56), P(14,-48), P(10,-46), P(5,-46));
            Oval(g, Color.Ivory, 8, -52, child ? 6 : 4, 5);
            Box(g, Ink, 10, -50, 1, 2);
            using (var crease = new Pen(Color.RosyBrown, 1))
            {
                g.DrawLines(crease, new[] { P(5,-43), P(10,-42), P(16,-45) });
                g.DrawLines(crease, new[] { P(17,-47), P(19,-43), P(16,-40) });
            }
            g.Restore(eye);
        }

        if (woman)
        {
            Shape(g, Ink, P(-9,-39), P(2,-38), P(11,-43), P(6,-35), P(-3,-34));
            Shape(g, Color.Ivory, P(4,-38), P(8,-40), P(6,-35));
            return;
        }

        Oval(g, Ink, -10, -43, 21, 15);
        Oval(g, child ? Color.DarkSlateGray : Color.Maroon, -7, -40, 15, 10);
        Shape(g, Color.Ivory, P(-8,-41), P(-3,-42), P(-5,-35));
        Shape(g, Color.Ivory, P(4,-42), P(9,-41), P(6,-35));
    }

    static void DrawSpiderDemon(Graphics g, double now)
    {
        Color shadow = Color.FromArgb(55, 37, 49);
        Color flesh = Color.FromArgb(169, 113, 131);
        using (var edge = new Pen(Ink, 10))
        using (var limb = new Pen(flesh, 6))
        {
            for (int side = -1; side <= 1; side += 2)
            for (int i = 0; i < 4; i++)
            {
                int bend = (int)(3 * Math.Sin(now * 8 + i + side));
                Point[] leg = { P(side * 14,-41), P(side * (25 + i * 3),-65 + i * 11 + bend),
                    P(side * (30 + i * 3),-52 + i * 15) };
                g.DrawLines(edge, leg);
                g.DrawLines(limb, leg);
            }
        }
        Oval(g, Ink, -24, -80, 48, 65);
        Oval(g, shadow, -20, -76, 40, 57);
        Shape(g, flesh, P(-18,-65), P(-10,-75), P(7,-72), P(16,-60), P(9,-64),
            P(4,-56), P(-3,-65), P(-11,-59));
        Shape(g, flesh, P(-18,-43), P(-11,-49), P(-9,-38), P(5,-35), P(16,-43), P(12,-26),
            P(0,-20), P(-12,-27));
        Oval(g, Ink, -12, -40, 23, 12);
        Shape(g, Color.Ivory, P(-9,-38), P(-4,-39), P(-6,-32));
        Shape(g, Color.Ivory, P(4,-39), P(8,-38), P(6,-32));
        Point[] eyes = { P(-10,-55), P(7,-51), P(0,-61) };
        foreach (Point eye in eyes)
        {
            Oval(g, Ink, eye.X - 5, eye.Y - 5, 11, 11);
            Oval(g, Color.Ivory, eye.X - 3, eye.Y - 3, 7, 7);
            Oval(g, Color.MediumPurple, eye.X - 1, eye.Y - 2, 4, 5);
            Box(g, Ink, eye.X, eye.Y - 1, 1, 3);
        }
    }
}
