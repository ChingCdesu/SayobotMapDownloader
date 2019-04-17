using HIDInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace mapdownloader
{
    public class BeatmapInfo
    {
        public Int32 bid, creator_id;
        public string creator, version;
        public double cs, ar, od, hp, stars;
        public double passrate;
        public Int32 playcount, passcount, maxcombo;
        public string img_base64;
        public Int16 length;
    };
    public class BeatmapSetInfo
    {
        public Int32 sid;
        public Int16 beatmap_count, favorite;
        public Int16 approved, video, storyboard, preview, language, genre;
        public string title, artist, tags, source;
        public Int32 last_update, approved_date;
        public double bpm;
        public List<BeatmapInfo> beatmapInfos;
    };
    /// <summary>
    /// INI文件设置（路径为当前目录下的Config.ini）
    /// </summary>
    public class ConfigFile
    {

        //ini文件名称
        public static string inifilename = "Config.ini";
        //获取ini文件路径
        public static string inifilepath = Directory.GetCurrentDirectory() + "\\" + inifilename;

        public static string GetValue(string key)
        {
            StringBuilder s = new StringBuilder(1024);
            SystemDllFunctions.GetPrivateProfileString("UserConfig", key, "", s, 1024, inifilepath);
            return s.ToString();
        }

        public static void SetValue(string key, string value)
        {
            try
            {
                SystemDllFunctions.WritePrivateProfileString("UserConfig", key, value, inifilepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetAllConfigsApply()
        {
            string SettingErrorInfo = "";

            Settings.AutoOpen = GetValue("AutoOpen") == "true" ? true : false;
            if (!Settings.AutoOpen)
                SetValue("AutoOpen", "false");
            try
            {
                Settings.DataFormCellNormalBackgroundColor = GetValue("DataFormCellNormalBackgroundColor");
                PublicControlers.dataGridView.RowsDefaultCellStyle.BackColor =
                    ColorTranslator.FromHtml(Settings.DataFormCellNormalBackgroundColor);
            }
            catch (Exception)
            {
                PublicControlers.dataGridView.RowsDefaultCellStyle.BackColor = SystemColors.Control;
                Settings.DataFormCellNormalBackgroundColor = UtilValues.PublicValue.ControlColorCode;
                SetValue("DataFormCellNormalBackgroundColor", UtilValues.PublicValue.ControlColorCode);
                SettingErrorInfo += "设置的表格普通项背景颜色错误，已被重置\n";
            }
            try
            {
                Settings.DataFormCellSelectedBackgroundColor = GetValue("DataFormCellSelectedBackgroundColor");
                PublicControlers.dataGridView.RowsDefaultCellStyle.SelectionBackColor
                = ColorTranslator.FromHtml(Settings.DataFormCellSelectedBackgroundColor);
            }
            catch (Exception)
            {
                Settings.DataFormCellSelectedBackgroundColor = UtilValues.PublicValue.HighlightColorCode;
                PublicControlers.dataGridView.RowsDefaultCellStyle.SelectionBackColor
                    = ColorTranslator.FromHtml(Settings.DataFormCellSelectedBackgroundColor);
                SetValue("DataFormCellSelectedBackgroundColor", UtilValues.PublicValue.HighlightColorCode);
                SettingErrorInfo += "设置的表格选中项背景颜色错误，已被重置\n";
            }
            try
            {
                Settings.DataFormHeaderColor = GetValue("DataFormHeaderColor");
                PublicControlers.dataGridView.ColumnHeadersDefaultCellStyle.BackColor
                = ColorTranslator.FromHtml(Settings.DataFormHeaderColor);
            }
            catch (Exception)
            {
                Settings.DataFormHeaderColor = UtilValues.PublicValue.ControlColorCode;
                PublicControlers.dataGridView.ColumnHeadersDefaultCellStyle.BackColor
                    = ColorTranslator.FromHtml(Settings.DataFormHeaderColor);
                SetValue("DataFormHeaderColor", UtilValues.PublicValue.ControlColorCode);
                SettingErrorInfo += "设置的表格标题栏颜色值错误，已被重置\n";
            }

            Settings.DefaultDownloadPath = GetValue("DefaultDownloadPath");
            if (!Directory.Exists(Settings.DefaultDownloadPath))
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Maps"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Maps");
                Settings.DefaultDownloadPath = Environment.CurrentDirectory + "\\Maps";
                SettingErrorInfo += "设置的默认下载路径不存在，已被重置\n";
            }

            try
            {
                Settings.MainFormColor = GetValue("MainFormColor");
                PublicControlers.mainForm.BackColor =
                    ColorTranslator.FromHtml(Settings.MainFormColor);
            }
            catch (Exception)
            {
                Settings.MainFormColor = UtilValues.PublicValue.ControlColorCode;
                PublicControlers.mainForm.BackColor
                    = ColorTranslator.FromHtml(Settings.MainFormColor);
                SetValue("MainFormColor", UtilValues.PublicValue.ControlColorCode);
                SettingErrorInfo += "设置的主窗口背景颜色值错误，已被重置\n";
            }

            Settings.MainFormImage = GetValue("MainFormImage");
            if (!File.Exists(Settings.MainFormImage) && Settings.MainFormImage != "")
            {
                Settings.MainFormImage = "";
                SetValue("MainFormImage", "");
                SettingErrorInfo += "设置的主窗口背景图片文件不存在，已被重置\n";
            }

            try
            {
                Settings.MainFormOpacity = Convert.ToInt32(GetValue("MainFormOpacity"));
                if (Settings.MainFormOpacity > 100 || Settings.MainFormOpacity < 0)
                    throw new Exception();
            }
            catch (Exception)
            {
                Settings.MainFormOpacity = 100;
                SetValue("MainFormOpacity", "100");
                SettingErrorInfo += "设置的主窗口透明度值超出正常范围，已被重置\n";
            }
            try
            {
                Settings.MemoryLimit = Convert.ToInt32(GetValue("MemoryLimit"));
                if (Settings.MemoryLimit > 1024 || Settings.MemoryLimit < 20)
                    throw new Exception();
            }
            catch (Exception)
            {
                Settings.MemoryLimit = 50;
                SetValue("MemoryLimit", "50");
                SettingErrorInfo += "设置的内存用量限制超出正常范围，已被重置\n";
            }
            Settings.MemoryUsageShown = GetValue("MemoryUsageShown") == "true" ? true : false;
            if (!Settings.MemoryUsageShown)
                SetValue("MemoryUsageShown", "false");

#if DEBUG
            Settings.MemoryUsageShown = true;
#endif
            try
            {
                Settings.PreviewVolume = Convert.ToInt32(GetValue("PreviewVolume"));
                if (Settings.PreviewVolume > 100 || Settings.PreviewVolume < 0)
                    throw new Exception();
            }
            catch (Exception)
            {
                Settings.PreviewVolume = 50;
                SetValue("PreviewVolume", "50");
                SettingErrorInfo += "设置的试听音量超出正常范围，已被重置\n";
            }

            try
            {
                Settings.SettingFormColor = GetValue("SettingFormColor");
                PublicControlers.settingForm.BackColor =
                    ColorTranslator.FromHtml(Settings.SettingFormColor);
            }
            catch (Exception)
            {
                Settings.SettingFormColor = UtilValues.PublicValue.ControlColorCode;
                PublicControlers.settingForm.BackColor
                    = ColorTranslator.FromHtml(Settings.SettingFormColor);
                SetValue("SettingFormColor", UtilValues.PublicValue.ControlColorCode);
                SettingErrorInfo += "设置的设置窗口背景颜色值错误，已被重置\n";
            }

            try
            {
                Settings.SettingFormOpacity = Convert.ToInt32(GetValue("SettingFormOpacity"));
                if (Settings.SettingFormOpacity > 100 || Settings.SettingFormOpacity < 0)
                    throw new Exception();
            }
            catch (Exception)
            {
                Settings.SettingFormOpacity = 100;
                SetValue("SettingFormOpacity", "100");
                SettingErrorInfo += "设置的设置窗口透明度值超出正常范围，已被重置";
            }
            if (SettingErrorInfo != "")
                MessageBox.Show(SettingErrorInfo, "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ResetAllConfigs()
        {
            ConfigFile.SetValue("MainFormColor", UtilValues.PublicValue.ControlColorCode);
            Settings.MainFormColor = UtilValues.PublicValue.ControlColorCode;
            PublicControlers.mainForm.BackColor = SystemColors.Control;

            ConfigFile.SetValue("MainFormImage", "");
            Settings.MainFormImage = "";
            PublicControlers.mainForm.BackgroundImage = Image.FromFile(Environment.CurrentDirectory + "\\Resources\\Pics\\Empty.png");

            ConfigFile.SetValue("MainFormOpacity", "100");
            Settings.MainFormOpacity = 100;
            PublicControlers.mainForm.Opacity = 1;

            ConfigFile.SetValue("AutoOpen", "false");
            Settings.AutoOpen = false;

            ConfigFile.SetValue("SettingFormOpacity", "100");
            Settings.SettingFormOpacity = 100;
            PublicControlers.settingForm.Opacity = 1;

            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Maps");
            ConfigFile.SetValue("DefaultDownloadPath", Environment.CurrentDirectory + "\\Maps");
            Settings.DefaultDownloadPath = Environment.CurrentDirectory + "\\Maps";

            ConfigFile.SetValue("PreviewVolume", "50");
            Settings.PreviewVolume = 50;
            PublicControlers.mediaPlayer.settings.volume = 50;

            ConfigFile.SetValue("DataFormHeaderColor", UtilValues.PublicValue.ControlColorCode);
            Settings.DataFormHeaderColor = UtilValues.PublicValue.ControlColorCode;
            PublicControlers.dataGridView.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;

            ConfigFile.SetValue("DataFormDefaultCellNormalColor", UtilValues.PublicValue.ControlColorCode);
            Settings.DataFormCellNormalBackgroundColor = UtilValues.PublicValue.ControlColorCode;
            PublicControlers.dataGridView.RowsDefaultCellStyle.BackColor = SystemColors.Control;

            ConfigFile.SetValue("DataFormDefaultCellSelectedColor", UtilValues.PublicValue.HighlightColorCode);
            Settings.DataFormCellSelectedBackgroundColor = UtilValues.PublicValue.HighlightColorCode;
            PublicControlers.dataGridView.RowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;

            ConfigFile.SetValue("MemoryLimit", "50");
            Settings.MemoryLimit = 50;

            ConfigFile.SetValue("MemoryUsageShown", "false");
            Settings.MemoryUsageShown = false;
        }
        public static void UpdateAllConfigs()
        {
            if (Settings.AutoOpen)
                SetValue("AutoOpen", "true");
            else
                SetValue("AutoOpen", "false");
            SetValue("DataFormCellNormalBackgroundColor", Settings.DataFormCellNormalBackgroundColor);
            SetValue("DataFormCellSelectedBackgroundColor", Settings.DataFormCellSelectedBackgroundColor);
            SetValue("DataFormHeaderColor", Settings.DataFormHeaderColor);
            SetValue("DefaultDownloadPath", Settings.DefaultDownloadPath);
            SetValue("MainFormColor", Settings.MainFormColor);
            SetValue("MainFormImage", Settings.MainFormImage);
            SetValue("MainFormOpacity", Settings.MainFormOpacity.ToString());
            SetValue("MemoryLimit", Settings.MemoryLimit.ToString());
            if (Settings.MemoryUsageShown)
                SetValue("MemoryUsageShown", "true");
            else
                SetValue("MemoryUsageShown", "false");
            SetValue("PreviewVolume", Settings.PreviewVolume.ToString());
            SetValue("SettingFormColor", Settings.SettingFormColor);
            SetValue("SettingFormOpacity", Settings.SettingFormOpacity.ToString());
        }
    }
    /// <summary>
    /// 用户自定义设置（变量名与Config.ini文件中必须相同）
    /// </summary>
    public static class Settings
    {
        public static string DefaultDownloadPath;

        public static string MainFormColor;
        public static string MainFormImage;
        public static int MainFormOpacity;

        public static string SettingFormColor;
        public static int SettingFormOpacity;

        public static bool AutoOpen;

        public static int PreviewVolume;

        public static int MemoryLimit;
        public static bool MemoryUsageShown;

        public static string DataFormHeaderColor;
        public static string DataFormCellNormalBackgroundColor;
        public static string DataFormCellSelectedBackgroundColor;

    }
    /// <summary>
    /// 用户系统信息
    /// </summary>
    public static class SystemInfo
    {
        public static string OSVersion;
        public static string DeviceUniqueCode;
    }
    /// <summary>
    /// 公共控件
    /// </summary>
    public static class PublicControlers
    {
        public static NotifyIcon notifyIcon;
        public static AxWMPLib.AxWindowsMediaPlayer mediaPlayer;
        public static DataGridView dataGridView;
        public static Form mainForm;
        public static Form settingForm;
        public static Label MemoryUsage;
        public static Button SearchBtn;
    }
    /// <summary>
    /// 工具函数
    /// </summary>
    public static class StaticUtilFunctions
    {
        public static string JsonGet(string url)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "get";
                wbRequest.KeepAlive = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
                string CertificateFilePath = Directory.GetCurrentDirectory() + "\\Resources\\crt.cer";
                X509Certificate cert = new X509Certificate(CertificateFilePath);
                wbRequest.ClientCertificates.Add(cert);
                wbRequest.UserAgent = "SayobotBeatmapDownloader(ChingC ver.)";
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    StreamReader sReader = new StreamReader(responseStream);
                    result = sReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "{\"error\":1}";
            }
            return result;
        }
        public static byte[] GetHash(string str)
        {
            HashAlgorithm algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
        }
        public static string GetHashString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(str))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
        public static void LoadingMainFormBackgroundPic()
        {
            /*
            Image BGImage = Image.FromFile(Settings.MainFormImage);
            PointF point;
            SizeF size;
            if (BGImage.Width > BGImage.Height)
            {
                point = new PointF((BGImage.Width - 778.0f / 522.0f * BGImage.Height) / 2, 0);
                size = new SizeF(778.0f / 522.0f * BGImage.Height, BGImage.Height);
            }
            else
            {
                point = new PointF(0, (BGImage.Height - 522.0f / 778.0f * BGImage.Width) / 2);
                size = new SizeF(778.0f / 522.0f * BGImage.Height, BGImage.Height);
            }
            Bitmap bitmap = new Bitmap((int)size.Width, (int)size.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(BGImage, new RectangleF(0, 0, size.Width, size.Height),
                new RectangleF(point, size), GraphicsUnit.Pixel);
            PublicControlers.mainForm.BackgroundImage = bitmap;
            graphics.Dispose();
            BGImage.Dispose();
            */
            PublicControlers.mainForm.BackgroundImage = Image.FromFile(Settings.MainFormImage);
        }
        public static string GetPCUniqueCode()
        {
            ManagementObjectSearcher CPUs = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectSearcher MotherBoards = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            ManagementObjectSearcher LogicalDisks = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
            string str = "";
            foreach (var CPU in CPUs.Get())
                str += (string)CPU.GetPropertyValue("ProcessorID");
            foreach (var MotherBoard in MotherBoards.Get())
                str += (string)MotherBoard.GetPropertyValue("SerialNumber");
            foreach (var LogicalDisk in LogicalDisks.Get())
                str += (string)LogicalDisk.GetPropertyValue("VolumeSerialNumber");
            return GetHashString(str);
        }
        public static void SetFormMid(Form form)
        {
            // Center the Form on the user's screen everytime it requires a Layout.
            form.SetBounds((Screen.GetBounds(form).Width / 2) - (form.Width / 2),
                (Screen.GetBounds(form).Height / 2) - (form.Height / 2),
                form.Width, form.Height, BoundsSpecified.Location);
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        public static string RenameFile(string FileName)
        {
            FileName = FileName.Replace('<', ' ');
            FileName = FileName.Replace('>', ' ');
            FileName = FileName.Replace('/', ' ');
            FileName = FileName.Replace('\\', ' ');
            FileName = FileName.Replace('|', ' ');
            FileName = FileName.Replace(':', ' ');
            FileName = FileName.Replace('\"', ' ');
            FileName = FileName.Replace('*', ' ');
            FileName = FileName.Replace('?', ' ');
            return FileName;
        }
        public static void DownloadFile(object Info)
        {
            UtilValues.DownloadInfo downloadInfo = (UtilValues.DownloadInfo)Info;
            DataGridViewButtonCell ClickedCell = null;
            if (downloadInfo.type == UtilValues.DownloadFileType.BeatmapFile)
                ClickedCell = downloadInfo.ClickedCell;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            string CertificateFilePath = Directory.GetCurrentDirectory() + "\\Resources\\crt.cer";
            X509Certificate cert = new X509Certificate(CertificateFilePath);
            string MapFilePath = Settings.DefaultDownloadPath + "\\" + downloadInfo.FileName;
            float precent = 0f;
            if (ClickedCell != null)
            {
                ClickedCell.Value = "连接中...";
                ClickedCell.Tag = true;
            }
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(downloadInfo.URL);
                Myrq.Method = "get";
                Myrq.KeepAlive = false;
                Myrq.UserAgent = "SayobotBeatmapDownloader(ChingC ver.)";
                Myrq.ClientCertificates.Add(cert);
                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(MapFilePath, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    Application.DoEvents();
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, by.Length);

                    precent = (float)totalDownloadedByte / totalBytes * 100;
                    if (ClickedCell != null)
                    {
                        ClickedCell.Value = Convert.ToString(Math.Round(precent, 2)) + "%";
                        ClickedCell.Tag = true;
                    }
                    Application.DoEvents();
                }
                so.Close();
                st.Close();
                if (ClickedCell != null)
                {
                    ClickedCell.Value = "下载完成";
                    ClickedCell.Tag = true;
                }
                PublicControlers.notifyIcon.ShowBalloonTip(500, "SayobotBeatmapDownloader",
                    downloadInfo.FileName + " 下载完成", ToolTipIcon.Info);
                if (Settings.AutoOpen)
                {
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = MapFilePath;
                    processStartInfo.Arguments = "";
                    Process.Start(processStartInfo);
                }
                return;

            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show(downloadInfo.FileName + "\n" + "自动打开文件失败",
                    "SayobotBeatmapDownloader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (WebException ex)
            {
                MessageBox.Show(downloadInfo.FileName + "下载失败" + "\n" + ex.Message, "网络错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(MapFilePath))
                    File.Delete(MapFilePath);
                if (ClickedCell != null)
                {
                    ClickedCell.Value = "下载失败";
                    ClickedCell.Tag = true;
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(downloadInfo.FileName + "下载失败" + "\n" + ex.Message, "未知错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (File.Exists(MapFilePath))
                    File.Delete(MapFilePath);
                if (ClickedCell != null)
                {
                    ClickedCell.Value = "下载失败";
                    ClickedCell.Tag = true;
                }
                return;
            }
        }
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SystemDllFunctions.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
        public static void GetFilterValue()
        {
            foreach (Control control in FilterCheckBox.SearchType)
                UtilValues.FilterValue.SearchType +=
                    ((CheckBox)control).Checked ? Convert.ToInt32(control.Name.Substring(10)) : 0;
            foreach (Control control in FilterCheckBox.Mode)
                UtilValues.FilterValue.Mode +=
                    ((CheckBox)control).Checked ? Convert.ToInt32(control.Name.Substring(4)) : 0;
            foreach (Control control in FilterCheckBox.MapStatus)
                UtilValues.FilterValue.MapStatus +=
                    ((CheckBox)control).Checked ? Convert.ToInt32(control.Name.Substring(6)) : 0;
            foreach (Control control in FilterCheckBox.Genre)
                UtilValues.FilterValue.Genre +=
                    ((CheckBox)control).Checked ? Convert.ToInt32(control.Name.Substring(5)) : 0;
            foreach (Control control in FilterCheckBox.Language)
                UtilValues.FilterValue.Language +=
                    ((CheckBox)control).Checked ? Convert.ToInt32(control.Name.Substring(8)) : 0;
        }
        public static string Spawn_SearchHttpLink(string KeyWord, int offset)
        {
            StaticUtilFunctions.GetFilterValue();
            string sources = String.Format("https://api.sayobot.cn/beatmaplist?0=200&1={0}&2=4&3={1}", offset, KeyWord);
            sources += UtilValues.FilterValue.SearchType != 0 ? String.Format("&4={0}", UtilValues.FilterValue.SearchType) : null;
            sources += UtilValues.FilterValue.Mode != 0 ? String.Format("&5={0}", UtilValues.FilterValue.Mode) : null;
            sources += UtilValues.FilterValue.MapStatus != 0 ? String.Format("&6={0}", UtilValues.FilterValue.MapStatus) : null;
            sources += UtilValues.FilterValue.Genre != 0 ? String.Format("&7={0}", UtilValues.FilterValue.Genre) : null;
            sources += UtilValues.FilterValue.Language != 0 ? String.Format("&8={0}", UtilValues.FilterValue.Language) : null;
            /*
                sources += String.Format(
                "star:{0}~{1},AR:{2}~{3},OD:{3}~{4},CS:{5}~{6},HP:{7}~{8},length:{9}~{10},BPM:{11}~{12},end",
                UtilValues.FilterValue.Stars[0], UtilValues.FilterValue.Stars[1],
                UtilValues.FilterValue.AR[0], UtilValues.FilterValue.AR[1],
                UtilValues.FilterValue.OD[0], UtilValues.FilterValue.OD[1],
                UtilValues.FilterValue.CS[0], UtilValues.FilterValue.CS[1],
                UtilValues.FilterValue.HP[0], UtilValues.FilterValue.HP[1],
                UtilValues.FilterValue.Length[0], UtilValues.FilterValue.Length[1],
                UtilValues.FilterValue.BPM[0], UtilValues.FilterValue.BPM[1]);
            */
            return sources;
        }
        public static byte[] CalcSHA(byte[] source)
        {
            int length = source.Length + 1;
            ushort sum = 0;
            byte[] result = new byte[length];
            for (int index = 0; index < source.Length; index++)
            {
                sum += source[index];
                if (sum > 255) sum -= 256;
            }
            Array.Copy(source, result, source.Length);
            result[length - 1] = (byte)sum;
            return result;
        }
        public static void FindNanoPad()
        {
            if (UtilValues.PublicValue.nanoPads.Length != 0)
            {
                Array.Clear(UtilValues.PublicValue.nanoPads, 0, UtilValues.PublicValue.nanoPads.Length);
                Array.Resize(ref UtilValues.PublicValue.nanoPads, 0);
            }
            HIDDevice.interfaceDetails[] Pads = HIDDevice.getConnectedPads();
            foreach (HIDDevice.interfaceDetails Pad in Pads)
            {
                SayobotNanoPad pad = new SayobotNanoPad
                {
                    devicePath = Pad.devicePath,
                    ProductID = Pad.PID
                };
                pad.ReadingPadSettings();
                HIDDevice PadDevice = new HIDDevice(pad.devicePath, false);
                byte[] writeData = CalcSHA(new byte[] { 0x02, 0x00, 0x00 });
                PadDevice.write(writeData);
                System.Threading.Thread.Sleep(100);
                byte[] readData = PadDevice.read();
                if (readData[1] == 0)
                    pad.OSVersion = readData[4].ToString();
                else
                    pad.OSVersion = "Unknown";

                writeData = CalcSHA(new byte[] { 0x02, 0x08, 0x01, 0x00 });
                PadDevice.write(writeData);
                System.Threading.Thread.Sleep(100);
                readData = PadDevice.read();
                if (readData[0] != 0xff)
                {
                    byte[] nameEncode = new byte[readData[2]];
                    Array.Copy(readData, 3, nameEncode, 0, readData[2]);
                    nameEncode = ChangeToSystemUnicodeEncoding(nameEncode);
                    pad.Name = Encoding.Unicode.GetString(nameEncode);
                }
                else
                    pad.Name = Pad.product;

                PadDevice.close();

                Array.Resize(ref UtilValues.PublicValue.nanoPads, UtilValues.PublicValue.nanoPads.Length + 1);
                UtilValues.PublicValue.nanoPads[UtilValues.PublicValue.nanoPads.Length - 1] = pad;
            }
        }
        public static byte[] ChangeToSystemUnicodeEncoding(byte[] source)
        {
            byte[] result = new byte[source.Length];
            for (int i = 0; i < source.Length; i += 2)
            {
                result[i] = source[i + 1];
                result[i + 1] = source[i];
            }
            return result;
        }
    }
    /// <summary>
    /// 系统级函数
    /// </summary>
    public static class SystemDllFunctions
    {
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        [DllImport("kernel32")]

        public static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
    }
    /// <summary>
    /// 触盘信息
    /// </summary>
    public class SayobotNanoPad
    {
        public class PadKeys
        {
            public bool[] controlKeys { get; set; }
            public PadSetConstValue.Keys.CharKeys charKeys { get; set; }
        }
        public class PadLights
        {
            public PadSetConstValue.Light.Speed Speed { get; set; }
            public PadSetConstValue.Light.Mode Mode { get; set; }
            public PadSetConstValue.Light.ColorMode ColorMode { get; set; }
            public Color[] CustomColors { get; set; }
        }
        public NanoPadType DeviceType { get; set; }
        public string OSVersion { get; set; }
        public ushort ProductID { get; set; }
        public string devicePath { get; set; }
        public string Name { get; set; }
        public PadKeys[] padKeys { get; set; }
        public PadLights[] padLights { get; set; }

        public enum NanoPadType
        {
            O2 = 2,
        }
        public void ReadingPadSettings()
        {
            HIDDevice Pad = new HIDDevice(devicePath, false);
            //读取按键信息
            const int countKey = 6;
            padKeys = new PadKeys[countKey];
            for (byte keyNo = 0; keyNo < countKey; keyNo++)
            {
                byte[] sendInformation = { 02, 06, 02, 00, keyNo };
                Pad.write(StaticUtilFunctions.CalcSHA(sendInformation));
                System.Threading.Thread.Sleep(100);
                byte[] receiveInformation = Pad.read();
                byte ControlKey = receiveInformation[7];
                byte CharKey = receiveInformation[8];
                this.padKeys[keyNo].charKeys = (PadSetConstValue.Keys.CharKeys)CharKey;

                //解析Control按键
                for (byte exp = 7; exp >= 0; exp--)
                {
                    if (ControlKey >= (1 << exp))
                    {
                        ControlKey -= (byte)(1 << exp);
                        this.padKeys[keyNo].controlKeys.SetValue(true, exp);
                    }
                }

            }
            //读取灯光信息
            const int countLight = 5;
            padLights = new PadLights[countLight];
            for (byte lightNo = 0; lightNo < countLight; lightNo++)
            {
                byte[] sendInformation = { 02, 07, 02, 00, lightNo };
                Pad.write(StaticUtilFunctions.CalcSHA(sendInformation));
                System.Threading.Thread.Sleep(100);
                byte[] receiveInformation = Pad.read();
                byte LightData = receiveInformation[7];
                this.padLights[lightNo].Speed = (PadSetConstValue.Light.Speed)(LightData << 6);
                byte LightMode = (byte)(LightData - (LightData << 6));
                if (LightMode >= 0x3a)
                {
                    padLights[lightNo].Mode = PadSetConstValue.Light.Mode.Unknown;
                    padLights[lightNo].ColorMode = PadSetConstValue.Light.ColorMode.Unknown;
                    padLights[lightNo].CustomColors = new Color[0];
                }
                else
                {
                    var PadSystemLightModes = Enum.GetValues(typeof(PadSetConstValue.Light.Mode));
                    for (int tmp = PadSystemLightModes.Length; tmp > 0; tmp--)
                    {
                        if (LightMode > (byte)PadSystemLightModes.GetValue(tmp - 1))
                        {
                            padLights[lightNo].Mode = (PadSetConstValue.Light.Mode)PadSystemLightModes.GetValue(tmp - 1);
                            try
                            {
                                padLights[lightNo].ColorMode = (PadSetConstValue.Light.ColorMode)
                                    LightMode - (byte)padLights[lightNo].Mode;
                            }
                            catch (Exception)
                            {
                                padLights[lightNo].ColorMode = PadSetConstValue.Light.ColorMode.Unknown;
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// 公共值
    /// </summary>
    public class UtilValues
    {
        public enum DownloadFileType
        {
            BeatmapFile = 0,
            MusicFile,
            PicFile
        }
        public class DownloadInfo
        {
            public DownloadFileType type;
            public string URL;
            public string FileName;
            public DataGridViewButtonCell ClickedCell;
        }
        public class PublicValue
        {
            public static SayobotNanoPad[] nanoPads = new SayobotNanoPad[0];
            public static long ProgramRunTime;
            public readonly static string[] MapStatus =
                { "graveyard", "WIP", "pending", "ranked", "approved", "qualified", "loved" };
            public readonly static string[] Language =
                { "any", "其他", "英语", "日语", "汉语", "纯音乐", "韩语", "法语", "德语", "瑞典语", "西班牙语", "意大利语" };
            public readonly static string[] Genre =
                { "any", "未知", "游戏", "动漫", "摇滚", "流行", "其他", "新奇", "", "嘻哈", "电子" };
            public readonly static string ControlColorCode = String.Format("#{0:X2}{1:X2}{2:X2}",
                SystemColors.Control.R, SystemColors.Control.G, SystemColors.Control.B);
            public readonly static string HighlightColorCode = String.Format("#{0:X2}{1:X2}{2:X2}",
                        SystemColors.Highlight.R, SystemColors.Highlight.G, SystemColors.Highlight.B);
        }
        public static class FilterValue
        {
            public static Int32 SearchType, Mode, MapStatus, Genre, Language = 0;
            public static Int32[] Stars, AR, OD, CS, HP, Length, BPM = { 0, 0 };
        }
    }
    public static class PadDevUtil
    {
        public static byte[] StringToByte(string str)
        {
            string[] midStr = str.Split(' ');
            int a = midStr.Length + 1;
            int b, sum = 0;
            byte[] result = new byte[a];
            for (int i = 0; i < a - 1; i++)
            {
                b = Convert.ToInt32(midStr[i], 16);
                sum += b;
                if (sum > 255) sum -= 256;
                result[i] = Convert.ToByte(midStr[i], 16);
            }
            result[midStr.Length] = Convert.ToByte(sum.ToString(), 10);
            return result;
        }
        public static string ToHexString(this byte[] bytes)
        {
            string byteStr = string.Empty;
            byte[] data = new byte[bytes[2]];
            switch (bytes[1])
            {
                case 0x00:
                    Array.Copy(bytes, 3, data, 0, bytes[2]);
                    if (data != null || data.Length > 0)
                    {
                        foreach (var item in data)
                        {
                            byteStr += String.Format("{0:X2} ", item);
                        }
                    }
                    break;
                case 0x02 | 0x04 | 0x03:
                    Array.Copy(bytes, 3, data, 0, bytes[2]);
                    byteStr = System.Text.Encoding.ASCII.GetString(data);
                    break;
                default:
                    byteStr = "Data Error!" + " Error Code:" + bytes[1].ToString();
                    break;
            }
            return byteStr;
        }
    }

    //public static class ChildForms
    //{
    //    public static BeatmapDetailInfo beatmapDetailInfo;
    //    public static void LoadBeatmapDetailInfoForm(object Handler)
    //    {
    //        beatmapDetailInfo = new BeatmapDetailInfo();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //    public static ErrorReport errorReport;
    //    public static void LoadErrorReportForm(object Handler)
    //    {
    //        errorReport = new ErrorReport();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //    public static Filter filter;
    //    public static void LoadFilterForm(object Handler)
    //    {
    //        filter = new Filter();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //    public static PadDev padDev;
    //    public static void LoadPadDevForm(object Handler)
    //    {
    //        padDev = new PadDev();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //    public static PadSettings padSettings;
    //    public static void LoadPadSettingsForm(object Handler)
    //    {
    //        padSettings = new PadSettings();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //    public static SettingForm settings;
    //    public static void LoadSettingsForm(object Handler)
    //    {
    //        settings = new SettingForm();
    //        var handler = (EventWaitHandle)Handler;
    //        handler.Set();
    //    }
    //}

}
