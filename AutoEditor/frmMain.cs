using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEditor
{
    public partial class frmMain : Form
    {
        string editVideoScript = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../../Python-Scripts/")) + "edit_video.py";
        string editAudioScript = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../../../Python-Scripts/")) + "edit_audio.py";

        private string LocateEXE(string filename)
        {
            string path = Environment.GetEnvironmentVariable("path");
            string[] folders = path.Split(';');
            foreach (string folder in folders)
            {
                if (File.Exists(folder + filename) && folder.Contains("Python38"))
                {
                    return folder + filename;
                }
                else if (File.Exists(folder + "\\" + filename))
                {
                    return folder + "\\" + filename;
                }
            }
            return null;
        }

        public frmMain()
        {
            InitializeComponent();
            this.Location = new Point(280, 130);

            // To report progress from the background worker we need to set this property
            backgroundWorker1.WorkerReportsProgress = true;
            // This event will be raised on the worker thread when the worker starts
            backgroundWorker1.DoWork += new DoWorkEventHandler(addImageInVideos);
            // This event will be raised when we call ReportProgress
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(addImageInVideos_ProgressChanged);

            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.DoWork += new DoWorkEventHandler(editAudios);
            backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(editAudios_ProgressChanged);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            dpStartAt_Images.Format = DateTimePickerFormat.Custom;
            dpStartAt_Images.CustomFormat = "HH:mm:ss";
            dpStartAt_Images.ShowUpDown = true;
            dpStartAt_Images.Value = new DateTime(2015, 1, 15);

            dpEndAt_Images.Format = DateTimePickerFormat.Custom;
            dpEndAt_Images.CustomFormat = "HH:mm:ss";
            dpEndAt_Images.ShowUpDown = true;
            dpEndAt_Images.Value = new DateTime(2015, 1, 15);

            dpTrimStart.Format = DateTimePickerFormat.Custom;
            dpTrimStart.CustomFormat = "HH:mm:ss";
            dpTrimStart.ShowUpDown = true;
            dpTrimStart.Value = new DateTime(2015, 1, 15);

            dpTrimEnd.Format = DateTimePickerFormat.Custom;
            dpTrimEnd.CustomFormat = "HH:mm:ss";
            dpTrimEnd.ShowUpDown = true;
            dpTrimEnd.Value = new DateTime(2015, 1, 15);

            ddBitrate.SelectedIndex = 1;
        }

        private void btnVideosNav_Click(object sender, EventArgs e)
        {
            bunifuPager.SetPage(1);
        }

        private void btnGeneralNav_Click(object sender, EventArgs e)
        {
            bunifuPager.SetPage(0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public string escapeWhiteSpaces(string text)
        {
            string finalText = "";
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var character in text)
                {
                    if (character == ' ')
                        finalText += "\\' ";
                    else
                        finalText += character;
                }
                return finalText;
            }
            return finalText;
        }

        private void btnAudiosNav_Click(object sender, EventArgs e)
        {
            bunifuPager.SetPage(2);
        }
    }
}
