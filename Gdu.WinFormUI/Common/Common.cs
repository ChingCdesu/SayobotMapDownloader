using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Gdu.WinFormUI
{
    public delegate GraphicsPath ButtonForePathGetter(Rectangle rect);

    public enum GMButtonState
    {
        Normal,
        Hover,
        Pressed,
        PressLeave,
    }

    public enum ButtonBorderType
    {
        Rectangle,
        Ellipse,
    }

    public enum MouseOperationType
    {
        Move,
        Down,
        Up,
        Hover,
        Leave,
    }    

    public enum ForePathRatoteDirection
    {
        Down,
        Left,
        Up,
        Right,
    }

    public enum ForePathRenderMode
    {
        Draw,
        Fill,
    }
}
