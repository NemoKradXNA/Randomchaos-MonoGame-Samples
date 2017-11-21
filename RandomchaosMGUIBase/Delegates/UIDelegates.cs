using Microsoft.Xna.Framework;

namespace RandomchaosMGUIBase
{
    public delegate void MouseOver(object sender, Vector2 position);
    public delegate void MouseEnter(object sender);
    public delegate void MouseLeave(object sender);

    public delegate void MouseButtonDown(object sender, bool leftButton, Vector2 position);
    public delegate void MouseClick(object sender, bool leftClick, Vector2 position);
    public delegate void MouseDoubleClick(object seneder, bool leftClick, Vector2 point);

    public delegate void GotFocus(object sender);
    public delegate void LostFocus(object sender);

    public delegate void ClosingWindow(object sender);
    public delegate void CloseWindow(object sender);

    public delegate void OnItemSelected(object sender, object item);
    public delegate void OnItemDeSelected(object sender, object item);
}
