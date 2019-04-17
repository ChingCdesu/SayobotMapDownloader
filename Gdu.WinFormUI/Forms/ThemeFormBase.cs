using System;
using System.Drawing;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    /// <summary>
    /// 该类封装设置一个主题所需的信息，可以继承该类实现不同的主题
    /// </summary>
    public class ThemeFormBase
    {
        #region about theme
        
        /// <summary>
        /// 主题名称，用于标识主题，对主题进行简单描述
        /// </summary>
        public string ThemeName { protected set; get; }

        #endregion

        #region public info

        // form shape about

        /// <summary>
        /// 窗体边界大小，鼠标移动到该边界将变成指针形状，拖动可改变窗体大小
        /// </summary>
        public int SideResizeWidth { get; set; }

        /// <summary>
        /// 窗体边框大小
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 窗体标题栏高度
        /// </summary>
        public int CaptionHeight { get; set; }

        /// <summary>
        /// 标题栏图标与窗体左边框的距离
        /// </summary>
        public int IconLeftMargin { get; set; }

        /// <summary>
        /// 标题栏图标尺寸
        /// </summary>
        public Size IconSize { get; set; }

        /// <summary>
        /// 标题栏文本左边距
        /// </summary>
        public int TextLeftMargin { get; set; }

        /// <summary>
        /// 控制按钮（关闭按钮）相对于窗体右上角的偏移量
        /// </summary>
        public Point ControlBoxOffset { get; set; }

        /// <summary>
        /// 窗体关闭按钮的尺寸
        /// </summary>
        public Size CloseBoxSize { get; set; }

        /// <summary>
        /// 窗体最大化/还原按钮的大小
        /// </summary>
        public Size MaxBoxSize { get; set; }

        /// <summary>
        /// 窗体最小化按钮的大小
        /// </summary>
        public Size MinBoxSize { get; set; }

        /// <summary>
        /// 关闭/最大化/最小化按钮之间的距离，可以设置成负值使它们重叠
        /// </summary>
        public int ControlBoxSpace { get; set; }

        /// <summary>
        /// 窗体圆角程度
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// 窗体圆角样式
        /// </summary>
        public RoundStyle RoundedStyle { get; set; }

        /// <summary>
        /// 是否使用默认的圆角样式，该样式为左上角和右上角圆角，
        /// 最好在窗体边框较粗时才使用该样式
        /// </summary>
        public bool UseDefaultTopRoundingFormRegion { get; set; }

        /// <summary>
        /// 是否在标题栏上绘制图标
        /// </summary>
        public bool DrawCaptionIcon { get; set; }

        /// <summary>
        /// 是否在标题栏上绘制文本
        /// </summary>
        public bool DrawCaptionText { get; set; }

        // form shadow

        /// <summary>
        /// 是否显示窗体阴影
        /// </summary>
        public bool ShowShadow { get; set; }

        /// <summary>
        /// 窗体阴影大小
        /// </summary>
        public int ShadowWidth { get; set; }

        /// <summary>
        /// 拖动窗体阴影是否可以改变窗体大小
        /// </summary>
        public bool UseShadowToResize { get; set; }

        /// <summary>
        /// 阴影的颜色，可以设置成其他颜色而不一定是黑色
        /// </summary>
        public Color ShadowColor { get; set; }

        /// <summary>
        /// 阴影从里到外是逐渐变浅的，这个值设置深色部分的透明度，ARGB中的A值
        /// </summary>
        public int ShadowAValueDark { get; set; }

        /// <summary>
        /// 阴影从里到外是逐渐变浅的，这个值设置浅色部分的透明度，ARGB中的A值
        /// </summary>
        public int ShadowAValueLight { get; set; }

        // form other
        
        /// <summary>
        /// 是否在客户区边界画线使其看起来有立体感
        /// </summary>
        public bool SetClientInset { get; set; }

        /// <summary>
        /// 窗体标题栏文字是否居中显示
        /// </summary>
        public bool CaptionTextCenter { get; set; }

        // form color

        /// <summary>
        /// 窗体边框最外一像素的颜色
        /// </summary>
        public Color FormBorderOutterColor { get; set; }

        /// <summary>
        /// 窗体边框第二最外像素的颜色
        /// </summary>
        public Color FormBorderInnerColor { get; set; }

        /// <summary>
        /// 窗体边框其他部分颜色，如果窗体边框大小大于2，则其他像素将用此颜色画出
        /// </summary>
        public Color FormBorderInmostColor { get; set; }

        /// <summary>
        /// 标题栏颜色是从上到下渐变的，这个值设置上边的颜色值
        /// </summary>
        public Color CaptionBackColorTop { get; set; }

        /// <summary>
        /// 标题栏颜色是从上到下渐变的，这个值设置下边的颜色值
        /// </summary>
        public Color CaptionBackColorBottom { get; set; }

        /// <summary>
        /// 标题栏文字颜色
        /// </summary>
        public Color CaptionTextColor { get; set; }

        /// <summary>
        /// 窗体背景颜色，该值将覆盖窗体自带的BackColor属性值
        /// </summary>
        public Color FormBackColor { get; set; }

        // control-box color table
        /// <summary>
        /// 窗体关闭按钮的颜色集合
        /// </summary>
        public ButtonColorTable CloseBoxColor { get; set; }     

        /// <summary>
        /// 窗体最大化/还原按钮的颜色集合
        /// </summary>
        public ButtonColorTable MaxBoxColor { get; set; }

        /// <summary>
        /// 窗体最小化按钮的颜色集合
        /// </summary>
        public ButtonColorTable MinBoxColor { get; set; }

        // control-box image
        public Image CloseBoxBackImageNormal { get; set; }
        public Image CloseBoxBackImageHover { get; set; }
        public Image CloseBoxBackImagePressed { get; set; }

        public Image MaxBoxBackImageNormal { get; set; }
        public Image MaxBoxBackImageHover { get; set; }
        public Image MaxBoxBackImagePressed { get; set; }

        public Image ResBoxBackImageNormal { get; set; }
        public Image ResBoxBackImageHover { get; set; }
        public Image ResBoxBackImagePressed { get; set; }

        public Image MinBoxBackImageNormal { get; set; }
        public Image MinBoxBackImageHover { get; set; }
        public Image MinBoxBackImagePressed { get; set; }

        #endregion

        #region mdi support

        // bar overall ---------------------------------------------------------------
        /// <summary>
        /// 是否用SendMessage的方式切换子窗体，以避免子窗体切换时产生的闪烁
        /// </summary>
        public bool Mdi_UseMsgToActivateChild { get; set; }

        /// <summary>
        /// Mdi-Bar与窗体左、上、右边界的距离，只用到这三个值，Bottom值没用到。
        /// 用这三个值来确定Mdi-Bar的宽度及其在窗体中的位置
        /// </summary>
        public Padding Mdi_BarMargin { get; set; }

        /// <summary>
        /// 内部左边空白，第一个标签将从这个空白距离之后开始
        /// </summary>
        public int Mdi_BarLeftPadding { get; set; }

        /// <summary>
        /// 内部右边空白
        /// </summary>
        public int Mdi_BarRightPadding { get; set; }
        public Color Mdi_BarBackColor { get; set; }
        public Color Mdi_BarBorderColor { get; set; }
        public bool Mdi_DrawBarBackground { get; set; }
        public bool Mdi_DrawBarBorder { get; set; }

        // bar bottom region--------------------------------------------------------------
        public Color Mdi_BarBottomRegionBackColor { get; set; }
        public int Mdi_BarBottomRegionHeight { get; set; }


        // tab ----------------------------------------------------------------------------
        /// <summary>
        /// 标签高度
        /// </summary>
        public int Mdi_TabHeight { get; set; }
        
        /// <summary>
        /// 被选中的标签高度，可以设置成与TabHeight不一样的值，以突出显示被选中状态
        /// </summary>
        public int Mdi_TabHeightActive { get; set; }

        /// <summary>
        /// 标签之间的距离，设成负值可以使标签有重叠的效果
        /// </summary>
        public int Mdi_TabAndTabSpace { get; set; }

        /// <summary>
        /// 标签的最大宽度，任何情况下标签都不能超过这个宽度
        /// </summary>
        public int Mdi_TabMaxWidth { get; set; }

        /// <summary>
        /// 标签正常宽度，如果标签需要很短的宽度（比如20像素）就可以显示完上面的文字，
        /// 但是Mdi-Bar上有足够的空间时，标签会以正常宽度（比如100像素）显示
        /// </summary>
        public int Mdi_TabNormalWidth { get; set; }

        /// <summary>
        /// 标签最小宽度，当标签小于这个宽度时将被隐藏
        /// </summary>
        public int Mdi_TabMinWidth { get; set; }

        /// <summary>
        /// 标签梯度大小，标签可以不以矩形方式显示，而是有一个梯度/斜度。
        /// </summary>
        public int Mdi_TabSlopeWidth { get; set; }

        /// <summary>
        /// 标签顶部空白，这个值用于参与计算Mdi-Bar高度，计算方式为：
        /// Mdi-Bar Height = BottomRegionHeight + TabHeight + TabTopSpace
        /// </summary>
        public int Mdi_TabTopSpace { get; set; }

        /// <summary>
        /// 标签上是否显示子窗体图标
        /// </summary>
        public bool Mdi_ShowTabIcon { get; set; }

        /// <summary>
        /// 选中状态的标签的上部背景色，与下部背景色不同时，标签背景色就有渐变效果
        /// </summary>
        public Color Mdi_TabActiveBackColorTop { get; set; }
        public Color Mdi_TabActiveBackColorBottom { get; set; }

        /// <summary>
        /// 非选中状态的标签的上部背景色
        /// </summary>
        public Color Mdi_TabDeactiveBackColorTop { get; set; }
        public Color Mdi_TabDeactiveBackColorBottom { get; set; }

        /// <summary>
        /// 标签外边框颜色
        /// </summary>
        public Color Mdi_TabOutterBorderColor { get; set; }

        /// <summary>
        /// 标签内边框颜色，这个颜色一般具有一定的透明度
        /// </summary>
        public Color Mdi_TabInnerBorderColor { get; set; }

        // new tab btn ---------------------------------------------------------------
        /// <summary>
        /// 是否显示默认的新建标签按钮(NewTabBtn)
        /// </summary>
        public bool Mdi_ShowNewTabBtn { get; set; }

        /// <summary>
        /// NewTabBtn与下边框的距离，这个值用来定位按钮的Y坐标
        /// </summary>
        public int Mdi_NewTabBtnBottomSpace { get; set; }
        public int Mdi_NewTabBtnLeftSpace { get; set; }
        public Size Mdi_NewTabBtnSize { get; set; }
        /// <summary>
        /// 按钮颜色集合
        /// </summary>
        public ButtonColorTable Mdi_NewTabBtnColor { get; set; }

        // list-all-btn       ----------------------------------------------------------
        /// <summary>
        /// 是否一直显示ListAllBtn，即使在没有标签被隐藏的情况下
        /// </summary>
        public bool Mdi_AlwaysShowListAllBtn { get; set; }
        public BarButtonAlignmentType Mdi_ListAllBtnAlign { get; set; }
        public int Mdi_ListAllBtnBottomSpace { get; set; }
        public int Mdi_ListAllBtnLeftSpace { get; set; }
        public ButtonColorTable Mdi_ListAllBtnColor { get; set; }
        public Size Mdi_ListAllBtnSize { get; set; }

        #endregion

        public ThemeFormBase()
        {
            // about theme
            ThemeName = "Base Default Theme";

            // form shape
            SideResizeWidth = 6;
            BorderWidth = 6;
            CaptionHeight = 28;
            IconLeftMargin = 2;
            IconSize = new Size(16, 16);
            TextLeftMargin = 2;
            ControlBoxOffset = new Point(8, 8);
            CloseBoxSize = new Size(37, 17);
            MaxBoxSize = new Size(25, 17);
            MinBoxSize = new Size(25, 17);
            ControlBoxSpace = 2;
            Radius = 8;
            RoundedStyle = RoundStyle.None;
            UseDefaultTopRoundingFormRegion = true;
            DrawCaptionIcon = true;
            DrawCaptionText = true;

            // form shadow
            ShowShadow = false;
            ShadowWidth = 6;
            ShadowColor = Color.Black;
            ShadowAValueDark = 80;
            ShadowAValueLight = 0;
            UseShadowToResize = false;

            // form color
            FormBorderInmostColor = FormBorderInnerColor = FormBorderOutterColor 
                = ColorHelper.GetLighterColor(Color.FromArgb(75, 159, 216), 10);
            CaptionBackColorBottom = CaptionBackColorTop 
                = ColorHelper.GetLighterColor(Color.FromArgb(75, 159, 216), 10);
            CaptionTextColor = Color.Black;
            FormBackColor = SystemColors.Control;

            // form other            
            SetClientInset = true;
            CaptionTextCenter = false;

            // control box color            
            CloseBoxColor = ButtonColorTable.GetDevWhiteThemeCloseBtnColor();            
            MaxBoxColor = ButtonColorTable.GetDevWhiteThemeMinMaxBtnColor();                        
            MinBoxColor = MaxBoxColor;

            #region mdi support

            // bar overall       
            Mdi_UseMsgToActivateChild = true;
            Mdi_BarMargin = new Padding(6, 38, 100, 0);
            Mdi_BarLeftPadding = 3;
            Mdi_BarRightPadding = 100;
            Mdi_BarBackColor = Color.LightSkyBlue;
            Mdi_BarBorderColor = Color.Red;
            Mdi_DrawBarBackground = false;
            Mdi_DrawBarBorder = false;

            // bar bottom region
            Mdi_BarBottomRegionBackColor = Color.White;
            Mdi_BarBottomRegionHeight = 3;

            // tab
            Mdi_TabHeight = 26;
            Mdi_TabHeightActive = Mdi_TabHeight;
            Mdi_TabSlopeWidth = 8;
            Mdi_TabAndTabSpace = -Mdi_TabSlopeWidth;
            Mdi_TabTopSpace = 2;
            Mdi_TabMaxWidth = 360;
            Mdi_TabNormalWidth = 180;
            Mdi_TabMinWidth = 90;
            Mdi_ShowTabIcon = true;

            Mdi_TabActiveBackColorTop = Color.White;
            Mdi_TabActiveBackColorBottom = Color.White;
            Mdi_TabDeactiveBackColorTop = Color.LightGray;
            Mdi_TabDeactiveBackColorBottom = Color.DarkGray;
            Mdi_TabOutterBorderColor = Color.Gray;
            Mdi_TabInnerBorderColor = Color.FromArgb(180, Color.White);

            // new tab btn
            Mdi_ShowNewTabBtn = true;
            Mdi_NewTabBtnBottomSpace = Mdi_BarBottomRegionHeight + 4;
            Mdi_NewTabBtnLeftSpace = 4;
            Mdi_NewTabBtnSize = new Size(25, 18);
            Mdi_NewTabBtnColor = ButtonColorTable.DefaultTable();

            // list all btn
            Mdi_AlwaysShowListAllBtn = true;
            Mdi_ListAllBtnAlign = BarButtonAlignmentType.Left;
            Mdi_ListAllBtnBottomSpace = Mdi_BarBottomRegionHeight + 4;
            Mdi_ListAllBtnLeftSpace = 4;
            Mdi_ListAllBtnSize = new Size(36, 18);
            Mdi_ListAllBtnColor = ButtonColorTable.DefaultTable();

            #endregion

        }
    }    
}
