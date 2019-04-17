using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class GMProgessBarThemeBase : IDisposable
    {
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color InnerBorderColor { get; set; }
        public Color CoveredColor { get; set; }
        public Color LeadingEdgeColor { get; set; }

        public bool DrawInnerBorder { get; set; }
        public bool DrawLeadingEdge { get; set; }
        public bool DrawBackColorGlass { get; set; }
        public bool DrawCoveredColorGlass { get; set; }

        public int BorderRadius { get; set; }
        public Color ForeColor { get; set; }
        public Font ForeFont { get; set; }

        public GMProgessBarThemeBase()
        {
            BackColor = Color.White;
            InnerBorderColor = Color.White;
            BorderColor = Color.FromArgb(171, 171, 171);
            CoveredColor = Color.FromArgb(0, 114, 198);

            DrawInnerBorder = true;
            DrawLeadingEdge = false;
            DrawBackColorGlass = false;
            DrawCoveredColorGlass = false;

            BorderRadius = 0;
            ForeColor = Color.Black;
            ForeFont = new Font("微软雅黑", 9);
        }

        #region IDisposable

        public void Dispose()
        {
            if (ForeFont != null && !ForeFont.IsSystemFont)
                ForeFont.Dispose();
        }

        #endregion
    }
}
