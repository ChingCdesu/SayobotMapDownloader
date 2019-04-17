using System;
using System.Drawing;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeFormNew : ThemeFormBase
    {
        public ThemeFormNew()
            : base()
        {
            // about theme
            ThemeName = "A New Look Theme";

            UseDefaultTopRoundingFormRegion = false;
            BorderWidth = 2;
            CaptionHeight = 36;
            IconSize = new Size(24, 24);
            ControlBoxOffset = new Point(6, 9);
            ControlBoxSpace = 2;
            MaxBoxSize = MinBoxSize = CloseBoxSize = new Size(32, 18);
            SideResizeWidth = 4;

            CaptionBackColorBottom = Color.LightSlateGray;
            CaptionBackColorTop = ColorHelper.GetLighterColor(CaptionBackColorBottom, 40);
            
            RoundedStyle = RoundStyle.None;

            FormBorderOutterColor = Color.Black;
            FormBorderInnerColor = Color.FromArgb(200, Color.White);
            SetClientInset = false;
        }
    }
}
