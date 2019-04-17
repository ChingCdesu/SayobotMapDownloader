using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mapdownloader
{
    public partial class Filter : mapdownloader.BaseForm
    {
        public Filter()
        {
            InitializeComponent();
            FilterCheckBox.SearchType = skinPanel1.Controls;
            FilterCheckBox.Mode = skinPanel2.Controls;
            FilterCheckBox.MapStatus = skinPanel3.Controls;
            FilterCheckBox.Genre = skinPanel4.Controls;
            FilterCheckBox.Language = skinPanel5.Controls;
            StaticUtilFunctions.SetFormMid(this);
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            this.Hide();
            PublicControlers.SearchBtn.Text = "搜索";
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.Transparent;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            panel2.BackColor = Color.FromArgb(128, 180, 180, 180);
        }
    }
    public static class FilterCheckBox
    {
        public static Control.ControlCollection SearchType;
        public static Control.ControlCollection Mode;
        public static Control.ControlCollection MapStatus;
        public static Control.ControlCollection Genre;
        public static Control.ControlCollection Language;
    }

}
