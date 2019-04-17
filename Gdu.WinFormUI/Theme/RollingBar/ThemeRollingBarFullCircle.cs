using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeRollingBarFullCircle : GMRollingBarThemeBase
    {
        public ThemeRollingBarFullCircle()
        {
            SpokeNum = 36;
            Radius1 = 16;
            Radius2 = 18;
            PenWidth = 4;

            BaseColor = Color.DarkGray;
        }
    }
}
