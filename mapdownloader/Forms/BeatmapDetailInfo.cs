using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace mapdownloader
{

    public partial class BeatmapDetailInfo : mapdownloader.BaseForm
    {
        public DataGridViewButtonCell vCell;
        public BeatmapDetailInfo()
        {
            InitializeComponent();
            StaticUtilFunctions.SetFormMid(this);
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            panel1.BackColor = Color.FromArgb(128, 180, 180, 180);
        }
        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Transparent;
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        private GraphicsPath GetRoundRectPath(int width, int height, int radius = 8)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddArc(0, 0, radius, radius, 180, 90);
            graphicsPath.AddLine(radius, 0, width - radius, 0);
            graphicsPath.AddArc(width - 10, 0, radius, radius, 270, 90);
            graphicsPath.AddLine(width, radius, width, height - radius);
            graphicsPath.AddArc(width - 10, height - 10, radius, radius, 0, 90);
            graphicsPath.AddLine(width - radius, height, radius, height);
            graphicsPath.AddArc(0, height - 10, radius, radius, 90, 90);
            graphicsPath.AddLine(0, height - radius, 0, radius);
            return graphicsPath;
        }
        public BeatmapSetInfo setInfo = new BeatmapSetInfo();

        private void DrawRoundRectControl(Control control,Color color)
        {
            Bitmap bitmap = new Bitmap(control.Size.Width, control.Size.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(new SolidBrush(color), GetRoundRectPath(control.Size.Width, control.Size.Height));
            control.BackgroundImage = bitmap;
        }
        private void label1_TextChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(Environment.CurrentDirectory + "\\Resources\\Pics\\loading.png");
            pictureBox1.ImageLocation = "https://cdn.sayobot.cn:25225/beatmaps/" + label1.Text + "/covers/cover.jpg?0";
            string GettedJsonString = StaticUtilFunctions.JsonGet("https://api.sayobot.cn/v2/beatmapinfo?0=" + label1.Text);
            JavaScriptObject Json = (JavaScriptObject)JavaScriptConvert.DeserializeObject(GettedJsonString);
            if (!Json.ContainsKey("status"))
            {
                MessageBox.Show("请求API失败", "网络错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (Convert.ToInt32(Json["status"]) == -1)
            {
                MessageBox.Show("没有查询到数据", "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                JavaScriptObject Data = (JavaScriptObject)Json["data"];
                setInfo.approved = Convert.ToInt16(Data["approved"]);
                setInfo.approved_date = Convert.ToInt32(Data["approved_date"]);
                setInfo.artist = Convert.ToString(Data["artist"]);
                setInfo.beatmap_count = Convert.ToInt16(Data["bids_amount"]);
                setInfo.bpm = Convert.ToDouble(Data["bpm"]);
                setInfo.genre = Convert.ToInt16(Data["genre"]);
                setInfo.language = Convert.ToInt16(Data["language"]);
                setInfo.last_update = Convert.ToInt32(Data["last_update"]);
                setInfo.preview = Convert.ToInt16(Data["preview"]);
                setInfo.sid = Convert.ToInt32(Data["sid"]);
                setInfo.source = Convert.ToString(Data["source"]);
                setInfo.storyboard = Convert.ToInt16(Data["storyboard"]);
                setInfo.tags = Convert.ToString(Data["tags"]);
                setInfo.title = Convert.ToString(Data["title"]);
                setInfo.video = Convert.ToInt16(Data["video"]);
                setInfo.favorite = Convert.ToInt16(Data["favourite_count"]);
                JavaScriptArray mapArray = (JavaScriptArray)Data["bid_data"];
                setInfo.beatmapInfos = new System.Collections.Generic.List<BeatmapInfo>();
                for (int i = 0; i < setInfo.beatmap_count; i++)
                {
                    JavaScriptObject singleMap = (JavaScriptObject)mapArray[i];
                    BeatmapInfo beatmap = new BeatmapInfo
                    {
                        ar = Convert.ToDouble(singleMap["AR"]),
                        bid = Convert.ToInt32(singleMap["bid"]),
                        creator = Convert.ToString(Data["creator"]),
                        cs = Convert.ToDouble(singleMap["CS"]),
                        hp = Convert.ToDouble(singleMap["HP"]),
                        img_base64 = Convert.ToString(singleMap["img"]),
                        maxcombo = Convert.ToInt32(singleMap["maxcombo"]),
                        od = Convert.ToDouble(singleMap["OD"]),
                        passcount = Convert.ToInt32(singleMap["passcount"]),
                        playcount = Convert.ToInt32(singleMap["playcount"]),
                        stars = Convert.ToDouble(singleMap["star"]),
                        version = Convert.ToString(singleMap["version"]),
                        length = Convert.ToInt16(singleMap["length"])
                    };
                    if (beatmap.passcount != 0)
                        beatmap.passrate = Convert.ToDouble(beatmap.passcount * 100) / beatmap.playcount;
                    else
                        beatmap.passrate = 0.0D;
                    setInfo.beatmapInfos.Add(beatmap);
                }
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                string[] MapStatus = { "graveyard", "WIP", "pending", "ranked", "approved", "qualified", "loved" };
                label2.Text = setInfo.beatmapInfos[0].version;
                label23.Text = Convert.ToString(setInfo.sid);
                label24.Text = MapStatus[setInfo.approved + 2];
                label3.Text = setInfo.artist + " - " + setInfo.title;
                button8.Text = startTime.AddSeconds(setInfo.last_update + 8 * 3600).ToString("yyyy-MM-dd HH:mm:ss");
                if (setInfo.approved_date > 0)
                    button9.Text = startTime.AddSeconds(setInfo.approved_date + 8 * 3600).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    button9.Text = "null";
                button2.Text = Convert.ToString(setInfo.bpm);
                button5.Text = UtilValues.PublicValue.Language[setInfo.language];
                button6.Text = UtilValues.PublicValue.Genre[setInfo.genre];
                button7.Text = Convert.ToString(setInfo.favorite);
                toolTip1.SetToolTip(label8, setInfo.tags);
                toolTip1.SetToolTip(label12, setInfo.source);
                if (50 * setInfo.beatmap_count < 450)
                    panel2.Size = new Size(50 * setInfo.beatmap_count, 32);
                else
                    panel2.Size = new Size(450, 32);
                DrawRoundRectControl(panel2, Color.FromArgb(127, 216, 148, 139));
                displayInfo(0);
                if (!this.Visible)
                    this.Show();
            }
        }

        private void displayInfo(int index)
        {
            button1.Text = setInfo.beatmapInfos[index].creator;
            button4.Text = Convert.ToString(setInfo.beatmapInfos[index].maxcombo) + "x";
            button10.Text = Convert.ToString(setInfo.beatmapInfos[index].passcount) +
                "/" + Convert.ToString(setInfo.beatmapInfos[index].playcount) + "(" +
                Convert.ToString(Math.Round(setInfo.beatmapInfos[index].passrate, 2)) + "%)";
            label16.Text = "CS\n" + Convert.ToString(setInfo.beatmapInfos[index].cs);
            label17.Text = "Stars\n" + Convert.ToString(Math.Round(setInfo.beatmapInfos[index].stars, 2));
            label18.Text = "AR\n" + Convert.ToString(setInfo.beatmapInfos[index].ar);
            label19.Text = "HP\n" + Convert.ToString(setInfo.beatmapInfos[index].hp);
            label20.Text = "OD\n" + Convert.ToString(setInfo.beatmapInfos[index].od);
            button3.Text = Convert.ToString(Convert.ToInt16(setInfo.beatmapInfos[index].length / 60)) + ":" +
                String.Format("{0:D2}",setInfo.beatmapInfos[index].length % 60);

            Graphics graphics = pictureBox2.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(this.BackColor);
            Pen DeepGrayPen = new Pen(Color.FromArgb(255, 127, 127, 127));
            Pen DeepBluePen = new Pen(Color.DeepSkyBlue);
            double x = Math.Sin(18d / 180d * Math.PI) * 60d,
                     y = Math.Cos(18d / 180d * Math.PI) * 60d,
                     a = Math.Sin(36d / 180d * Math.PI) * 60d,
                     b = Math.Cos(36d / 180d * Math.PI) * 60d;

            PointF[] SourcePoints = new PointF[6];
            SourcePoints[0] = new PointF(75f, 15f);
            SourcePoints[1] = new PointF(75f - (float)y, 75f - (float)x);
            SourcePoints[2] = new PointF(75f - (float)a, 75 + (float)b);
            SourcePoints[3] = new PointF(75 + (float)a, 75 + (float)b);
            SourcePoints[4] = new PointF(75 + (float)y, 75f - (float)x);
            SourcePoints[5] = SourcePoints[0];
            PointF[] p2 = new PointF[6];
            PointF[] p3 = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                p2[i].X = SourcePoints[i].X * 0.7f + 0.3f * 75f;
                p2[i].Y = SourcePoints[i].Y * 0.7f + 0.3f * 75f;
                p3[i].X = SourcePoints[i].X * 0.4f + 0.6f * 75f;
                p3[i].Y = SourcePoints[i].Y * 0.4f + 0.6f * 75f;
            }
            graphics.DrawLines(DeepGrayPen, SourcePoints);
            graphics.DrawLines(DeepGrayPen, p2);
            graphics.DrawLines(DeepGrayPen, p3);

            PointF[] DataPoints = new PointF[6];
            DataPoints[0] = new PointF(75f, 75f - (float)setInfo.beatmapInfos[index].cs / 9f * 60f);
            DataPoints[1] = new PointF(75f - (float)setInfo.beatmapInfos[index].stars / 9f * 60f * (float)Math.Cos(18f / 180f * Math.PI),
                75f - (float)setInfo.beatmapInfos[index].stars / 9f * 60f * (float)Math.Sin(18f / 180f * Math.PI));
            DataPoints[2] = new PointF(75f - (float)setInfo.beatmapInfos[index].hp / 9f * 60f * (float)Math.Sin(36f / 180f * Math.PI),
                75f + (float)setInfo.beatmapInfos[index].hp / 9f * 60f * (float)Math.Cos(36f / 180f * Math.PI));
            DataPoints[3] = new PointF(75f + (float)setInfo.beatmapInfos[index].od / 9f * 60f * (float)Math.Sin(36f / 180f * Math.PI),
                75f + (float)setInfo.beatmapInfos[index].od / 9f * 60f * (float)Math.Cos(36f / 180f * Math.PI));
            DataPoints[4] = new PointF(75f + (float)setInfo.beatmapInfos[index].ar / 9f * 60f * (float)Math.Cos(18f / 180f * Math.PI),
                75f - (float)setInfo.beatmapInfos[index].ar / 9f * 60f * (float)Math.Sin(18f / 180f * Math.PI));
            DataPoints[5] = DataPoints[0];
            graphics.DrawLines(DeepBluePen, DataPoints);

            SolidBrush DeepBlueBrush = new SolidBrush(Color.DeepSkyBlue);
            graphics.FillEllipse(DeepBlueBrush, DataPoints[0].X - 2, DataPoints[0].Y - 2, 4, 4);
            graphics.FillEllipse(DeepBlueBrush, DataPoints[1].X - 2, DataPoints[1].Y - 2, 4, 4);
            graphics.FillEllipse(DeepBlueBrush, DataPoints[2].X - 2, DataPoints[2].Y - 2, 4, 4);
            graphics.FillEllipse(DeepBlueBrush, DataPoints[3].X - 2, DataPoints[3].Y - 2, 4, 4);
            graphics.FillEllipse(DeepBlueBrush, DataPoints[4].X - 2, DataPoints[4].Y - 2, 4, 4);

            graphics.Dispose();
        }
        private void roundButton1_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[0].version;
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            displayInfo(0);
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            displayInfo(1);
        }

        private void roundButton3_Click(object sender, EventArgs e)
        {
            displayInfo(2);
        }

        private void roundButton4_Click(object sender, EventArgs e)
        {
            displayInfo(3);
        }

        private void roundButton5_Click(object sender, EventArgs e)
        {
            displayInfo(4);
        }

        private void roundButton6_Click(object sender, EventArgs e)
        {
            displayInfo(5);
        }

        private void roundButton7_Click(object sender, EventArgs e)
        {
            displayInfo(6);
        }

        private void roundButton8_Click(object sender, EventArgs e)
        {
            displayInfo(7);
        }

        private void roundButton9_Click(object sender, EventArgs e)
        {
            displayInfo(8);
        }

        private void roundButton2_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[1].version;
        }

        private void roundButton3_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[2].version;
        }

        private void roundButton4_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[3].version;
        }

        private void roundButton5_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[4].version;
        }

        private void roundButton6_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[5].version;
        }

        private void roundButton7_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[6].version;
        }

        private void roundButton8_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[7].version;
        }

        private void roundButton9_MouseMove(object sender, MouseEventArgs e)
        {
            label2.Text = setInfo.beatmapInfos[8].version;
        }

        private void BeatmapDetailInfo_Load(object sender, EventArgs e)
        {
            this.Opacity = 90;
            this.BackColor = Color.FromArgb(255, 248, 248, 248);
            DrawRoundRectControl(button1, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button2, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button3, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button4, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button5, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button6, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button7, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button8, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button9, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button10, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button11, Color.FromArgb(255, 187, 222, 138));
            DrawRoundRectControl(button12, Color.FromArgb(255, 255, 212, 123));
            DrawRoundRectControl(button13, Color.FromArgb(255, 178, 152, 243));
            DrawRoundRectControl(button14, Color.FromArgb(255, 239, 165, 126));
            DrawRoundRectControl(button15, Color.FromArgb(255, 245, 186, 120));
            DrawRoundRectControl(button16, Color.FromArgb(255, 117, 169, 127));
            DrawRoundRectControl(button17, Color.FromArgb(255, 200, 200, 200));
            DrawRoundRectControl(panel2, Color.FromArgb(255, 225, 143, 146));
            DrawRoundRectControl(panel3, Color.White);
            DrawRoundRectControl(panel4, Color.White);
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            UtilValues.DownloadInfo info = new UtilValues.DownloadInfo();
            info.FileName = setInfo.sid.ToString() + " " + setInfo.artist + " - " + setInfo.title + ".osz";
            info.FileName = StaticUtilFunctions.RenameFile(info.FileName);
            info.URL = "https://osu.sayobot.cn/osu.php?s=" + setInfo.sid.ToString() + ".osz";
            info.ClickedCell = this.vCell;
            info.type = UtilValues.DownloadFileType.BeatmapFile;
            Thread DownloadThread = new Thread(new ParameterizedThreadStart(StaticUtilFunctions.DownloadFile));
            PublicControlers.notifyIcon.ShowBalloonTip(500, "SayobotMapDownloader", info.FileName + "开始下载", ToolTipIcon.Info);
            DownloadThread.Start(info);
            DownloadThread.DisableComObjectEagerCleanup();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            UtilValues.DownloadInfo info = new UtilValues.DownloadInfo();
            info.FileName = setInfo.sid.ToString() + " " + setInfo.artist + " - " + setInfo.title + ".png";
            info.FileName = StaticUtilFunctions.RenameFile(info.FileName);
            info.URL = "https://txy1.sayobot.cn/maps/bg/" + setInfo.beatmapInfos[0].img_base64;
            info.type = UtilValues.DownloadFileType.PicFile;
            Thread DownloadThread = new Thread(new ParameterizedThreadStart(StaticUtilFunctions.DownloadFile));
            DownloadThread.Start(info);
            DownloadThread.DisableComObjectEagerCleanup();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (PublicControlers.mediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                PublicControlers.mediaPlayer.URL = "";
            else
            {
                PublicControlers.mediaPlayer.settings.volume = Settings.PreviewVolume;
                PublicControlers.mediaPlayer.URL = "https://txy1.sayobot.cn/preview/" + setInfo.sid.ToString() + ".mp3";
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject("https://sayobot.cn/?search=s/" + setInfo.sid.ToString());
            PublicControlers.notifyIcon.ShowBalloonTip(500, "SayobotBeatmapDownloader", "已将谱面链接复制到剪贴板", ToolTipIcon.Info);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            UtilValues.DownloadInfo info = new UtilValues.DownloadInfo();
            info.URL = "https://txy1.sayobot.cn/maps/audio/" + setInfo.sid.ToString();
            info.FileName= setInfo.sid.ToString() + " " + setInfo.artist + " - " + setInfo.title + ".mp3";
            info.FileName = StaticUtilFunctions.RenameFile(info.FileName);
            info.type = UtilValues.DownloadFileType.MusicFile;
            Thread DownloadThread = new Thread(new ParameterizedThreadStart(StaticUtilFunctions.DownloadFile));
            PublicControlers.notifyIcon.ShowBalloonTip(500, "SayobotMapDownloader", info.FileName + "开始下载", ToolTipIcon.Info);
            DownloadThread.Start(info);
            DownloadThread.DisableComObjectEagerCleanup();
        }

    }
}
