using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeTrackBarMetropolis : GMTrackBarThemeBase
    {
        public ThemeTrackBarMetropolis()
        {
            MainLineDrawBorder = false;
            MainLineRange1BackColor = Color.FromArgb(247, 138, 9);
            //MainLineRange1BackColor = Color.FromArgb(55, 58, 61);
            MainLineRange2BackColor = Color.FromArgb(157, 157, 157);

            MainLineLength = 3;

            ButtonLength1 = 5;
            ButtonLength2 = 10;

            ThumbButtonColorTable = GetColorTable();
        }

        private ButtonColorTable GetColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();
            table.BackColorNormal = Color.FromArgb(55, 58, 61);
            table.BackColorHover = Color.Black;
            table.BackColorPressed = Color.FromArgb(130, 130, 130);
            return table;
        }
    }
}
