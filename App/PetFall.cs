using System;
using System.Drawing;

sealed partial class Pet
{
    const double LandingDuration = 0.28;
    double fallSpeed, landingUntil;

    bool UpdateFall(int floor, double now, double elapsed)
    {
        if (Top < floor)
        {
            fallSpeed = Math.Min(1250, Math.Max(180, fallSpeed) + 1900 * elapsed);
            Top = Math.Min(floor, Top + (int)Math.Ceiling(fallSpeed * elapsed));
            if (Top == floor)
            {
                fallSpeed = 0;
                landingUntil = now + LandingDuration;
            }
            return true;
        }

        return now < landingUntil;
    }

    void DrawFalling(Graphics g, double now)
    {
        double rush = fallSpeed / 1250;
        int drift = (int)(now * (120 + 160 * rush)) % 40;
        using (var frame = new Pen(Color.Sienna, 2))
        using (var glow = new Pen(Color.Goldenrod, 1))
        {
            for (int i = 0; i < 3; i++)
            {
                int y = -14 - ((drift + i * 29) % 92);
                int side = i == 1 ? 1 : -1;
                int x = side * (36 + i * 6);
                g.DrawLine(frame, x, y, x + side * 18, y - 17);
                g.DrawLine(frame, x + side * 18, y - 17, x + side * 17, y - 35);
                g.DrawLine(glow, x + side * 4, y - 4, x + side * 11, y - 17);
            }
        }

        using (var wind = new Pen(Color.LightGoldenrodYellow, 2))
        {
            int streak = 14 + (int)(22 * rush);
            g.DrawLine(wind, -31, -50, -37, -50 - streak);
            g.DrawLine(wind, 31, -65, 38, -65 - streak);
            g.DrawLine(wind, 41, -31, 46, -31 - streak);
        }

        g.TranslateTransform(0, -41);
        g.RotateTransform((float)(-16 - 21 * rush + 9 * Math.Sin(now * 13)));
        g.TranslateTransform(0, 35);
        DrawCharacter(g, kind, 5, false);
    }

    void DrawLanding(Graphics g, double phase)
    {
        double crouch = 1 - phase;
        var pose = g.Save();
        g.ScaleTransform((float)(1 + 0.14 * crouch), (float)(1 - 0.34 * crouch));
        DrawCharacter(g, kind, 4, false);
        g.Restore(pose);

        int radius = 17 + (int)(43 * phase);
        using (var impact = new Pen(phase < 0.42 ? Color.Khaki : Color.Peru, 2))
            g.DrawEllipse(impact, -radius, -10, 2 * radius, 9);
        for (int i = 0; i < 3; i++)
        {
            int x = radius - i * 7;
            int y = -8 - (int)((15 + i * 7) * Pulse(phase, 0, 0.42, 1));
            Box(g, i == 1 ? Color.Gold : Color.Peru, x, y, 3, 3);
            Box(g, i == 1 ? Color.Gold : Color.Peru, -x - 3, y, 3, 3);
        }
    }
}
