using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Gdu.WinFormUI
{
    [ToolboxItem(true)]
    public class GMHScrollBar : GMScrollBarBase
    {

        #region 重写/实现属性

        protected override int ScrollBarLength
        {
            get
            {
                return base.Size.Width;
            }
        }

        //现在发现这个属性其实没有必要重写
        protected override int MiddleButtonBeginPositionDot
        {
            get
            {
                int sideBtnLen = ShowSideButtons ? base.SideButtonLength : 0;
                return base.InnerPaddingWidth + sideBtnLen +
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
                    height = base.Size.Height - InnerPaddingWidth * 2;
                    width = base.SideButtonLength;

                    int len = ScrollBarLength - InnerPaddingWidth * 2;
                    if (len < 0)
                        len = 0;
                    if (len < SideButtonLength * 2)
                        width = len / 2;
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
                return new Rectangle(Size.Width - InnerPaddingWidth - rect1.Width,
                    InnerPaddingWidth, rect1.Width, rect1.Height);
                    
            }
        }

        protected override Rectangle MiddleButtonRect
        {
            get
            {
                return new Rectangle(this.MiddleButtonCurrentPositionDot,
                    InnerPaddingWidth + MiddleButtonOutterSpace2,
                    base.MiddleButtonLength,
                    Size.Height - InnerPaddingWidth * 2 - MiddleButtonOutterSpace2 * 2
                    );
            }
        }

        protected override Rectangle BeforeMdlBtnRect
        {
            get
            {
                return new Rectangle(MiddleButtonBeginPositionDot,
                    InnerPaddingWidth,                    
                    MiddleButtonCurrentPositionDot - MiddleButtonBeginPositionDot,
                    Size.Height - InnerPaddingWidth * 2);
            }
        }

        protected override Rectangle AfterMdlBtnRect
        {
            get
            {
                int top = InnerPaddingWidth;
                int left = MiddleButtonRect.Bottom;
                int height = Size.Height - top * 2;
                int width = SideButton2Rect.Left - left - MiddleButtonOutterSpace1;
                if (!ShowSideButtons)
                    width += SideButtonLength;
                return new Rectangle(left, top, width, height);
            }
        }

        protected override ForePathRatoteDirection SideButton1RotateInfo
        {
            get 
            {
                return ForePathRatoteDirection.Left;
            }
        }

        protected override ForePathRatoteDirection SideButton2RotateInfo
        {
            get { return ForePathRatoteDirection.Right; }
        }

        protected override bool IsVerticalBar
        {
            get { return false; }
        }

        #endregion

        #region 构造函数

        public GMHScrollBar()
        {            
        }

        #endregion
        
    }
}
