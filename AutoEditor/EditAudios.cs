using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEditor
{
    public partial class frmMain : Form
    {
        string folderPathAudio;
        int allSelectedFilesNrAudio;
        List<string> filesPathAudio;
        string saveAtPathAudio;
        string currentFileNameAudio = "";

        string localFolderAudio;
        string localSaveFilesAtAudio;
        List<string> localFilesAudio = new List<string>();


        private void btnAddFolderToEditInto2_Click(object sender, EventArgs e)
        {
            string selectedFolder = validateFolderForNrOfFilesAndPathAudio();
            if (!string.IsNullOrEmpty(selectedFolder))
                localFolderAudio = selectedFolder;
        }

        private void btnAddManualFilesToEdit2_Click(object sender, EventArgs e)
        {
            var selectedFiles = validateMultipleFilesAudio();
            if (selectedFiles != null && selectedFiles.Any())
            {
                localFilesAudio.Clear();
                foreach (var file in selectedFiles)
                {
                    localFilesAudio.Add(file);
                }
            }
        }

        private void btnSaveFilesAt2_Click(object sender, EventArgs e)
        {
            string selectedFolder = validateFoldersForPath();
            if (!string.IsNullOrEmpty(selectedFolder))
                localSaveFilesAtAudio = selectedFolder;
        }

        public string validateFolderToEditAudio()
        {
            if (string.IsNullOrEmpty(localFolderAudio))
            {
                var folderAtGeneral = checkForGeneralPath(1);
                if (string.IsNullOrEmpty(folderAtGeneral))
                    return null;
                else
                    return folderAtGeneral;
            }
            else
                return localFolderAudio;
        }

        public List<string> validateFilesToEditAudio()
        {
            if (localFilesAudio.Any())
            {
                return localFilesAudio;
            }
            else
                return null;
        }

        public string validateSaveFilesAtAudio()
        {
            if (string.IsNullOrEmpty(localSaveFilesAtAudio))
            {
                var folderAtGeneral = checkForGeneralPath(3);
                if (string.IsNullOrEmpty(folderAtGeneral))
                    return null;
                else
                    return folderAtGeneral;
            }
            else
                return localSaveFilesAtAudio;
        }


        // START EDITING
        private void btnEditAudio_Click(object sender, EventArgs e)
        {
            if (backgroundWorker2.IsBusy)
            {
                MessageBox.Show("Please wait until current operation completes");
                return;
            }
            backgroundWorker2.RunWorkerAsync();
        }

        public void editAudios(object sender, DoWorkEventArgs e)
        {
            folderPathAudio = validateFolderToEditAudio();
            filesPathAudio = validateFilesToEditAudio();
            saveAtPathAudio = validateSaveFilesAtAudio();

            if (folderPathAudio != null)
                allSelectedFilesNrAudio += Directory.EnumerateFiles(folderPathAudio, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp3") ||
                s.EndsWith(".aac") || s.EndsWith(".wav")).Count();

            if (localFilesAudio != null)
                allSelectedFilesNrAudio += localFilesAudio.Count;

            var trimStart = dpTrimStart.Value.ToString("HH:mm:ss");
            var trimEnd = dpTrimEnd.Value.ToString("HH:mm:ss");

            //VALIDATE FOLDER, FILES, SAVE AT, IMAGE
            if (filesPathAudio == null && folderPathAudio == null) { MessageBox.Show("No Files or Folder Selected\nPlease select an folder with Audios or multiple Audios to edit"); return; }
            if (string.IsNullOrEmpty(saveAtPathAudio)) { MessageBox.Show("You didn't select any folder where to save the files\nPlease select any folder"); return; }

            //ADD SELECTED FILES FROM LIST TO STRING
            string filesPathString = "";
            if (filesPathAudio != null)
            {
                foreach (var file in filesPathAudio)
                {
                    filesPathString += file + ",seperated,";
                }
                filesPathString = filesPathString.Remove(filesPathString.Length - 11);
            }

            Regex onlyNumbers = new Regex(@"\d+");
            Regex onlyDecimals = new Regex(@"\d+(\.\d+)?");
            Match m = null;
            Invoke(new Action(() =>
            {
                m = onlyNumbers.Match(ddBitrate.SelectedItem.ToString());
            }));
            var bitrate = m.Value;
            var speed = onlyDecimals.Match(txtAudioSpeed.Text).Value;

            string python = LocateEXE("python.exe");
            if(python == null)
            {
                MessageBox.Show("Couldn't find Python, Python is needed for this application to work.\nPlease Install Python 3.8 Version");
                return;
            }

            var arguments = $"-u \"{editAudioScript}\" \"{folderPathAudio}\" \"{filesPathString}\" \"{saveAtPathAudio}\" \"{bitrate}\" \"{trimStart}\" \"{trimEnd}\" \"{speed}\"";

            Python.initPythonScript(python, arguments);

            Python.process.OutputDataReceived += ProcessAudio_OutputDataReceived;
            Python.process.ErrorDataReceived += ProcessAudio_ErrorDataReceived;

            clearProgressBarAudio();
            Python.execute();
            clearLocalFieldsAudio();
            MessageBox.Show("Audio Editings Completed");
        }

        private void clearLocalFieldsAudio()
        {
            allSelectedFilesNrAudio = 0;
            localFilesAudio.Clear();
            localFolderAudio = "";
            localSaveFilesAtAudio = "";
        }

        public void ProcessAudio_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                if (e.Data.StartsWith("Duration:") || e.Data.StartsWith("size="))
                {
                    Invoke(new Action(() =>
                    {
                        if (currentFileNameAudio.Length > 15)
                            currentFileNameAudio = currentFileNameAudio.Substring(0, 15) + "...";
                        lblAudioInfos.Text = $"File:{currentFileNameAudio}, {e.Data.Split('B')[1]}";
                    }));
                }
            //MessageBox.Show(e.Data);
        }

        public void ProcessAudio_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.StartsWith("current-audio:"))
                    currentFileNameAudio = e.Data.Split(':')[1];
                else if (e.Data.StartsWith("audios-completed:"))
                {
                    var data = e.Data;
                    int currentVideoNr = int.Parse(e.Data.Split(':')[1]);
                    Invoke(new Action(() =>
                    {
                        lblAudiosCompleted.Text = $"({currentVideoNr} of {allSelectedFilesNrAudio}) Audios Edited";
                    }));

                    if (currentVideoNr != 0)
                    {
                        var percentCompleted = (decimal)currentVideoNr / allSelectedFilesNrAudio * 100;
                        backgroundWorker2.ReportProgress(Convert.ToInt32(percentCompleted));
                        Invoke(new Action(() =>
                        {
                            lblPercentCompleted_Audios.Text = $"{Convert.ToInt32(percentCompleted)}% Completed";
                        }));
                    }
                }
                //MessageBox.Show(e.Data);
            }
        }

        public void clearProgressBarAudio()
        {
            Invoke(new Action(() =>
            {
                lblAudiosCompleted.Text = "";
                lblPercentCompleted_Audios.Text = "";
                lblAudioInfos.Text = "";
            }));
            backgroundWorker2.ReportProgress(0);
        }


        // Back on the 'UI' thread so we can update the progress bar
        void editAudios_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgAudios.Value = e.ProgressPercentage;
        }
    }
}
