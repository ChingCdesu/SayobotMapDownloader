using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    /// <summary>
    /// SimpleObject指的是只有一种状态（不像普通按钮一样有三种状态），并且其上的元素
    /// 布局比较简单的Object
    /// </summary>
    public class SimpleObjectPainter
    {
        public static void RenderCircleProgressBar(Graphics g, Rectangle rect, Color coveredColor, Color borderColor,
            Color backColor, bool drawInnerBorder, int startAngle, int percentage, bool drawText, Font textFont)
        {
            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.AntiAlias))
            {                
                Rectangle backup = rect;
                rect.Width--;
                rect.Height--;

                if (percentage < 0)
                    percentage = 0;
                if (percentage > 360)
                    percentage = 360;

                SolidBrush brushBack = new SolidBrush(backColor);
                Pen penBorder = new Pen(borderColor);

                // fill background                
                g.FillEllipse(brushBack, rect);                

                // outter most circle                
                g.DrawEllipse(penBorder, rect);                

                // pie covered region
                rect.Inflate(-1, -1);
                if (drawInnerBorder)
                    rect.Inflate(-1, -1);
                using (SolidBrush sb = new SolidBrush(coveredColor))
                {
                    g.FillPie(sb, rect, startAngle, 360 * percentage / 100);
                }

                // inner circle background
                rect = backup;
                rect.Inflate(-rect.Width / 4, -rect.Width / 4);                
                g.FillEllipse(brushBack, rect);                

                // inner circle line
                rect.Inflate(-1, -1);
                if (drawInnerBorder)
                    rect.Inflate(-1, -1);                
                g.DrawEllipse(penBorder, rect);                

                // text
                if (drawText)
                {
                    string text = percentage.ToString() + "%";
                    TextRenderer.DrawText(g, text, textFont, backup, Color.Black,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }

                brushBack.Dispose();
                penBorder.Dispose();
            }
        }
    }
}
