/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Gdu.WinFormUI.MyGraphics
{

    /// <summary>
    /// 提供产生各种路径的静态方法，比如圆角路径、关闭按钮上的x路径、+号路径
    /// </summary>
    public static class GraphicsPathHelper
    {
        public static GraphicsPath CreateRoundedRect(Rectangle rect, int radius, RoundStyle type, bool shorten)
        {
            GraphicsPath path = new GraphicsPath();

            if (shorten)
            {
                rect.Width--;
                rect.Height--;
            }

            if (radius < 2)
                type = RoundStyle.None;

            Rectangle rectTopLeft = new Rectangle(rect.X, rect.Y, radius, radius);
            Rectangle rectTopRight = new Rectangle(rect.Right - radius, rect.Y, radius, radius);
            Rectangle rectBottomLeft = new Rectangle(rect.X, rect.Bottom - radius, radius, radius);
            Rectangle rectBottomRight = new Rectangle(rect.Right - radius, rect.Bottom - radius, radius, radius);
            Point p1 = new Point(rect.X, rect.Y);
            Point p2 = new Point(rect.Right, rect.Y);
            Point p3 = new Point(rect.Right, rect.Bottom);
            Point p4 = new Point(rect.X, rect.Bottom);

            switch (type)
            {
                case RoundStyle.None:
                    path.AddRectangle(rect);
                    break;
                case RoundStyle.All:
                    path.AddArc(rectTopLeft, 180, 90);
                    path.AddArc(rectTopRight, 270, 90);
                    path.AddArc(rectBottomRight, 0, 90);
                    path.AddArc(rectBottomLeft, 90, 90);
                    break;
                case RoundStyle.Top:
                    path.AddArc(rectTopLeft, 180, 90);
                    path.AddArc(rectTopRight, 270, 90);
                    path.AddLine(p3, p4);
                    break;
                case RoundStyle.Bottom:
                    path.AddArc(rectBottomRight, 0, 90);
                    path.AddArc(rectBottomLeft, 90, 90);
                    path.AddLine(p1, p2);
                    break;
                case RoundStyle.Left:
                    path.AddArc(rectBottomLeft, 90, 90);
                    path.AddArc(rectTopLeft, 180, 90);
                    path.AddLine(p2, p3);
                    break;
                case RoundStyle.Right:
                    path.AddArc(rectTopRight, 270, 90);
                    path.AddArc(rectBottomRight, 0, 90);
                    path.AddLine(p4, p1);
                    break;
                default:
                    break;
            }
            path.CloseFigure();            
            return path;
        }

        public static GraphicsPath CreateMinimizeFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;
            Point p1 = new Point(x + 1, y + 5);
            Point p2 = new Point(x + 7, y + 5);
            Point p3 = new Point(x + 1, y + 6);
            Point p4 = new Point(x + 7, y + 6);
            path.AddLines(new Point[] { p1, p2, p3, p4 });            
            return path;
        }

        public static GraphicsPath CreateMaximizeFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;
            Point p1 = new Point(x + 1, y + 1);
            Point p2 = new Point(x + 7, y + 1);
            path.AddRectangle(new Rectangle(new Point(x, y), new Size(8, 6)));
            path.CloseFigure();
            path.AddLine(p1, p2);
            return path;
        }

        public static GraphicsPath CreateRestoreFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X + (rect.Width - 11) / 2;
            int y = rect.Y + (rect.Height - 9) / 2;

            Point p1 = new Point(x, y + 3);
            Point p2 = new Point(x + 6, y + 3);
            Point p3 = new Point(x + 6, y + 4);
            Point p4 = new Point(x + 6, y + 8);
            Point p5 = new Point(x, y + 8);
            Point p6 = new Point(x, y + 4);

            Point p7 = new Point(x + 7, y + 5);
            Point p8 = new Point(x + 9, y + 5);
            Point p9 = new Point(x + 9, y + 1);
            Point p10 = new Point(x + 3, y + 1);
            Point p11 = new Point(x + 3, y + 2);
            Point p12 = new Point(x + 3, y);
            Point p13 = new Point(x + 9, y);

            path.AddLines(new Point[] { p1, p2, p4, p5, p6, p3, p2, p1 });
            path.CloseFigure();

            path.AddLines(new Point[] { p7, p8, p9, p10, p11, p12, p13, p8, p7 });
            return path;
        }

        public static GraphicsPath CreateCloseFlagPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - 9) / 2;
            int y = rect.Y + (rect.Height - 7) / 2;

            Point p1 = new Point(x + 1-1, y-1);
            Point p2 = new Point(x + 7, y + 6);
            Point p3 = new Point(x + 8, y + 6);
            Point p4 = new Point(x + 2-1, y-1);

            Point p5 = new Point(x + 6+1, y-1);
            Point p6 = new Point(x, y + 6);
            Point p7 = new Point(x + 1, y + 6);
            Point p8 = new Point(x + 7+1, y-1);

            path.AddLine(p1, p2);
            path.AddLine(p3, p4);
            path.CloseFigure();
            path.AddLine(p5, p6);
            path.AddLine(p7, p8);

            return path;
        }

        public static GraphicsPath CreateTopRoundedPathForFormRegion(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();

            Point pBL = new Point(rect.X, rect.Bottom); // bottom left
            Point pBR = new Point(rect.Right, rect.Bottom); // bottom right

            int x = rect.X, y = rect.Y, r = rect.Right;
            Point p1 = new Point(x, y + 4);
            Point p2 = new Point(x + 1, y + 4);
            Point p3 = new Point(x + 1, y + 2);
            Point p4 = new Point(x + 2, y + 2);
            Point p5 = new Point(x + 2, y + 1);
            Point p6 = new Point(x + 4, y + 1);
            Point p7 = new Point(x + 4, y);

            Point p8 = new Point(r - 4, y);
            Point p9 = new Point(r - 4, y + 1);
            Point p10 = new Point(r - 2, y + 1);
            Point p11 = new Point(r - 2, y + 2);
            Point p12 = new Point(r - 1, y + 2);
            Point p13 = new Point(r - 1, y + 4);
            Point p14 = new Point(r, y + 4);

            path.AddLines(new Point[] { p1, p2, p3, p4, p5, p6, p7 });
            path.AddLines(new Point[] { p8, p9, p10, p11, p12, p13, p14 });
            path.AddLine(pBR, pBL);

            path.CloseFigure();
            return path;
        }

        public static GraphicsPath CreateSingleLineCloseFlag(Rectangle rect, int width)
        {
            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - width) / 2;
            int y = rect.Y + (rect.Height - width) / 2;

            Point p1 = new Point(x, y);
            Point p2 = new Point(x + width, y);
            Point p3 = new Point(x + width, y + width);
            Point p4 = new Point(x, y + width);

            path.AddLine(p1, p3);
            path.CloseFigure();
            path.AddLine(p2, p4);

            return path;
        }

        public static GraphicsPath CreateSingleLineCloseFlag(Rectangle rect)
        {
            return CreateSingleLineCloseFlag(rect, 6);
        }

        public static GraphicsPath CreatePlusFlag(Rectangle rect, int width)
        {
            GraphicsPath path = new GraphicsPath();

            if (width % 2 == 1)
                width++;

            int x = rect.X + (rect.Width - width) / 2;
            int y = rect.Y + (rect.Height - width) / 2;

            Point p1 = new Point(x + width / 2 - 1, y);
            Point p2 = new Point(x + width / 2 - 1, y + width - 1);
            Point p3 = new Point(x + width / 2, y + width - 1);
            Point p4 = new Point(x + width / 2, y);

            Point p5 = new Point(x, y + width / 2 - 1);
            Point p6 = new Point(x + width - 1, y + width / 2 - 1);
            Point p7 = new Point(x + width - 1, y + width / 2);
            Point p8 = new Point(x, y + width / 2);

            path.AddLines(new Point[] { p1, p2, p3, p4 });
            path.CloseFigure();
            path.AddLines(new Point[] { p5, p6, p7, p8 });

            return path;
        }

        public static GraphicsPath CreatePlusFlag(Rectangle rect)
        {
            return CreatePlusFlag(rect, 10);
        }

        public static GraphicsPath CreateDownTriangleFlag(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - 10) / 2;
            int y = rect.Y + (rect.Height - 9) / 2;

            if (rect.Height % 2 == 0)
                y++;

            Point p1 = new Point(x, y);
            Point p2 = new Point(x + 9, y);
            Point p3 = new Point(x + 9, y + 1);
            Point p4 = new Point(x, y + 1);

            path.AddLines(new Point[] { p1, p2, p3, p4 });
            path.CloseFigure();

            int x1 = x, y1 = y + 4, x2 = x + 9, y2 = y + 4;
            for (int i = 1; i <= 5; i++)
            {
                if (i % 2 == 0)
                    path.AddLine(x2, y2, x1, y1);
                else
                    path.AddLine(x1, y1, x2, y2);
                x1++;
                x2--;
                y1++;
                y2++;
            }

            return path;
        }

        public static GraphicsPath Create7x4DownTriangleFlag(Rectangle rect)
        {
            return CreateWxHDownTriangleFlag(rect, 7, 4);
        }

        public static GraphicsPath Create9x5DownTriangleFlag(Rectangle rect)
        {
            return CreateWxHDownTriangleFlag(rect, 9, 5);
        }

        private static GraphicsPath CreateWxHDownTriangleFlag(Rectangle rect, int w, int h)
        {
            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - w) / 2;
            int y = rect.Y + (rect.Height - h) / 2;

            int x1 = x, x2 = x + w - 1;

            for (int i = 0; i < h; i++)
            {
                if (i % 2 == 0)
                    path.AddLine(x1, y, x2, y);
                else
                    path.AddLine(x2, y, x1, y);
                x1++;
                x2--;
                y++;
            }
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath Create7x4In7x7DownTriangleFlag(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();

            int x = rect.X + (rect.Width - 7) / 2;
            int y = rect.Y + (rect.Height - 7) / 2 + 2;

            int x1 = x, x2 = x + 6;

            for (int i = 0; i < 4; i++)
            {
                if (i % 2 == 0)
                    path.AddLine(x1, y, x2, y);
                else
                    path.AddLine(x2, y, x1, y);
                x1++;
                x2--;
                y++;
            }
            path.CloseFigure();
            return path;
        }
    }
}
