using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HIDInterface;


namespace mapdownloader
{
    public partial class PadDev : mapdownloader.BaseForm
    {
        public PadDev()
        {
            InitializeComponent();
        }
        static HIDDevice OpenedPad = UtilValues.PublicValue.nanoPads.Length != 0 ?
            new HIDDevice(UtilValues.PublicValue.nanoPads[0].devicePath, false) : null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (OpenedPad != null)
            {
                OpenedPad.write(PadDevUtil.StringToByte(skinTextBox1.Text));
                System.Threading.Thread.Sleep(100);
                skinTextBox2.Text = PadDevUtil.ToHexString(OpenedPad.read());
            }
            else
                skinTextBox2.Text = "没有连接触盘";
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (OpenedPad != null)
                OpenedPad.close();
            this.Close();
        }

        private void PadDev_Load(object sender, EventArgs e)
        {
            if (UtilValues.PublicValue.nanoPads.Length != 0)
                foreach (SayobotNanoPad Pad in UtilValues.PublicValue.nanoPads)
                    comboBox1.Items.Add(Pad.Name);
            else
                PublicControlers.notifyIcon.ShowBalloonTip(500, 
                    "SayobotBeatmapDownloader", "没有连接触盘", ToolTipIcon.Warning);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OpenedPad != null)
                OpenedPad.close();
            OpenedPad = new HIDDevice(UtilValues.PublicValue.nanoPads[comboBox1.SelectedIndex].devicePath, false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StaticUtilFunctions.FindNanoPad();
            comboBox1.Items.Clear();
            foreach (SayobotNanoPad Pad in UtilValues.PublicValue.nanoPads)
                comboBox1.Items.Add(Pad.Name);
            comboBox1.SelectedIndex = 0;
            
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(180, 180, 180, 180);
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.Transparent;
        }
    }
}
