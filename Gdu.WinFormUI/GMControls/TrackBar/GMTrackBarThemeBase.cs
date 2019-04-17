using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class GMTrackBarThemeBase
    {
        #region 尺寸及间距调节

        public int ButtonLength1 { get; set; }
        public int ButtonLength2 { get; set; }
        public int MainLineLength { get; set; }
        public int ButtonOutterSpace1 { get; set; }
        public int ButtonOutterSpace2 { get; set; }
        public int TickLineLength { get; set; }
        public int TickLineSpaceWithButton { get; set; }
        public int TickLineSpaceWithBorder { get; set; }

        #endregion

        #region 边框及颜色设置

        public int BorderWidth { get; set; }
        public bool DrawBackground { get; set; }
        public bool DrawBorder { get; set; }
        public bool DrawInnerBorder { get; set; }
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color InnerBorderColor { get; set; }        
        public Color TickLineColor { get; set; }

        #endregion 

        #region extra

        public ButtonColorTable ThumbButtonColorTable { get; set; }
        public ButtonBorderType ThumbButtonBorderType { get; set; }

        public bool MainLineDrawBorder { get; set; }
        public Color MainLineBorderColor { get; set; }
        public int MainLineRadius { get; set; }
        public Color MainLineRange1BackColor { get; set; }
        public Color MainLineRange2BackColor { get; set; }

        #endregion

        public GMTrackBarThemeBase()
        {
            ButtonLength1 = 8;
            ButtonLength2 = 18;
            MainLineLength = 4;
            ButtonOutterSpace1 = 4;
            ButtonOutterSpace2 = 2;
            TickLineLength = 3;
            TickLineSpaceWithButton = 2;
            TickLineSpaceWithBorder = 6;

            BorderWidth = 1;
            DrawBackground = false;
            DrawBorder = false;
            DrawInnerBorder = false;
            BackColor = Color.Transparent;
            TickLineColor = Color.FromArgb(185, 185, 185);

            MainLineDrawBorder = true;
            MainLineBorderColor = Color.FromArgb(0, 114, 198);
            MainLineRadius = 0;
            MainLineRange1BackColor = MainLineRange2BackColor = Color.White;

        }
    }
}
