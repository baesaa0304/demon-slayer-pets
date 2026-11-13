using System;
using System.Drawing;

sealed partial class Pet
{
    static void DrawHandDemon(Graphics g, double now, bool staggered, int health)
    {
        Color skin = staggered ? Color.PaleGreen : Color.FromArgb(119, 157, 83);
        Color shadow = Color.FromArgb(69, 100, 60);
        using (var outline = new Pen(Ink, 14))
        using (var flesh = new Pen(skin, 10))
        {
            for (int side = -1; side <= 1; side += 2)
            for (int i = 0; i < 4; i++)
            {
                int endX = side * (54 + i % 2 * 8);
                int endY = -96 + i * 22 + (int)(5 * Math.Sin(now * 7 + i + side));
                Point[] arm = { P(side * 18, -55 + i * 9), P(side * (36 + i * 3), endY + 13), P(endX, endY) };
                g.DrawLines(outline, arm);
                g.DrawLines(flesh, arm);
                Oval(g, Ink, endX - 8, endY - 9, 16, 17);
                Oval(g, skin, endX - 6, endY - 7, 12, 13);
                for (int finger = -1; finger <= 1; finger++)
                    Box(g, skin, endX + finger * 4 - 1, endY - 11, 3, 8);
            }
        }

        Oval(g, Ink, -43, -84, 86, 84);
        Oval(g, shadow, -40, -81, 80, 77);
        Oval(g, skin, -35, -81, 70, 55);
        Oval(g, Ink, -29, -111, 58, 51);
        Oval(g, skin, -25, -107, 50, 43);
        Shape(g, shadow, P(-24,-99), P(-13,-108), P(10,-108), P(22,-99), P(5,-94), P(-9,-97));
        Shape(g, Ink, P(-20,-91), P(-5,-86), P(-6,-80), P(-18,-83));
        Shape(g, Ink, P(20,-91), P(5,-86), P(6,-80), P(18,-83));
        Box(g, Color.Gold, -15, -87, 4, 5);
        Box(g, Color.Gold, 11, -87, 4, 5);
        Oval(g, Ink, -13, -78, 26, 12);
        Shape(g, Color.Ivory, P(-10,-76), P(-4,-76), P(-7,-70));
        Shape(g, Color.Ivory, P(10,-76), P(4,-76), P(7,-70));

        using (var fold = new Pen(shadow, 4))
        {
            g.DrawArc(fold, -29, -67, 58, 27, 0, 180);
            g.DrawArc(fold, -24, -46, 48, 25, 0, 180);
        }
        for (int i = 0; i < 3; i++)
            Box(g, i < health ? Color.OrangeRed : Ink, -13 + i * 9, -10, 7, 3);
    }
}
