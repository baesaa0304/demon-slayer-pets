using System.Drawing;

sealed partial class Pet
{
    static Skill[] NezukoSkills()
    {
        const string demon = "혈귀술";
        return new[] {
            new Skill("상자에 숨기", DrawNezukoSkill),
            new Skill("도약 발차기", DrawNezukoKick, 1.05),
            new Skill("폭혈 · 분홍 불꽃", DrawNezukoBurst, 1.1, group: demon),
            new Skill("크기 변화", DrawNezukoSizeChange, 1.55),
            new Skill("고속 재생", DrawNezukoRegeneration, 1.55),
            new Skill("잠으로 회복", DrawNezukoRest, 2.15),
        };
    }

    static void DrawNezukoSizeChange(Graphics g, double phase)
    {
        double small = Pulse(phase, 0, 0.32, 0.61);
        double large = Pulse(phase, 0.54, 0.73, 1);
        float size = (float)(1 - 0.49 * small + 0.20 * large);
        g.ScaleTransform(size, size);
        DrawCharacter(g, 1, (int)(5 * large), false);
    }

    static void DrawNezukoRegeneration(Graphics g, double phase)
    {
        DrawCharacter(g, 1, 0, false);
        int end = 16 + (int)(18 * Progress(phase, 0.15, 0.65));
        using (var outline = new Pen(Ink, 8)) g.DrawLine(outline, 13, -29, end, -23);
        using (var arm = new Pen(Skin, 5)) g.DrawLine(arm, 13, -29, end, -23);
        DrawPinkSparks(g, end, -23, Pulse(phase, 0, 0.45, 1));
    }

    static void DrawNezukoRest(Graphics g, double phase)
    {
        var state = g.Save();
        g.ScaleTransform(1, (float)(1 - 0.35 * Pulse(phase, 0, 0.27, 1)));
        DrawCharacter(g, 1, 0, true);
        g.Restore(state);
        using (var letters = new Font(SystemFonts.DefaultFont.FontFamily, 10, FontStyle.Bold))
            g.DrawString("z Z", letters, Brushes.Lavender, 19, -62 - (int)(15 * phase));
    }

}
