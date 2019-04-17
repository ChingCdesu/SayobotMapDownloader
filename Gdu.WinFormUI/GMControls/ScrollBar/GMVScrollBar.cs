using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Gdu.WinFormUI
{
    [ToolboxItem(true)]
    public class GMVScrollBar : GMScrollBarBase
    {
        #region private var

        #endregion

        #region 重写/实现属性

        protected override int ScrollBarLength
        {
            get
            {
                return base.Size.Height;
            }
        }

        protected override int MiddleButtonBeginPositionDot
        {
            get
            {
                int sizeBtnLen = ShowSideButtons ? base.SideButtonLength : 0;
                return base.InnerPaddingWidth + sizeBtnLen +
                    base.MiddleButtonOutterSpace1;
            }
        }

        protected override Rectangle SideButton1Rect
        {
            get
            {
                int width, height;
                if (ShowSideButtons)
                {
                    width = base.Size.Width - InnerPaddingWidth * 2;
                    height = base.SideButtonLength;

                    int len = ScrollBarLength - InnerPaddingWidth * 2;
                    if (len < 0)
                        len = 0;
                    if (len < SideButtonLength * 2)
                        height = len / 2;
                }
                else
                {
                    width = height = 0;
                }
                return new Rectangle(base.InnerPaddingWidth,
                    base.InnerPaddingWidth, width, height);
            }
        }

        protected override Rectangle SideButton2Rect
        {
            get
            {
                Rectangle rect1 = SideButton1Rect;
                return new Rectangle(InnerPaddingWidth,
                    Size.Height - InnerPaddingWidth - rect1.Height,
                    rect1.Width, rect1.Height);
                    
            }
        }

        protected override Rectangle MiddleButtonRect
        {
            get
            {
                return new Rectangle(InnerPaddingWidth + MiddleButtonOutterSpace2,
                    this.MiddleButtonCurrentPositionDot,
                    Size.Width - InnerPaddingWidth * 2 - MiddleButtonOutterSpace2 * 2,
                    base.MiddleButtonLength);
            }
        }

        protected override Rectangle BeforeMdlBtnRect
        {
            get
            {
                return new Rectangle(InnerPaddingWidth,
                    MiddleButtonBeginPositionDot,
                    Size.Width - InnerPaddingWidth * 2,
                    MiddleButtonCurrentPositionDot - MiddleButtonBeginPositionDot);
            }
        }

        protected override Rectangle AfterMdlBtnRect
        {
            get
            {
                int left = InnerPaddingWidth;
                int top = MiddleButtonRect.Bottom;
                int width = Size.Width - left * 2;
                int height = SideButton2Rect.Top - top - MiddleButtonOutterSpace1;
                if (!ShowSideButtons)
                    height += SideButtonLength;
                return new Rectangle(left, top, width, height);
            }
        }

        protected override ForePathRatoteDirection SideButton1RotateInfo
        {
            get 
            {
                return ForePathRatoteDirection.Up;
            }
        }

        protected override ForePathRatoteDirection SideButton2RotateInfo
        {
            get { return ForePathRatoteDirection.Down; }
        }

        protected override bool IsVerticalBar
        {
            get { return true; }
        }

        #endregion

        #region 构造函数

        public GMVScrollBar()
        {            
        }

        #endregion
        
    }
}
