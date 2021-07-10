using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEditor
{
    public partial class frmMain : Form
    {
        string folderPath;
        int allSelectedFilesNr;
        List<string> filesPath;
        string saveAtPath;
        string imgPath;
        string currentFileName;
        string position;

        string localFolder;
        string localSaveFilesAt;
        List<string> localFiles = new List<string>();

        private void btnAddFolderToEditInto1_Click(object sender, EventArgs e)
        {
            string selectedFolder = validateFolderForNrOfFilesAndPath();
            if (!string.IsNullOrEmpty(selectedFolder))
                localFolder = selectedFolder;
        }

        private void btnAddManualFilesToEdit1_Click(object sender, EventArgs e)
        {
            var selectedFiles = validateMultipleFiles();
            if (selectedFiles != null && selectedFiles.Any())
            {
                localFiles.Clear();
                foreach (var file in selectedFiles)
                {
                    localFiles.Add(file);
                }
            }
        }

        private void btnSaveFilesAt1_Click(object sender, EventArgs e)
        {
            string selectedFolder = validateFoldersForPath();
            if (!string.IsNullOrEmpty(selectedFolder))
                localSaveFilesAt = selectedFolder;
        }

        private void btnAddImageToVideo_Click(object sender, EventArgs e)
        {
            string imagePath = validateImagesFolder();
            imgPath = imagePath;

            if (!string.IsNullOrEmpty(imagePath))
            {
                Bitmap b = new Bitmap(imagePath);
                Image finalImage = resizeImage(b, new Size(72, 75));
                imgSelectedPhoto.Image = finalImage;
            }
        }

        private void cbImagesWithPercentage_Click(object sender, EventArgs e)
        {
            lblStart_Images.Visible = cbImagesWithPercentage.Checked;
            lblEnd_Images.Visible = cbImagesWithPercentage.Checked;
            txtPercentageStart.Visible = cbImagesWithPercentage.Checked;
            txtPercentageEnd.Visible = cbImagesWithPercentage.Checked;
            lblPercentStart_Images.Visible = cbImagesWithPercentage.Checked;
            lblPercentEnd_Images.Visible = cbImagesWithPercentage.Checked;
        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (Image)b;
        }

        public string validateFolderToEdit()
        {
            if (string.IsNullOrEmpty(localFolder))
            {
                var folderAtGeneral = checkForGeneralPath(0);
                if (string.IsNullOrEmpty(folderAtGeneral))
                    return null;
                else
                    return folderAtGeneral;
            }
            else
                return localFolder;
        }

        public List<string> validateFilesToEdit()
        {
            if (localFiles.Any())
            {
                return localFiles;
            }
            else
                return null;
        }

        public string validateSaveFilesAt()
        {
            if (string.IsNullOrEmpty(localSaveFilesAt))
            {
                var folderAtGeneral = checkForGeneralPath(2);
                if (string.IsNullOrEmpty(folderAtGeneral))
                    return null;
                else
                    return folderAtGeneral;
            }
            else
                return localSaveFilesAt;
        }

        public string validateAddedImage()
        {
            if (string.IsNullOrEmpty(imgPath))
            {
                var imagePath = checkForGeneralPath(4);
                if (string.IsNullOrEmpty(imagePath))
                    return null;
                else
                    return imagePath;
            }
            return imgPath; //RETURN "" IF THE IMAGE IS ADDED AT VIDEOS PAGE
        }

        private void ddPosition_Images_SelectedIndexChanged(object sender, EventArgs e)
        {
            position = ddPosition_Images.SelectedItem.ToString();
        }


        // START EDITING
        private void btnEditForImages_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                MessageBox.Show("Please wait until current operation completes");
                return;
            }
            backgroundWorker1.RunWorkerAsync();
        }

        public int fromTimeToSeconds(string time)
        {
            int hour = int.Parse(time.Substring(0, 2));
            int minutes = int.Parse(time.Substring(3, 2));
            int seconds = int.Parse(time.Substring(6, 2));
            return (hour * 60 * 60) + (minutes * 60) + seconds;
        }

        void addImageInVideos(object sender, DoWorkEventArgs e)
        {  
            folderPath = validateFolderToEdit();
            filesPath = validateFilesToEdit();
            saveAtPath = validateSaveFilesAt();
            string imagePath = validateAddedImage();

            if (folderPath != null)
                allSelectedFilesNr += Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") ||
                s.EndsWith(".wmv") || s.EndsWith(".mov") || s.EndsWith(".avi") || s.EndsWith(".flv")).Count();

            if (localFiles != null)
                allSelectedFilesNr += localFiles.Count;


            var start = dpStartAt_Images.Value.ToString("HH:mm:ss");
            var end = dpEndAt_Images.Value.ToString("HH:mm:ss");
            int startSeconds;
            int endSeconds;
            if (cbImagesWithPercentage.Checked)
            {
                startSeconds = int.Parse(txtPercentageStart.Text);
                endSeconds = int.Parse(txtPercentageEnd.Text);
            }
            else
            {
                startSeconds = fromTimeToSeconds(start);
                endSeconds = fromTimeToSeconds(end);
            }
            
            //VALIDATE FOLDER, FILES, SAVE AT, IMAGE
            if (filesPath == null && folderPath == null) { MessageBox.Show("No Files or Folder Selected\nPlease select an folder with videos or some files to edit"); return; }
            if(string.IsNullOrEmpty(saveAtPath)) { MessageBox.Show("You didn't select any folder where to save the files\nPlease select any folder"); return; }
            if(imagePath == null) { MessageBox.Show("No Image added\nPlease select any Image to add in videos"); return; }

            //ADD SELECTED FILES FROM LIST TO STRING
            string filesPathString = "";
            if(filesPath != null)
            {
                foreach (var file in filesPath)
                {
                    filesPathString += file + ",seperated,";
                }
                filesPathString = filesPathString.Remove(filesPathString.Length - 11);
            }

            string python = LocateEXE("python.exe");
            if (python == null)
            {
                MessageBox.Show("Couldn't find Python, Python is needed for this application to work.\nPlease Install Python 3.8 Version");
                return;
            }
                
            var arguments = $"-u \"{editVideoScript}\" \"{folderPath}\" \"{filesPathString}\" \"{saveAtPath}\" \"{imagePath}\" \"{position}\" \"{startSeconds}\" \"{endSeconds}\" \"{cbImagesWithPercentage.Checked}\"";

            Python.initPythonScript(python, arguments);

            Python.process.OutputDataReceived += Process_OutputDataReceived;
            Python.process.ErrorDataReceived += Process_ErrorDataReceived;

            clearProgressBar();
            Python.execute();
            clearLocalFields();
            MessageBox.Show("Video Editings Completed");
        }

        private void clearLocalFields()
        {
            allSelectedFilesNr = 0;
            localFiles.Clear();
            localFolder = "";
            localSaveFilesAt = "";
        }

        public void clearProgressBar()
        {
            Invoke(new Action(() =>
            {
                lblVideosCompleted_Images.Text = "";
                lblPercentCompleted_Images.Text = "";
                lblVideoInfos_Images.Text = "";
            }));
            backgroundWorker1.ReportProgress(0);
        }

        public void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                if (e.Data.StartsWith("Duration:") || e.Data.StartsWith("frame="))
                {
                    Invoke(new Action(() =>
                    {
                        if (currentFileName != null && currentFileName.Length > 16)
                            currentFileName = currentFileName.Substring(0, 15) + "...";
                        lblVideoInfos_Images.Text = $"File:{currentFileName}, {e.Data.Split('q')[0]}, {e.Data.Substring(e.Data.IndexOf("time"), 13)}";
                    }));
                }
            //MessageBox.Show(e.Data);
        }

        public void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.StartsWith("current-video:"))
                    currentFileName = e.Data.Split(':')[1];
                if (e.Data.StartsWith("videos-completed:"))
                {
                    var data = e.Data;
                    int currentVideoNr = int.Parse(e.Data.Split(':')[1]);
                    Invoke(new Action(() =>
                    {
                        lblVideosCompleted_Images.Text = $"({currentVideoNr} of {allSelectedFilesNr}) Videos Edited";
                    }));

                    if(currentVideoNr != 0)
                    {
                        var percentCompleted = (decimal)currentVideoNr / allSelectedFilesNr * 100;
                        backgroundWorker1.ReportProgress(Convert.ToInt32(percentCompleted));
                        Invoke(new Action(() =>
                        {
                            lblPercentCompleted_Images.Text = $"{Convert.ToInt32(percentCompleted)}% Completed";
                        }));
                    }   
                }
                //MessageBox.Show(e.Data);
            }
        }

        
        // Back on the 'UI' thread so we can update the progress bar
        void addImageInVideos_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgForImages.Value = e.ProgressPercentage;
        }
    }
}
