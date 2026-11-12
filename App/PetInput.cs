using System;
using System.Drawing;
using System.Windows.Forms;

sealed partial class Pet
{
    Point grabOffset;
    Rectangle dragThreshold;
    bool mouseHeld, dragging;
    int nextClickSkill;
    bool clickSkillQueued;

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left) return;

        mouseHeld = true;
        grabOffset = e.Location;
        Point grab = PointToScreen(e.Location);
        Size dragSize = SystemInformation.DragSize;
        dragThreshold = new Rectangle(grab.X - dragSize.Width / 2, grab.Y - dragSize.Height / 2,
            dragSize.Width, dragSize.Height);
        Capture = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!mouseHeld) return;

        Point cursor = PointToScreen(e.Location);
        if (!dragging && dragThreshold.Contains(cursor)) return;

        dragging = true;
        actionUntil = 0;
        clickSkillQueued = false;
        fallSpeed = landingUntil = 0;
        Rectangle area = Screen.PrimaryScreen.WorkingArea;
        x = Math.Max(area.Left - 44, Math.Min(area.Right - Width + 44, cursor.X - grabOffset.X));
        int y = Math.Max(area.Top, Math.Min(area.Bottom - Height, cursor.Y - grabOffset.Y));
        Location = new Point((int)x, y);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        bool click = e.Button == MouseButtons.Left && mouseHeld && !dragging;
        if (e.Button == MouseButtons.Left)
        {
            mouseHeld = dragging = false;
            Capture = false;
        }

        base.OnMouseUp(e);
        if (click) OnSkillClick();
    }

    protected override void OnMouseCaptureChanged(EventArgs e)
    {
        if (!Capture) mouseHeld = dragging = false;
        base.OnMouseCaptureChanged(e);
    }

    void OnSkillClick()
    {
        double now = clock.Elapsed.TotalSeconds;
        if (now < actionUntil || now < landingUntil || Top < Screen.PrimaryScreen.WorkingArea.Bottom - Height)
        {
            clickSkillQueued = true;
            return;
        }
        PlayNextClickSkill();
    }

    void PlayNextClickSkill()
    {
        var skills = Skills[kind];
        Play(skills[nextClickSkill]);
        nextClickSkill = (nextClickSkill + 1) % skills.Length;
    }
}
