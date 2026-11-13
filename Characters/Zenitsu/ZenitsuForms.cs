using System;
using System.Drawing;

sealed partial class Pet
{
    static Skill[] ZenitsuSkills()
    {
        const string thunder = "번개의 호흡";
        return new[] {
            new Skill("제1형 · 벽력일섬", DrawZenitsuSkill, 0.95, ZenitsuDashing, group: thunder),
            new Skill("청각 집중", DrawZenitsuListening, 1.55)
        };
    }

    static void DrawZenitsuListening(Graphics g, double phase)
    {
        DrawCharacter(g, 2, 0, true);
        using (var sound = new Pen(Color.Khaki, 2))
        {
            int radius = 9 + (int)(18 * (phase * 3 % 1));
            g.DrawArc(sound, 15 - radius, -54 - radius, radius * 2, radius * 2, -60, 120);
            g.DrawArc(sound, -15 - radius, -54 - radius, radius * 2, radius * 2, 120, 120);
        }
    }
}
