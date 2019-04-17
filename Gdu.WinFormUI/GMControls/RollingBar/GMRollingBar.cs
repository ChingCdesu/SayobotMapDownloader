/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    [ToolboxItem(true)]
    public class GMRollingBar : GMBarControlBase, IGMControl
    {

        #region 构造函数及初始化

        public GMRollingBar()
        {            
            rollingTimer = new Timer();
            rollingTimer.Tick += new EventHandler(rollingTimer_Tick);
            rollingTimer.Interval = _refleshFrequency;
            rollingTimer.Enabled = false;
        }        

        #endregion

        #region private var

        float currentAngle = 0f;

        Timer rollingTimer;

        #endregion

        #region IGMControl实现

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public GMControlType ControlType
        {
            get { return GMControlType.RollingBar; }
        }

        #endregion

        #region 新增的公开属性

        GMRollingBarThemeBase _xtheme;
        RollingBarStyle _style;
        int _refleshFrequency = 150;

        [DefaultValue(150)]
        public int RefleshFrequency
        {
            get
            {
                return _refleshFrequency;
            }
            set
            {
                if (_refleshFrequency != value)
                {
                    if (value < 1)
                        value = 150;
                    _refleshFrequency = value;
                    rollingTimer.Interval = _refleshFrequency;
                }
            }
        }

        [DefaultValue(typeof(RollingBarStyle),"0")]
        public RollingBarStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public GMRollingBarThemeBase XTheme
        {
            get
            {
                return _xtheme;
            }
            set
            {
                _xtheme = value;
                Invalidate();
            }
        }

        #endregion

        #region 公开的方法

        public void StartRolling()
        {
            if (rollingTimer.Enabled)
                return;            
            rollingTimer.Enabled = true;
        }

        public void StopRolling()
        {
            rollingTimer.Enabled = false;            
        }

        #endregion

        #region 可用XTheme配置的属性

        protected virtual int Radius1
        {
            get
            {
                if (_xtheme == null)
                    return 10;
                else
                    return _xtheme.Radius1;
            }
        }

        protected virtual int Radius2
        {
            get
            {
                if (_xtheme == null)
                    return 24;
                else
                    return _xtheme.Radius2;
            }
        }

        protected virtual int SpokeNum
        {
            get
            {
                if (_xtheme == null)
                    return 12;
                else
                    return _xtheme.SpokeNum;
            }
        }

        protected virtual float PenWidth
        {
            get
            {
                if (_xtheme == null)
                    return 2;
                else
                    return _xtheme.PenWidth;
            }
        }

        protected virtual Color BaseColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Red;
                else
                    return _xtheme.BaseColor;
            }
        }

        protected virtual Color GMBackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Transparent;
                else
                    return _xtheme.BackColor;
            }
        }

        #endregion

        #region 内部方法

        private void rollingTimer_Tick(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        #endregion

        #region 内部绘图

        protected virtual void PaintThisRollingBar(Graphics g)
        {
            switch (Style)
            {
                case RollingBarStyle.Default:
                    PaintDefault(g);
                    break;

                case RollingBarStyle.ChromeOneQuarter:
                    PaintChromeOneQuarter(g);
                    break;

                case RollingBarStyle.DiamondRing:
                    PaintDiamondRing(g);
                    break;

                case RollingBarStyle.BigGuyLeadsLittleGuys:
                    PaintTheseGuys(g);
                    break;
            }
        }

        private void IncreaseCurrentAngle(int spokeNum)
        {
            if (rollingTimer.Enabled)
            {
                currentAngle += 360f / spokeNum;
                if (currentAngle > 360f)
                    currentAngle -= 360f;
            }
        }

        private void PaintDefault(Graphics g)
        {
            IncreaseCurrentAngle(SpokeNum);

            RollingBarPainter.RenderDefault(
                g,
                ClientRectangle,
                GMBackColor,
                currentAngle,
                Radius1,
                Radius2,
                SpokeNum,
                PenWidth,
                ColorHelper.GetLighterArrayColors(BaseColor, SpokeNum));
        }

        private void PaintChromeOneQuarter(Graphics g)
        {
            IncreaseCurrentAngle(10);

            RollingBarPainter.RenderChromeOneQuarter(
                g,
                ClientRectangle,
                GMBackColor,
                currentAngle,
                Radius1,
                BaseColor);
        }

        private void PaintDiamondRing(Graphics g)
        {
            IncreaseCurrentAngle(12);

            RollingBarPainter.RenderDiamondRing(
                g,
                ClientRectangle,
                GMBackColor,
                currentAngle,
                Radius1,
                BaseColor,
                _xtheme == null ? Color.White : _xtheme.DiamondColor);
        }

        private void PaintTheseGuys(Graphics g)
        {
            IncreaseCurrentAngle(10);

            RollingBarPainter.RenderTheseGuys(
                g,
                ClientRectangle,
                GMBackColor,
                currentAngle,
                Radius1,
                BaseColor);
        }

        #endregion

        #region 重写基类方法

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PaintThisRollingBar(e.Graphics);
        }

        #endregion

    }
}
