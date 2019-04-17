using Newtonsoft.Json;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace mapdownloader
{
    public partial class MainForm : BaseForm
    {
        public MainForm()
        {
            StaticUtilFunctions.SetFormMid(this);
            InitializeComponent();
        }
        SettingForm settingForm = new SettingForm();
        BeatmapDetailInfo beatmapDetailInfo = new BeatmapDetailInfo();
        Filter filter = new Filter();
        Forms.Welcome welcome;
        
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            notifyIcon1.Dispose();
            
            Application.Exit();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            UtilValues.PublicValue.ProgramRunTime = 0;

            //Thread[] FormsLoad = {
            //    new Thread(new ParameterizedThreadStart(ChildForms.LoadSettingsForm)),
            //    new Thread(new ParameterizedThreadStart(ChildForms.LoadFilterForm))};
            //var waits = new EventWaitHandle[2];
            //int temp = 0;
            //foreach (Thread subThread in FormsLoad)
            //{
            //    var handler = new ManualResetEvent(false);
            //    waits[temp]=handler;
            //    subThread.Start(handler);
            //    subThread.DisableComObjectEagerCleanup();
            //}

            label1.BackColor = Color.Transparent;
            linkLabel1.BackColor = Color.Transparent;
            panel2.BackColor = Color.Transparent;
            menuStrip1.BackColor = Color.Transparent;
            panel1.BackColor = Color.Transparent;
            menuStrip1.BackColor = Color.Transparent;
            gmProgressBar1.XTheme = new Gdu.WinFormUI.ThemeProgressBarGreen();

            
            dataGridView1.Visible = false;
            string SupportStatus = StaticUtilFunctions.JsonGet("https://api.sayobot.cn/support");
            JavaScriptObject SupportStatusJson = (JavaScriptObject)JavaScriptConvert.DeserializeObject(SupportStatus);
            if (!SupportStatusJson.ContainsKey("data"))
            {
                MessageBox.Show("网络错误，请检查网络连接后重启程序", "SayobotBeatmapDownloader",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            JavaScriptObject Data = (JavaScriptObject)SupportStatusJson["data"];
            gmProgressBar1.Percentage = (Int32)(Convert.ToDouble(Data["total"]) / Convert.ToDouble(Data["target"]) * 100.0);
            linkLabel1.Text = "投喂进度：$" + Data["total"].ToString() + "/$" + Data["target"].ToString();
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://sayobot.cn/home");


            PublicControlers.notifyIcon = notifyIcon1;
            PublicControlers.mediaPlayer = axWindowsMediaPlayer1;
            PublicControlers.dataGridView = dataGridView1;
            PublicControlers.mainForm = this;
            PublicControlers.settingForm = settingForm;
            PublicControlers.MemoryUsage = label2;

            StaticUtilFunctions.SetFormMid(settingForm);
            StaticUtilFunctions.SetFormMid(beatmapDetailInfo);
            StaticUtilFunctions.SetFormMid(filter);
            
            if (File.Exists(ConfigFile.inifilepath))
                ConfigFile.GetAllConfigsApply();
            else
            {
                /*
                welcome = new Forms.Welcome();
                welcome.Show();
                */
                FileStream streamtmp = File.Create(ConfigFile.inifilename);
                streamtmp.Close();
                ConfigFile.ResetAllConfigs();
            }
            if (Settings.MainFormImage != "")
                StaticUtilFunctions.LoadingMainFormBackgroundPic();
            StaticUtilFunctions.FindNanoPad();
            if (UtilValues.PublicValue.nanoPads.Length != 0)
                notifyIcon1.ShowBalloonTip(500, "SayobotBeatmapDownloader",
                    "发现" + UtilValues.PublicValue.nanoPads.Length.ToString() + "个触盘", ToolTipIcon.Info);
            //WaitHandle.WaitAll(waits);
        }
        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            panel1.BackColor = Color.FromArgb(128, 180, 180, 180);
        }
        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(0, 180, 180, 180);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && button1.Text == "搜索")
            {
                dataGridView1.Rows.Clear();
                DataTable table = new DataTable();
                table.Columns.Add("艺术家 - 标题", typeof(string));
                table.Columns.Add("sid", typeof(string));
                table.Columns.Add("谱面状态", typeof(string));
                table.Columns.Add("Mapper", typeof(string));
                string GettedJsonString = StaticUtilFunctions.JsonGet(StaticUtilFunctions.Spawn_SearchHttpLink(textBox1.Text, 0));
                JavaScriptObject GettedJson = (JavaScriptObject)JavaScriptConvert.DeserializeObject(GettedJsonString);
                if (!GettedJson.ContainsKey("status"))
                {
                    MessageBox.Show("请求API失败", "网络错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (Convert.ToInt32(GettedJson["status"]) == -1)
                {
                    MessageBox.Show("没有查询到数据", "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.Visible = false;
                    label3.Visible = true;
                    return;
                }
                label3.Visible = false;
                JavaScriptArray Data = (JavaScriptArray)GettedJson["data"];
                
                for (int tmp=0;tmp<Data.Count;++tmp)
                {
                    JavaScriptObject SingleMap = (JavaScriptObject)Data[tmp];
                    string BasicInfo = SingleMap["artist"].ToString() + " - " + SingleMap["title"].ToString();
                    table.Rows.Add(BasicInfo, SingleMap["sid"].ToString(), UtilValues.PublicValue.MapStatus[Convert.ToInt32(SingleMap["approved"]) + 2], SingleMap["creator"].ToString());
                }
                while (Convert.ToInt32(GettedJson["endid"]) != 0 && table.Rows.Count < 1000)
                 {
                    var offset = Convert.ToInt32(GettedJson["endid"]);
                    GettedJsonString = StaticUtilFunctions.JsonGet(StaticUtilFunctions.Spawn_SearchHttpLink(textBox1.Text,offset));
                    GettedJson = (JavaScriptObject)JavaScriptConvert.DeserializeObject(GettedJsonString);
                    if (!GettedJson.ContainsKey("status"))
                    {
                        GettedJson.Add("endid", offset);
                        continue;
                    }
                    Data = (JavaScriptArray)GettedJson["data"];
                    for (int tmp = 0; tmp < Data.Count; ++tmp)
                    {
                        JavaScriptObject SingleMap = (JavaScriptObject)Data[tmp];
                        string BasicInfo = SingleMap["artist"].ToString() + " - " + SingleMap["title"].ToString();
                        table.Rows.Add(BasicInfo, SingleMap["sid"].ToString(), UtilValues.PublicValue.MapStatus[Convert.ToInt32(SingleMap["approved"]) + 2], SingleMap["creator"].ToString());
                    }
                }
                if (29 + 23 * table.Rows.Count > 354)
                    dataGridView1.Height = 354;
                else
                    dataGridView1.Height = 29 + 23 * table.Rows.Count;
                foreach (DataRow row in table.Rows)
                    dataGridView1.Rows.Add(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3]);
                dataGridView1.Visible = true;
                table.Clear();
                table.Dispose();
            }
            else if (textBox1.Text != "" && button1.Text == "查找")
            {
                bool flag = false;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    for (int tmp = 0; tmp < 4; tmp++)
                        if (row.Cells[tmp].Value.ToString().Contains(textBox1.Text))
                        {
                            flag = true;
                            break;
                        }
                    if (!flag)
                        row.Visible = false;
                }
            }
            else if (button1.Text == "过滤器")
            {
                PublicControlers.SearchBtn = button1;
                filter.BackColor = ColorTranslator.FromHtml(Settings.MainFormColor);
                filter.Show();
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingForm.Show();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.Links[linkLabel1.Links.IndexOf(e.Link)].Visited = true;
            string targetUrl = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(targetUrl);
        }
        private DataGridViewButtonCell vCell;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 4 && e.ColumnIndex != 5)
                return;
            string sid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string TitleArtist = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            if (dataGridView1.Columns[e.ColumnIndex].Name=="DownloadButton" && e.RowIndex>=0 &&
                ((DataGridViewButtonCell)dataGridView1.CurrentCell).FormattedValue.ToString() == "√")
            {
                UtilValues.DownloadInfo info = new UtilValues.DownloadInfo();
                vCell = (DataGridViewButtonCell)dataGridView1.CurrentCell;
                info.FileName = sid + " " + StaticUtilFunctions.RenameFile(TitleArtist) + ".osz";
                info.URL = "https://osu.sayobot.cn/osu.php?s=" + sid + ".osz";
                info.ClickedCell = (DataGridViewButtonCell)dataGridView1.CurrentCell;
                info.type = UtilValues.DownloadFileType.BeatmapFile;
                Thread DownloadThread = new Thread(new ParameterizedThreadStart(StaticUtilFunctions.DownloadFile));
                notifyIcon1.ShowBalloonTip(500, "SayobotMapDownloader", info.FileName + "开始下载", ToolTipIcon.Info);
                DownloadThread.Start(info);
                DownloadThread.DisableComObjectEagerCleanup();
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Listening" && e.RowIndex >= 0 &&
                ((DataGridViewButtonCell)dataGridView1.CurrentCell).FormattedValue.ToString()=="▶")
            {
                vCell = (DataGridViewButtonCell)dataGridView1.CurrentCell;
                for (int tmp=0;tmp<dataGridView1.RowCount;tmp++)
                    dataGridView1.Rows[tmp].Cells[5].Value = "▶";
                axWindowsMediaPlayer1.URL= "https://txy1.sayobot.cn/preview/" + sid + ".mp3";
                vCell.Value = "■";
                vCell.Tag = true;
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Listening" && e.RowIndex >= 0 &&
                vCell.FormattedValue.ToString() == "■")
            {
                vCell = (DataGridViewButtonCell)dataGridView1.CurrentCell;
                vCell.Value = "▶";
                vCell.Tag = true;
                axWindowsMediaPlayer1.URL = "";
            }
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode && textBox1.Focused)
                button1_Click(sender, e);
            else if (Keys.ShiftKey == e.KeyCode || Keys.ControlKey == e.KeyCode)
                button1.Text = "搜索";
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
        }
        private void 打开下载文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Settings.DefaultDownloadPath);
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.ForeColor = SystemColors.WindowText;
        }
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ArtistTitle" && e.RowIndex >= 0)
            {
                beatmapDetailInfo.label1.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                beatmapDetailInfo.vCell = (DataGridViewButtonCell)dataGridView1.Rows[e.RowIndex].Cells[4];
                beatmapDetailInfo.Show();
            }
        }
        /*
        protected override void WndProc(ref Message m)
        {
            if (m.Msg==0x2190)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0x8000:
                        DriveInfo[] drives = DriveGetDrives();
                        foreach (DriveInfo drive in drives)
                        {
                            if (drive.DriveType == DriveType.Removable)
                            {
                                MessageBox.Show("USB插入");
                                break;
                            }
                        }
                        break;
                    case 0x8004:
                        MessageBox.Show("USB卸载");
                        break;
                    default:
                        break;
                }
            }
            base.WndProc(ref m);
        }
        */
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.ShiftKey == e.KeyCode)
                button1.Text = "查找";
            else if (Keys.ControlKey == e.KeyCode)
                button1.Text = "过滤器";
        }
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState==WMPLib.WMPPlayState.wmppsStopped)
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[5].Value = "▶";
                    row.Cells[5].Tag = true;
                }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            double CurrentMemoryUsage = Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0;
            UtilValues.PublicValue.ProgramRunTime += 1;
            if (Settings.MemoryUsageShown)
            label2.Text = String.Format("运行时间：{0}s 内存占用：{1:f2}mb", UtilValues.PublicValue.ProgramRunTime,
                CurrentMemoryUsage);
            if (CurrentMemoryUsage > Convert.ToDouble(Settings.MemoryLimit))
                StaticUtilFunctions.ClearMemory();
        }

        private void 触盘开发界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PadDev padDev = new PadDev();
            padDev.Show();
        }

        private void 触盘设置界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PadSettings padSettings = new PadSettings();
            padSettings.Show();
        }
    }
  
}
