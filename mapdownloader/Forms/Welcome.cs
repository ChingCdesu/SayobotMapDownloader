using System;
using System.IO;
using System.Drawing;

namespace mapdownloader.Forms
{
    public partial class Welcome : mapdownloader.BaseForm
    {
        public Welcome()
        {
            StaticUtilFunctions.SetFormMid(this);
            InitializeComponent();
        }

        private void RoundButton2_Click(object sender, EventArgs e)
        {
            int imageNo = Convert.ToInt32(label4.Text);
            if (imageNo == 0)
                return;
            else
                label4.Text = (imageNo - 1).ToString();
        }

        private void RoundButton1_Click(object sender, EventArgs e)
        {
            int imageNo = Convert.ToInt32(label4.Text);
            if (imageNo == welcomeImages.Length - 1)
                return;
            else
                label4.Text = (imageNo + 1).ToString();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            this.RoundButton1_Click(sender, e);
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            this.RoundButton2_Click(sender, e);
        }
        private readonly string[] welcomeImages = 
            Directory.GetFiles(Environment.CurrentDirectory + "\\Resources\\Pics\\Welcome");

        private void Welcome_Load(object sender, EventArgs e)
        {
            label3.Text = "1 / " + welcomeImages.Length;
            label4.Text = "0";
        }

        private void Label4_TextChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(welcomeImages[Convert.ToInt32(label4.Text)]);
        }
    }
}