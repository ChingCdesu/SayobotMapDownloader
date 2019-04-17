using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeTrackBarTTPlayer : GMTrackBarThemeBase
    {
        public ThemeTrackBarTTPlayer()
        {
            ButtonLength1 = ButtonLength2 = 12;
            ButtonOutterSpace2 = 6;
            MainLineLength = 4;

            DrawBackground = true;
            BackColor = Color.FromArgb(49, 76, 111);

            MainLineDrawBorder = false;
            MainLineRadius = 2;
            MainLineRange1BackColor = Color.FromArgb(154, 207, 242);
            MainLineRange2BackColor = Color.FromArgb(66, 114, 176);

            ThumbButtonBorderType = ButtonBorderType.Ellipse;
            ThumbButtonColorTable = GetColorTable();
        }

        private ButtonColorTable GetColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();
            table.BackColorNormal = Color.White;
            table.BackColorHover = Color.FromArgb(230, 230, 230);
            table.BackColorPressed = Color.FromArgb(220, 220, 220);
            return table;
        }
    }
}
