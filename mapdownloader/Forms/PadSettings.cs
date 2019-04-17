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
    public partial class PadSettings : mapdownloader.BaseForm
    {
        public PadSettings()
        {
            InitializeComponent();
            StaticUtilFunctions.SetFormMid(this);
        }

        static HIDDevice OpenedPad = UtilValues.PublicValue.nanoPads.Length != 0 ?
            new HIDDevice(UtilValues.PublicValue.nanoPads[0].devicePath, false) : null;

        private void PadSettings_Load(object sender, EventArgs e)
        {
            if (UtilValues.PublicValue.nanoPads.Length != 0)
                foreach (SayobotNanoPad Pad in UtilValues.PublicValue.nanoPads)
                    comboBox1.Items.Add(Pad.Name);
            else
                PublicControlers.notifyIcon.ShowBalloonTip(500,
                    "SayobotBeatmapDownloader", "没有连接触盘", ToolTipIcon.Warning);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StaticUtilFunctions.FindNanoPad();
            comboBox1.Items.Clear();
            foreach (SayobotNanoPad Pad in UtilValues.PublicValue.nanoPads)
                comboBox1.Items.Add(Pad.Name);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OpenedPad != null)
                OpenedPad.close();
            OpenedPad = new HIDDevice(UtilValues.PublicValue.nanoPads[comboBox1.SelectedIndex].devicePath, false);
            //读取配置

        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(128, 128, 128, 128);
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.BackColor = Color.Transparent;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
    public class PadSetConstValue
    {
        public class Light
        {
            public enum Speed
            {
                Quarter = 0x00,
                Half,
                Normal,
                Double
            }
            public enum Mode
            {
                Constant = 0x00,
                BreathingSingle = 0x02,
                Breathing = 0x08,
                GradualChange = 0x18,
                Switch = 0x22,
                FadeOut = 0x26,
                FadeIn = 0x2B,
                Flash = 0x30,
                SingleFlash = 0x35,
                Unknown = 0xff
            }
            public enum ColorMode
            {
                Single = 0x00,
                SystemLoop1 = 0x01,
                SystemLoop2 = 0x02,
                CustomLoop = 0x03,
                Randomize = 0x04,
                Unknown = 0xff
            }
        }
        public static class Keys
        {
            public enum CharKeys
            {
                Unknown = 0,
                A = 0x04, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
                Char1, Char2, Char3, Char4, Char5, Char6, Char7, Char8, Char9, Char0,
                Enter, Esc, Backspace, Tab, SpaceBar
            }
            public enum ControlKeys
            {
                RWin = 0x80,
                RAlt = 0x40,
                RShift = 0x20,
                RCtrl = 0x10,
                LWin = 0x08,
                LAlt = 0x04,
                LShift = 0x02,
                LCtrl = 0x01
            }
        }
    }
}
