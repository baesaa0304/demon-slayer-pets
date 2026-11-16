using System.Windows.Forms;
using System;
using System.Drawing;

sealed partial class Pet
{
    static readonly Color Ink = Color.FromArgb(39, 29, 39);
    static readonly Color Skin = Color.FromArgb(255, 206, 169);
    static readonly Color NezukoHair = Color.FromArgb(46, 29, 43);
    static readonly Color[] Clothes = { Color.SeaGreen, Color.HotPink, Color.Orange, Color.Sienna, Color.Maroon };

    static void Box(Graphics g, Color color, int x, int y, int w, int h)
    {
        using (var brush = new SolidBrush(color)) g.FillRectangle(brush, x, y, w, h);
    }

    static double Progress(double phase, double start, double end)
    {
        return Math.Max(0, Math.Min(1, (phase - start) / (end - start)));
    }

    static double Pulse(double phase, double start, double peak, double end)
    {
        return Progress(phase, start, peak) - Progress(phase, peak, end);
    }

    static void Weapon(Graphics g, int shoulderX, int shoulderY, double angle, Color sleeve, Color blade)
    {
        double radians = angle * Math.PI / 180;
        var hand = P(shoulderX + (int)(14 * Math.Cos(radians)), shoulderY + (int)(14 * Math.Sin(radians)));
        var tip = P(hand.X + (int)(23 * Math.Cos(radians)), hand.Y + (int)(23 * Math.Sin(radians)));
        using (var outline = new Pen(Ink, 9)) g.DrawLine(outline, shoulderX, shoulderY, hand.X, hand.Y);
        using (var cloth = new Pen(sleeve, 6)) g.DrawLine(cloth, shoulderX, shoulderY, hand.X, hand.Y);
        using (var edge = new Pen(Ink, 5)) g.DrawLine(edge, hand, tip);
        using (var steel = new Pen(blade, 3)) g.DrawLine(steel, hand, tip);
        Oval(g, Skin, hand.X - 3, hand.Y - 3, 6, 6);
    }

    static void Shape(Graphics g, Color color, params Point[] points)
    {
        using (var brush = new SolidBrush(color)) g.FillPolygon(brush, points);
    }

    static void Oval(Graphics g, Color color, int x, int y, int w, int h)
    {
        using (var brush = new SolidBrush(color)) g.FillEllipse(brush, x, y, w, h);
    }

    static Point P(int x, int y) { return new Point(x, y); }

    static void DrawBodyLightning(Graphics g, double phase, Color glow, Color core)
    {
        int flicker = (int)(phase * 28);
        int offset = flicker % 3 * 3;
        var pose = g.Save();
        if (flicker % 2 == 1) g.ScaleTransform(-1, 1);

        Point[] shoulder = { P(-33, -56 + offset), P(-23, -48), P(-28, -42), P(-17, -37), P(-22, -31), P(-11, -27) };
        Point[] waist = { P(-27, -22), P(-15, -28 + offset), P(-11, -21), P(4, -25), P(12, -16 + offset), P(26, -21), P(34, -13) };
        Point[] side = { P(32, -57), P(23, -49 + offset), P(29, -44), P(21, -35), P(27, -31), P(19, -23) };
        using (var aura = new Pen(glow, 4))
        using (var flash = new Pen(core, 1))
        {
            foreach (var arc in new[] { shoulder, waist, side })
            {
                g.DrawLines(aura, arc);
                g.DrawLines(flash, arc);
            }
        }
        g.Restore(pose);
    }

    static void DrawCharacter(Graphics g, int kind, int stride, bool sleeping, bool unmasked = false, bool threeQuarter = false)
    {
        Color coat = Clothes[kind];
        bool tanjiroPose = threeQuarter && kind == 0;
        int frontFoot = tanjiroPose ? 8 : 0;
        int backFoot = tanjiroPose ? -2 : 0;

        Oval(g, Ink, -17 + stride + backFoot, -11, 15, 11);
        Oval(g, Ink, 3 - stride + frontFoot, -11, 15, 11);
        Oval(g, Color.LightGray, -16 + stride + backFoot, -7, 11, 3);
        Oval(g, Color.LightGray, 6 - stride + frontFoot, -7, 10, 3);

        Shape(g, Ink, P(-16,-38), P(16,-38), P(24,-17), P(17,-9), P(-18,-9), P(-25,-17));
        using (var fabric = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(-22, -36, 44, 25), ControlPaint.Light(coat, 0.32f),
            ControlPaint.Dark(coat, 0.25f), 58f))
            g.FillPolygon(fabric, new[] { P(-15,-35), P(15,-35), P(21,-18), P(15,-12), P(-15,-12), P(-22,-18) });

        if (kind == 0)
        {
            Box(g, Ink, -14, -31, 7, 8);
            Box(g, Ink, 7, -31, 7, 8);
            Box(g, Ink, -20, -22, 7, 7);
            Box(g, Ink, 13, -22, 7, 7);
        }
        if (kind == 1)
        {
            Shape(g, Color.MistyRose, P(-13,-30), P(-9,-33), P(14,-19), P(10,-17));
            Shape(g, Color.MistyRose, P(13,-30), P(9,-33), P(-14,-19), P(-10,-17));
        }
        if (kind == 2)
        {
            Shape(g, Color.FloralWhite, P(-14,-29), P(-18,-22), P(-10,-22));
            Shape(g, Color.FloralWhite, P(13,-29), P(9,-22), P(17,-22));
            Shape(g, Color.FloralWhite, P(13,-21), P(9,-15), P(17,-15));
        }

        if (kind == 4)
        {
            DrawGiyu(g, threeQuarter);
            return;
        }

        if (kind == 3)
        {
            Shape(g, Skin, P(-15,-35), P(15,-35), P(19,-23), P(12,-20), P(-13,-20), P(-19,-23));
            Shape(g, Color.Peru, P(-16,-20), P(-8,-23), P(2,-20), P(14,-23), P(19,-13), P(-19,-13));
            if (unmasked) DrawInosukeFace(g);
            else DrawBoarMask(g);
            return;
        }

        Shape(g, Ink, P(-7,-36), P(8,-36), P(6,-14), P(-6,-14));
        Box(g, Color.Ivory, -7, -19, 14, 3);
        Oval(g, Color.Goldenrod, -1, -29, 3, 3);

        if (kind == 1)
        {
            Shape(g, Ink, P(-23,-60), P(19,-64), P(25,-45), P(23,-26), P(28,-17),
                P(14,-19), P(10,-34), P(-12,-33), P(-15,-19), P(-27,-17), P(-23,-31));
            Shape(g, Color.Sienna, P(-22,-33), P(-17,-32), P(-17,-22), P(-24,-20));
            Shape(g, Color.Sienna, P(18,-33), P(21,-35), P(22,-26), P(25,-20), P(17,-22));
        }

        Oval(g, Ink, -24, -74, 48, 46);
        Oval(g, Color.Peru, -21, -69, 43, 39);
        SculptedFace(g, -20, -70, 40, 38);
        if (kind == 0)
        {
            Shape(g, Ink, P(-24,-49), P(-29,-56), P(-23,-61), P(-24,-71), P(-15,-68),
                P(-10,-78), P(-3,-74), P(5,-79), P(9,-74), P(19,-76), P(18,-70),
                P(24,-70), P(23,-61), P(27,-55), P(18,-47), P(16,-58), P(5,-66),
                P(-1,-57), P(-3,-65), P(-14,-56), P(-13,-63));
            Shape(g, Color.IndianRed, P(-22,-61), P(-16,-69), P(-9,-69), P(-6,-75),
                P(-3,-71), P(4,-76), P(8,-71), P(15,-73), P(20,-68), P(20,-63),
                P(13,-68), P(4,-70), P(-4,-67), P(-13,-66));
            Shape(g, Color.FromArgb(241, 150, 141), P(-14,-70), P(-6,-73), P(-9,-69));
            Shape(g, Color.FromArgb(201, 102, 111), P(2,-74), P(15,-72), P(7,-71));
            Shape(g, Color.Firebrick, P(-12,-58), P(-7,-60), P(-3,-57), P(-5,-53), P(-11,-54));
            Box(g, Ink, -22, -45, 7, 15);
            Box(g, Color.Ivory, -21, -43, 5, 12);
            Box(g, Color.Firebrick, -20, -41, 3, 4);
        }
        if (kind == 1)
        {
            Shape(g, NezukoHair, P(-24,-48), P(-23,-64), P(-16,-73), P(-5,-76), P(11,-75),
                P(21,-68), P(24,-56), P(20,-47), P(14,-58), P(9,-67), P(1,-61),
                P(-4,-60), P(-1,-65), P(-11,-61), P(-18,-49));
            Shape(g, Color.FromArgb(99, 66, 87), P(-19,-65), P(-10,-72), P(3,-73), P(-10,-68), P(-16,-62));
            Shape(g, Color.HotPink, P(11,-66), P(11,-73), P(17,-70), P(24,-73),
                P(24,-64), P(18,-67), P(15,-60));
        }
        if (kind == 2)
        {
            Shape(g, Ink, P(-25,-47), P(-27,-62), P(-20,-73), P(-8,-77), P(10,-77),
                P(22,-71), P(27,-58), P(23,-45), P(15,-51), P(10,-56), P(6,-51),
                P(1,-57), P(-4,-52), P(-10,-58), P(-16,-49), P(-18,-55));
            Shape(g, Color.DarkOrange, P(-23,-51), P(-25,-61), P(-18,-72), P(-7,-75),
                P(9,-75), P(20,-70), P(25,-58), P(22,-49), P(10,-59), P(6,-54),
                P(1,-60), P(-4,-55), P(-10,-61), P(-16,-53), P(-18,-58));
            Shape(g, Color.Gold, P(-24,-62), P(-17,-71), P(-6,-75), P(8,-75), P(19,-70),
                P(24,-59), P(15,-63), P(8,-61), P(1,-66), P(-6,-62), P(-11,-66), P(-17,-61));
        }

        if (kind == 2)
        {
            Shape(g, Color.LemonChiffon, P(-19,-68), P(-10,-73), P(2,-74), P(-7,-70));
            Shape(g, Color.Khaki, P(5,-73), P(16,-71), P(20,-66), P(11,-69));
        }

        Color iris = kind == 0 ? Color.Maroon : kind == 1 ? Color.MediumVioletRed : Color.SaddleBrown;
        DrawEye(g, tanjiroPose ? -8 : -14, iris, sleeping);
        DrawEye(g, tanjiroPose ? 9 : 5, iris, sleeping, tanjiroPose ? 0.70f : 1);
        if (kind == 1)
        {
            Oval(g, Ink, -12, -41, 26, 11);
            Oval(g, Color.SeaGreen, -11, -40, 24, 9);
            Box(g, Color.PaleGreen, -8, -39, 18, 2);
            Box(g, Color.DarkGreen, 7, -39, 2, 7);
        }
        else
        {
            Oval(g, Color.IndianRed, tanjiroPose ? 5 : -2, -39, 6, 3);
            if (tanjiroPose)
            {
                Shape(g, Color.PeachPuff, P(7,-45), P(12,-42), P(7,-41));
                Shape(g, Color.DarkSeaGreen, P(-24,-27), P(-16,-34), P(-9,-27), P(-17,-18), P(-25,-20));
                Shape(g, Ink, P(-22,-27), P(-17,-30), P(-13,-25), P(-18,-21));
                Oval(g, Skin, -24, -22, 8, 7);
            }
        }
    }

    static void SculptedFace(Graphics g, int x, int y, int width, int height)
    {
        var bounds = new Rectangle(x, y, width, height);
        using (var skin = new System.Drawing.Drawing2D.LinearGradientBrush(
            bounds, Color.Linen, Color.FromArgb(215, 143, 119), 65f))
            g.FillEllipse(skin, bounds);
    }

    static void DrawEye(Graphics g, int x, Color iris, bool sleeping, float width = 1, bool loweredLid = false)
    {
        if (sleeping)
        {
            using (var pen = new Pen(Ink, 2)) g.DrawArc(pen, x, -49, 9, 6, 12, 156);
            return;
        }

        var eye = g.Save();
        g.TranslateTransform(x, 0);
        g.ScaleTransform(width, 1);
        x = 0;
        Oval(g, Ink, x, loweredLid ? -51 : -53, 11, loweredLid ? 11 : 13);
        Oval(g, Color.FloralWhite, x + 1, loweredLid ? -50 : -51, 9, loweredLid ? 9 : 10);
        Oval(g, iris, x + 3, loweredLid ? -50 : -51, 7, loweredLid ? 9 : 10);
        Oval(g, Ink, x + 5, -50, 4, 7);
        if (!loweredLid) Oval(g, Color.White, x + 4, -50, 3, 3);
        if (loweredLid)
        {
            using (var lid = new Pen(Ink, 1))
                g.DrawLine(lid, 2, -50, 9, -50);
            Oval(g, Color.White, 4, -49, 2, 2);
        }
        g.Restore(eye);
    }

}

