using System;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace mapdownloader
{
    public partial class ErrorReport : mapdownloader.BaseForm
    {
        public ErrorReport()
        {
            InitializeComponent();
        }
        public string ErrorLogFilePath;
        public string ErrorLogInformation;
        public static Stream FileToStream(string fileName)

        {
            // 打开文件

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            // 读取文件的 byte[]

            byte[] bytes = new byte[fileStream.Length];

            fileStream.Read(bytes, 0, bytes.Length);

            fileStream.Close();

            // 把 byte[] 转换成 Stream

            Stream stream = new MemoryStream(bytes);

            return stream;

        }
        private void ErrorReport_Load(object sender, EventArgs e)
        {
            StaticUtilFunctions.SetFormMid(this);
            textBox1.Text = ErrorLogInformation;
            pictureBox1.Image = Image.FromFile(Environment.CurrentDirectory + "\\Resources\\Pics\\error.png");
        }
        private string Upload(string url, string filepath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 5000;
            Stream postStream = new MemoryStream();

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

            //准备文件流
            using (var fileStream = FileToStream(filepath))
            {
                var formdata = string.Format(formdataTemplate, "", System.IO.Path.GetFileName(filepath) /*Path.GetFileName(fileName)*/);
                var formdataBytes = Encoding.UTF8.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//第一行不需要换行
                postStream.Write(formdataBytes, 0, formdataBytes.Length);

                //写入文件
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    postStream.Write(buffer, 0, bytesRead);
                }
            }
            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            postStream.Write(footer, 0, footer.Length);
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            if (postStream != null)
            {
                postStream.Position = 0;

                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//关闭文件访问
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            catch (WebException)
            {
                DialogResult result = 
                    MessageBox.Show("上传失败，错误日志保存到本地", "SayobotBeatmapDownloader", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Retry)
                    return Upload(url, filepath);
                else
                    return "failed";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (Upload("http://218.92.17.254:6002/", ErrorLogFilePath) != "success")
                File.Delete(ErrorLogFilePath);
            PublicControlers.notifyIcon.Dispose();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PublicControlers.notifyIcon.Dispose();
            Application.Exit();
        }
    }
}
