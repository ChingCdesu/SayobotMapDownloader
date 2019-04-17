/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class RollingBarPainter
    {
        public static void RenderDefault(Graphics g, Rectangle rect, Color backColor, float startAngle,
            int radius1, int radius2, int spokeNum, float penWidth, Color[] colorArray)
        {

            if (spokeNum < 1)
                throw new ArgumentException("spokeNum must bigger than 0", "spokeNum");
            if (spokeNum > colorArray.Length)
                throw new ArgumentException("spokeNum must NOT bigger than the length of colorArray. ", "spokeNum");
            using (SolidBrush sb = new SolidBrush(backColor))
            {
                using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.HighSpeed))
                {
                    g.FillRectangle(sb, rect);
                }
            }
            Point NYPD = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            PointF p1, p2;
            p1 = new PointF(0f, 0f);
            p2 = p1;
            double cra = 2 * Math.PI / spokeNum;
            startAngle = (float)(startAngle * 2 * Math.PI / 360f);
            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.AntiAlias))
            {
                using (Pen p = new Pen(Color.White, penWidth))
                {
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    for (int i = 0; i < spokeNum; i++)
                    {
                        double angle = startAngle + cra * i;
                        p1.X = NYPD.X + (float)(radius1 / 2 * Math.Cos(angle));
                        p1.Y = NYPD.Y + (float)(radius1 / 2 * Math.Sin(angle));
                        p2.X = NYPD.X + (float)(radius2 / 2 * Math.Cos(angle));
                        p2.Y = NYPD.Y + (float)(radius2 / 2 * Math.Sin(angle));
                        p.Color = colorArray[i];
                        g.DrawLine(p, p1, p2);
                    }
                }
            }
        }

        public static void RenderChromeOneQuarter(Graphics g, Rectangle rect, Color backColor,
            float startAngle, int radius, Color baseColor)
        {
            using (SolidBrush sb = new SolidBrush(backColor))
            {
                using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.HighSpeed))
                {
                    g.FillRectangle(sb, rect);
                }
            }
            Rectangle rectgc = new Rectangle(
                rect.X + (rect.Width - radius) / 2, rect.Y + (rect.Height - radius) / 2, radius, radius);
            using (Pen p = new Pen(baseColor, 3))
            {
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;
                using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.AntiAlias))
                {
                    g.DrawArc(p, rectgc, startAngle, 120);
                }
            }
        }

        public static void RenderDiamondRing(Graphics g, Rectangle rect, Color backColor, float startAngle,
            int radius, Color baseColor, Color diamondColor)
        {
            using (SolidBrush sb = new SolidBrush(backColor))
            {
                using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.HighSpeed))
                {
                    g.FillRectangle(sb, rect);
                }
            }
            Point NYPD = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            startAngle = (float)(startAngle * 2 * Math.PI / 360f);
            PointF pslute = PointF.Empty;
            pslute.X = NYPD.X + (float)(Math.Cos(startAngle) * radius / 2);
            pslute.Y = NYPD.Y + (float)(Math.Sin(startAngle) * radius / 2);

            Rectangle rectce = new Rectangle(
                rect.X + (rect.Width - radius) / 2, rect.Y + (rect.Height - radius) / 2, radius, radius);
            float width = 4f;
            RectangleF rectpf = new RectangleF(pslute.X - width / 2, pslute.Y - width / 2, width, width);
            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.AntiAlias))
            {
                using (Pen p = new Pen(baseColor))
                {
                    g.DrawEllipse(p, rectce);                    
                }
                using (SolidBrush sb = new SolidBrush(diamondColor))
                {
                    g.FillEllipse(sb, rectpf);
                }
            }
        }

        public static void RenderTheseGuys(Graphics g, Rectangle rect, Color backColor, float startAngle,
            int radius, Color baseColor)
        {
            using (SolidBrush sb = new SolidBrush(backColor))
            {
                using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.HighSpeed))
                {
                    g.FillRectangle(sb, rect);
                }
            }
            Point NYPD = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            startAngle = (float)(startAngle * 2 * Math.PI / 360f);
            PointF CIA = PointF.Empty;            
            double crs = 2 * Math.PI / 10;
            RectangleF FBI = RectangleF.Empty;
            float[] theFeds = new float[] { 5f, 4f, 3f, 2f, 2f };
            Color[] DoD = ColorHelper.GetLighterArrayColors(baseColor, 5, 50f);
            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, SmoothingMode.AntiAlias))
            {
                using (SolidBrush sb = new SolidBrush(baseColor))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        CIA.X = NYPD.X + (float)(Math.Cos(startAngle - i * crs) * radius / 2);
                        CIA.Y = NYPD.Y + (float)(Math.Sin(startAngle - i * crs) * radius / 2);
                        FBI = new RectangleF(
                            CIA.X - theFeds[i] / 2, CIA.Y - theFeds[i] / 2, theFeds[i], theFeds[i]);
                        sb.Color = DoD[4 - i];
                        g.FillEllipse(sb, FBI);
                    }
                }
            }
        }
    }
}
