using System;
using System.Drawing;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeFormDevExpress : ThemeFormBase
    {
        public ThemeFormDevExpress()
            : base()
        {
            ThemeName = "DevExpress Default";

            BorderWidth = 2;
            CaptionHeight = 30;
            IconSize = new Size(16, 16);
            ControlBoxOffset = new Point(8, 8);
            ControlBoxSpace = 2;
            SideResizeWidth = 4;
            UseDefaultTopRoundingFormRegion = false;

            CaptionBackColorBottom = Color.White;
            CaptionBackColorTop = Color.White;

            RoundedStyle = RoundStyle.None;

            FormBorderOutterColor = Color.FromArgb(0, 144, 198);
            FormBorderInnerColor = Color.White;
            SetClientInset = false;

            CaptionTextCenter = true;
            CaptionTextColor = Color.FromArgb(102, 102, 102);
            FormBackColor = Color.White;
        }
    }
}
