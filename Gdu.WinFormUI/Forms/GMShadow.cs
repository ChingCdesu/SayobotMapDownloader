/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Gdu.WinFormUI.Win32;
using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    internal partial class GMShadow : Form
    {
        private GMForm _owner;
        private bool _timeToResizeOwner;

        public GMShadow(GMForm owner)
        {
            _owner = owner;
            _timeToResizeOwner = false;
            InitializeComponent();
            FormIni();
        }

        public void BeginToResizeOwner()
        {
            _timeToResizeOwner = true;
        }

        private void FormIni()
        {
            base.FormBorderStyle = FormBorderStyle.None;
            base.WindowState = FormWindowState.Normal;
            base.ShowInTaskbar = false;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer, true);            
            UpdateStyles();
            base.AutoScaleMode = AutoScaleMode.None;
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WinAPI.WindowStyleEx.WS_EX_LAYERED;                
                return cp;
            }
        }

        private void SetFormReion()
        {
            if (base.Region != null)
                base.Region.Dispose();

            using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                    new Rectangle(Point.Empty, base.Size), 16, RoundStyle.All, false))
            {                
                this.Region = new Region(path);
            }
        }

        private void SetOwnerSize()
        {
            var size = base.Size;
            size.Width -= ShadowWidth * 2;
            size.Height -= ShadowWidth * 2;
            if (_owner.Size != size)
            {
                _owner.Size = size;
                _owner.Update();
            }
        }

        private void SetOwnerLocation()
        {
            Point location = base.Location;
            location.Offset(ShadowWidth, ShadowWidth);
            if (_owner.Location != location)
            {
                _owner.Location = location;
                _owner.Update();
            }
        }

        #region properties from owner
        
        private int ShadowWidth
        {
            get { return _owner.XTheme.ShadowWidth; }
        }
       
        private bool UseShadowToResize
        {
            get { return _owner.XTheme.UseShadowToResize; }
        }

        private Color ShadowColor
        {
            get { return _owner.XTheme.ShadowColor; }
        }

        private int ShadowAValueDark
        {
            get { return _owner.XTheme.ShadowAValueDark; }
        }

        private int ShadowAValueLight
        {
            get { return _owner.XTheme.ShadowAValueLight; }
        }


        #endregion

        #region resize rect region

        private Rectangle TopLeftRect
        {
            get
            {
                return new Rectangle(0, 0, ShadowWidth, ShadowWidth);
            }
        }

        private Rectangle TopRect
        {
            get
            {
                return new Rectangle(
                    ShadowWidth,
                    0,
                    this.Size.Width - ShadowWidth * 2,
                    ShadowWidth);
            }
        }

        private Rectangle TopRightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - ShadowWidth,
                    0,
                    ShadowWidth,
                    ShadowWidth);
            }
        }

        private Rectangle LeftRect
        {
            get
            {
                return new Rectangle(
                    0,
                    ShadowWidth,
                    ShadowWidth,
                    this.Size.Height - ShadowWidth * 2);
            }
        }

        private Rectangle RightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - ShadowWidth,
                    ShadowWidth,
                    ShadowWidth,
                    this.Size.Height - ShadowWidth * 2);
            }
        }

        private Rectangle BottomLeftRect
        {
            get
            {
                return new Rectangle(
                    0,
                    this.Size.Height - ShadowWidth,
                    ShadowWidth,
                    ShadowWidth);
            }
        }

        private Rectangle BottomRect
        {
            get
            {
                return new Rectangle(
                    ShadowWidth,
                    this.Size.Height - ShadowWidth,
                    this.Size.Width - ShadowWidth * 2,
                    ShadowWidth);
            }
        }

        private Rectangle BottomRightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - ShadowWidth,
                    this.Size.Height - ShadowWidth,
                    ShadowWidth,
                    ShadowWidth);
            }
        }

        #endregion

        private void DrawShadow(Graphics g)
        {
            int aDark = ShadowAValueDark;
            int aLight = ShadowAValueLight;
            if (aDark < 0)
                aDark = 0;
            if (aDark > 180)
                aDark = 180;
            if (aLight < 0)
                aLight = 0;
            if (aLight > 180)
                aLight = 180;
            if (aLight > aDark)
            {
                int t = aLight;
                aLight = aDark;
                aDark = t;
            }
            
            int diff = (aDark - aLight) / ShadowWidth;
            Rectangle rect = base.ClientRectangle;
            rect.Width--;
            rect.Height--;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < ShadowWidth; i++)
            {
                using (Pen p = new Pen(Color.FromArgb(aLight + diff * i, ShadowColor)))
                {
                    g.DrawRectangle(p, rect);
                }
                rect.Inflate(-1, -1);
            }
        }

        internal void RefleshLayeredForm()
        {
            Bitmap formBitMap = new Bitmap(base.Width, base.Height);
            Graphics g = Graphics.FromImage(formBitMap);

            DrawShadow(g);

            WinAPI.POINT ptSrc = new WinAPI.POINT(0, 0);
            WinAPI.POINT ptWinPos = new WinAPI.POINT(base.Left, base.Top);
            WinAPI.SIZE szWinSize = new WinAPI.SIZE(Width, Height);
            byte biAlpha = 0xFF;
            WinAPI.BLENDFUNCTION stBlend = new WinAPI.BLENDFUNCTION(
                (byte)WinAPI.BlendOp.AC_SRC_OVER, 0, biAlpha, (byte)WinAPI.BlendOp.AC_SRC_ALPHA);

            IntPtr gdiBitMap = IntPtr.Zero;
            IntPtr memoryDC = IntPtr.Zero;
            IntPtr preBits = IntPtr.Zero;
            IntPtr screenDC = IntPtr.Zero;

            try
            {
                screenDC = WinAPI.GetDC(IntPtr.Zero);
                memoryDC = WinAPI.CreateCompatibleDC(screenDC);
                
                gdiBitMap = formBitMap.GetHbitmap(Color.FromArgb(0));

                preBits = WinAPI.SelectObject(memoryDC, gdiBitMap);
                WinAPI.UpdateLayeredWindow(base.Handle
                    , screenDC
                    , ref ptWinPos
                    , ref szWinSize
                    , memoryDC
                    , ref ptSrc
                    , 0
                    , ref stBlend
                    , (uint)WinAPI.ULWPara.ULW_ALPHA);
            }
            finally
            {
                if (gdiBitMap != IntPtr.Zero)
                {
                    WinAPI.SelectObject(memoryDC, preBits);
                    WinAPI.DeleteObject(gdiBitMap);
                }

                WinAPI.DeleteDC(memoryDC);
                WinAPI.ReleaseDC(IntPtr.Zero, screenDC);
                g.Dispose();
                formBitMap.Dispose();
            }
        }

        private void WmNcHitTest(ref Message m)
        {
            int para = m.LParam.ToInt32();
            int x0 = WinAPI.LOWORD(para);
            int y0 = WinAPI.HIWORD(para);
            Point p = PointToClient(new Point(x0, y0));

            if (UseShadowToResize)
            {
                if (TopLeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPLEFT);
                    return ;
                }

                if (TopRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOP);
                    return ;
                }

                if (TopRightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPRIGHT);
                    return ;
                }

                if (LeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTLEFT);
                    return ;
                }

                if (RightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTRIGHT);
                    return ;
                }

                if (BottomLeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMLEFT);
                    return ;
                }

                if (BottomRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOM);
                    return ;
                }

                if (BottomRightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMRIGHT);
                    return ;
                }
            }
            
            m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
            return ;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WinAPI.WindowMessages.WM_NCHITTEST)
            {
                WmNcHitTest(ref m);
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RefleshLayeredForm();
            SetFormReion();
            if (UseShadowToResize && _timeToResizeOwner && _owner.WindowState == FormWindowState.Normal)
                SetOwnerSize();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (UseShadowToResize && _timeToResizeOwner && _owner.WindowState == FormWindowState.Normal)
                SetOwnerLocation();
        }
    }
}
