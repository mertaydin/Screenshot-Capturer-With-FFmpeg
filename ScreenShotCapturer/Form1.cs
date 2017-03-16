using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace ScreenShotCapturer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        string sFileName = "";
        string justFileName = "";
        ArrayList capturedScreenshots = new ArrayList();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            string formats = "All Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
            choofdlog.Filter = formats;

            choofdlog.FilterIndex = 1;
            //choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                sFileName = choofdlog.FileName;
                justFileName = choofdlog.SafeFileName;
                axWindowsMediaPlayer1.URL = sFileName;
                //string[] arrAllFiles = choofdlog.FileNames; //çoklu seçim için.
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();            
        }



        void captureScreenShot(int start, int end, int incNumber, string filePath, string fileName, int videoDuration)
        {
            string secondToFormat;          
            string command;

            for (int i = start; i < end; i+=incNumber)
            {
                Console.WriteLine(i.ToString() + " fmpeg worked...");
                secondToFormat = string.Format("{0:00}:{1:00}:{2:00}", i / 3600, (i / 60) % 60, i % 60);
                //command = " -y -i " + filePath.ToString() + " -acodec: copy -vcodec: copy -threads 0 -ss " + secondToFormat + " -vprofile high -preset ultrafast output_" + fileName + "_" + i + "_.jpg";
                command = " -threads 0  -ss " + secondToFormat + " -y -i " + filePath.ToString() + " -frames 1 -q:v 1  output_" + fileName + "_" + i + "_.jpg";
                FFmpegConversion(command);
                Console.WriteLine("screenshot Captured" + secondToFormat.ToString());
                listBox1.Items.Add("Screenshot Captured - " + " output_" + fileName + "_" + i + ".jpg");
                capturedScreenshots.Add("output_" + fileName + "_" + i + ".jpg");
                Console.WriteLine(command.ToString());
            }

        }

        static void FFmpegConversion(string command)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ffmpeg\\bin\\ffmpeg.exe";
            proc.StartInfo.Arguments = command.ToString();
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            if (!proc.Start())
            {
                Console.WriteLine("Error!");
                return;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            //MessageBox.Show("Bitti!");
            proc.Close();
        }

        private int getVideoDuration(string filePath)
        {
            if (filePath != "")
                return Convert.ToInt32(axWindowsMediaPlayer1.Ctlcontrols.currentItem.duration);
            else
                return 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            pictureBox1.Image = Image.FromFile((capturedScreenshots[selectedIndex]).ToString());            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            listBox1.Items.Clear();
            capturedScreenshots.Clear();

            int videoDuration = getVideoDuration(sFileName);

            var checkedButton = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);

            if (checkedButton != null && Convert.ToBoolean(videoDuration))
            {
                switch (checkedButton.Name.ToString())
                {
                    case "radioButton1":
                        captureScreenShot(1, Convert.ToInt32(textBox4.Text), 2, sFileName, justFileName, videoDuration);
                        break;
                    case "radioButton2":
                        captureScreenShot(0, Convert.ToInt32(textBox5.Text), 2, sFileName, justFileName, videoDuration);
                        break;
                    case "radioButton3":
                        captureScreenShot(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), sFileName, justFileName, videoDuration);
                        break;
                    case "radioButton4":
                        captureScreenShot(Convert.ToInt32(textBox6.Text), (Convert.ToInt32(textBox6.Text) + 1), 1, sFileName, justFileName, videoDuration);
                        break;
                    default:
                        MessageBox.Show("There is no selection");
                        break;
                }
            }
            else
            {
                MessageBox.Show("There is no selection");
            }
        }
    }
}
