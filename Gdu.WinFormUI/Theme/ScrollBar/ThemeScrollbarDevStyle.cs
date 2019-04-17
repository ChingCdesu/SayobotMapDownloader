using System;
using System.Drawing;
using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeScrollbarDevStyle : ThemeScrollbarVS2013
    {
        public ThemeScrollbarDevStyle()
        {
            BestUndirectLen = 17;
            BackColor = Color.FromArgb(245, 245, 247);
            MiddleButtonOutterSpace2 = 5;
            SideButtonLength = 17;
            InnerPaddingWidth = 0;
            SideButtonForePathGetter = new ButtonForePathGetter(
                GraphicsPathHelper.Create7x4In7x7DownTriangleFlag);
            SideButtonForePathSize = new Size(7, 7);

            SideButtonColorTable.ForeColorNormal = Color.FromArgb(128, 131, 143);
            SideButtonColorTable.ForeColorHover = Color.FromArgb(32, 31, 53);
            SideButtonColorTable.ForeColorPressed = Color.Black;

            MiddleButtonColorTable.BorderColorNormal = MiddleButtonColorTable.BorderColorHover = 
                MiddleButtonColorTable.BorderColorPressed = Color.FromArgb(169, 172, 181);
            MiddleButtonColorTable.BackColorNormal = Color.FromArgb(217, 218, 223);
            MiddleButtonColorTable.BackColorHover = Color.FromArgb(213, 224, 252);
            MiddleButtonColorTable.BackColorPressed = Color.FromArgb(202, 203, 205);

            MiddleButtonRadius = 6;
        }
    }
}
