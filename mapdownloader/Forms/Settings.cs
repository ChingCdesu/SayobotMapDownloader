using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace mapdownloader
{
    public partial class SettingForm : mapdownloader.BaseForm
    {
        public SettingForm()
        {
            InitializeComponent();
            StaticUtilFunctions.SetFormMid(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                skinTextBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        private void SettingForm_Load(object sender, EventArgs e)
        {
            this.Height = 390;
            panel1.Height = 336;
            panel2.Height = panel1.Height;
        }

        private void settingForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                label3.Text = "试听音量大小：" + Settings.PreviewVolume.ToString();
                skinTextBox1.Text = Settings.DefaultDownloadPath;
                gmTrackBar1.Value = Settings.PreviewVolume;
                skinCheckBox1.Checked = Settings.AutoOpen;
                if (Settings.MainFormImage != "")
                    pictureBox1.Image = Image.FromFile(Settings.MainFormImage);
                else
                    pictureBox1.BackColor= ColorTranslator.FromHtml(Settings.MainFormColor);
                pictureBox2.BackColor = ColorTranslator.FromHtml(Settings.SettingFormColor);
                label8.Text = "主窗口: " + Settings.MainFormOpacity.ToString() + "%";
                gmTrackBar2.Value = Settings.MainFormOpacity;
                label9.Text = "设置窗口: " + Settings.SettingFormOpacity.ToString() + "%";
                gmTrackBar3.Value = Settings.SettingFormOpacity;
                label10.Text = "内存用量限制: " + Settings.MemoryLimit.ToString() + " MB";
                gmTrackBar4.Value = Settings.MemoryLimit;
                skinCheckBox2.Checked = Settings.MemoryUsageShown;
            }
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            panel3.BackColor = Color.FromArgb(128, 180, 180, 180);
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            panel3.BackColor = Color.Transparent;
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigFile.UpdateAllConfigs();
            this.Hide();
        }

        private void skinTextBox1_Leave(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Length > 1024)
            {
                MessageBox.Show("路径过长", "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                skinTextBox1.Text = Settings.DefaultDownloadPath;
            }
            if (Directory.Exists(skinTextBox1.Text))
                Settings.DefaultDownloadPath = skinTextBox1.Text;
            else
            {
                MessageBox.Show("不存在的路径", "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                skinTextBox1.Text = Settings.DefaultDownloadPath;
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                openFileDialog1.Filter = "静态图片文件| *.jpg; *.png; *.jpeg; *.bmp";
                openFileDialog1.FileName = "";
                openFileDialog1.Title = "选择一张图片作为主窗口的背景";
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                    Settings.MainFormImage = openFileDialog1.FileName;
                    StaticUtilFunctions.LoadingMainFormBackgroundPic();
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    pictureBox1.Image = Image.FromFile(Environment.CurrentDirectory + "\\Resources\\Pics\\Empty.png");
                    Settings.MainFormImage = "";
                    PublicControlers.mainForm.BackgroundImage =
                        Image.FromFile(Environment.CurrentDirectory + "\\Resources\\Pics\\Empty.png");
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (DialogResult.OK == colorDialog1.ShowDialog())
                {
                    pictureBox1.BackColor = colorDialog1.Color;
                    Settings.SettingFormColor = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                    this.BackColor = colorDialog1.Color;
                }
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                pictureBox2.BackColor = colorDialog1.Color;
                Settings.SettingFormColor = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                this.BackColor = colorDialog1.Color;
            }
        }

        private void skinCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoOpen = skinCheckBox1.Checked;
        }

        private void gmTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            Settings.PreviewVolume = gmTrackBar1.Value;
            label3.Text = "试听音量大小：" + Settings.PreviewVolume.ToString();
            PublicControlers.mediaPlayer.settings.volume = gmTrackBar1.Value;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            panel2.Focus();
        }

        private void gmTrackBar2_ValueChanged(object sender, EventArgs e)
        {
            label8.Text = "主窗口: " + gmTrackBar2.Value.ToString() + "%";
            Settings.MainFormOpacity = gmTrackBar2.Value;
            PublicControlers.mainForm.Opacity = gmTrackBar2.Value / 100.0;
        }

        private void gmTrackBar3_ValueChanged(object sender, EventArgs e)
        {
            label9.Text = "设置窗口: " + gmTrackBar3.Value.ToString() + "%";
            Settings.SettingFormOpacity = gmTrackBar3.Value;
            this.Opacity = gmTrackBar3.Value / 100.0;
        }

        private void gmTrackBar4_ValueChanged(object sender, EventArgs e)
        {
            label10.Text = "内存用量限制: " + gmTrackBar4.Value + " MB";
            Settings.MemoryLimit = gmTrackBar4.Value;
        }

        private void skinCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.MemoryUsageShown = skinCheckBox2.Checked;
            PublicControlers.MemoryUsage.Visible = skinCheckBox2.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("你确定要重置所有设置吗？", "SayobotBeatmapDownloader", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (DialogResult.Yes == dialogResult)
                ConfigFile.ResetAllConfigs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = label2.Location.Y - 10;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = label3.Location.Y - 10;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = skinCheckBox1.Location.Y - 10;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = label4.Location.Y - 10;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = label7.Location.Y - 10;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //panel2.VerticalScroll.Value = label10.Location.Y - 10;
        }

    }
}
