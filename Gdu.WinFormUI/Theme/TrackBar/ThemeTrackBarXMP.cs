using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeTrackBarXMP : ThemeTrackBarTTPlayer
    {
        public ThemeTrackBarXMP()
        {
            MainLineRadius = 0;
            MainLineRange1BackColor = Color.FromArgb(0, 90, 175);
            MainLineRange2BackColor = Color.FromArgb(48, 49, 52);

            MainLineLength = 3;
            ButtonOutterSpace2 = 12;
            BackColor = Color.FromArgb(7, 7, 7);
            ButtonLength1 = ButtonLength2 = 10;
            ThumbButtonColorTable.BackColorNormal = Color.FromArgb(152, 152, 152);
            ThumbButtonColorTable.BackColorHover = Color.FromArgb(192, 192, 192);
            ThumbButtonColorTable.BackColorPressed = Color.FromArgb(172, 172, 172);
        }
    }
}
