using System;
using System.Drawing;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeFormVS2013 : ThemeFormBase
    {
        public ThemeFormVS2013()
            : base()
        {
            ThemeName = "A VS2013 Look";

            BorderWidth = 1;
            CaptionHeight = 38;
            IconSize = new Size(22, 22);
            CloseBoxSize = MaxBoxSize = MinBoxSize = new Size(34, 27);
            ControlBoxOffset = new Point(1, 1);
            ControlBoxSpace = 1;
            SideResizeWidth = 6;
            UseDefaultTopRoundingFormRegion = false;

            CaptionBackColorBottom = CaptionBackColorTop = Color.FromArgb(214, 219, 233);

            RoundedStyle = RoundStyle.None;

            FormBorderOutterColor = Color.FromArgb(0, 0, 0);
            SetClientInset = false;

            CaptionTextColor = Color.FromArgb(0, 0, 0);
            FormBackColor = Color.FromArgb(42, 58, 86);

            CloseBoxColor = ButtonColorTable.GetColorTableVs2013Theme();
            MaxBoxColor = MinBoxColor = CloseBoxColor;
        }
    }
}
