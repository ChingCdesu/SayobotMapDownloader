using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeRollingBarCircleDots : GMRollingBarThemeBase
    {
        public ThemeRollingBarCircleDots()
        {
            Radius1 = 12;
            Radius2 = 14;
            PenWidth = 4f;
            SpokeNum = 9;

            BaseColor = Color.LightSeaGreen;
        }
    }
}
