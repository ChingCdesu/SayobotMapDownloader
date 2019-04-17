using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace mapdownloader
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] processes = Process.GetProcessesByName("mapdownloader.exe");
            if (processes.Length > 1)
            {
                MessageBox.Show("SayobotBeatmapDownloader已经在运行", "SayobotBeatmapDownloader", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(-2);
            }
            #if DEBUG
            Debug();
            #else
            Release();
            #endif
        }
        static void Debug()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            StaticUtilFunctions.SetFormMid(mainForm);
            Application.Run(mainForm);
        }
        static void Release()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();
            StaticUtilFunctions.SetFormMid(mainForm);
            SystemInfo.OSVersion = Environment.OSVersion.VersionString;
            SystemInfo.DeviceUniqueCode = StaticUtilFunctions.GetPCUniqueCode();
            try
            {
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                string osVersion = "OSVersion: " + Environment.OSVersion.VersionString;
                string AppVersion = "AppVersion: " + Application.ProductVersion.ToString();

                var str = osVersion + "\r\n\r\n" + AppVersion + "\r\n\r\n" 
                    + string.Format("Type: {0}\r\n\r\nMessage: {1}\r\n\r\nInformation: \r\n{2}",
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                if (!Directory.Exists(Environment.CurrentDirectory + "\\ErrorLog"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\ErrorLog");
                var LogFileName = Environment.CurrentDirectory + "\\ErrorLog\\" + SystemInfo.DeviceUniqueCode
                    + " " + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".log";
                using (var sw = new StreamWriter(LogFileName, true))
                {
                    sw.Write(str);
                    sw.Close();
                }
                ErrorReport errorReporter = new ErrorReport();
                StaticUtilFunctions.SetFormMid(errorReporter);
                errorReporter.ErrorLogFilePath = LogFileName;
                errorReporter.ErrorLogInformation = str;
                errorReporter.ShowDialog();
            }
        }
    }
}
