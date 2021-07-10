using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoEditor
{
    public partial class frmMain : Form
    {
        string textPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"../GeneralFiles.txt"));

        private void btnAddVideosFolderToEditGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateFolderForNrOfFilesAndPath();
            if(!string.IsNullOrEmpty(filePath))
                saveGeneralFilesToTextFile(0 + filePath);
        }

        private void btnAddAudiosFolderToEditGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateFolderForNrOfFilesAndPathAudio();
            if (!string.IsNullOrEmpty(filePath))
                saveGeneralFilesToTextFile(1 + filePath);
        }

        private void btnSaveEditedVideosAtGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateFoldersForPath();
            if (!string.IsNullOrEmpty(filePath))
                saveGeneralFilesToTextFile(2 + filePath);
        }

        private void btnSaveEditedAudiosAtGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateFoldersForPath();
            if (!string.IsNullOrEmpty(filePath))
                saveGeneralFilesToTextFile(3 + filePath);
        }

        private void btnAddImageGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateImagesFolder();
            if (!string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Image added successfully");
                saveGeneralFilesToTextFile(4 + filePath);
            }   
        }

        private void btnAddSoundGeneral_Click(object sender, EventArgs e)
        {
            string filePath = validateAudiosFolder();
            if (!string.IsNullOrEmpty(filePath))
                saveGeneralFilesToTextFile(5 + filePath);
        }

        public string validateFoldersForPath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    MessageBox.Show("Success, the folder is added");
                }
                return fbd.SelectedPath;
            }
        }

        public string validateFolderForNrOfFilesAndPath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    int nrOfFilesInFolder = Directory.EnumerateFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4") ||
                    s.EndsWith(".wav") || s.EndsWith(".wmv") || s.EndsWith(".mov") || s.EndsWith(".flv")).Count();

                    if (nrOfFilesInFolder <= 0)
                    {
                        MessageBox.Show("The folder should have minimum 1 file inside");
                        return null;
                    }
                    else
                    {
                        MessageBox.Show($"Success, Folder with {nrOfFilesInFolder} files is added");
                    }
                    //allSelectedFilesNr += nrOfFilesInFolder;
                }
                return fbd.SelectedPath;
            }
        }

        public string validateFolderForNrOfFilesAndPathAudio()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    int nrOfFilesInFolder = Directory.EnumerateFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp3") ||
                    s.EndsWith(".wav") || s.EndsWith(".aac")).Count();

                    if (nrOfFilesInFolder <= 0)
                    {
                        MessageBox.Show("The folder should have minimum 1 file inside");
                        return null;
                    }
                    else
                    {
                        MessageBox.Show($"Success, Folder with {nrOfFilesInFolder} files is added");
                    }
                    //allSelectedFilesNrAudio += nrOfFilesInFolder;
                }
                return fbd.SelectedPath;
            }
        }

        public string validateImagesFolder()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG;)|*.BMP;*.JPG;*.PNG|All files (*.*)|*.*";
                var result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    if (ofd.FileName.EndsWith(".bmp") || ofd.FileName.EndsWith(".jpg") || ofd.FileName.EndsWith(".png"))
                        return ofd.FileName;
                    else
                    {
                        MessageBox.Show("Format not supported!\nOnly Images...");
                        return null;
                    }
                }
                return ofd.FileName;
            }
        }

        public string validateAudiosFolder()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.MP3;*.WAV;*.AAC;)|*.MP3;*.WAV;*.AAC|All files (*.*)|*.*";
                var result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    if (ofd.FileName.EndsWith(".mp3") || ofd.FileName.EndsWith(".wav") || ofd.FileName.EndsWith(".aac"))
                        MessageBox.Show("Audio added successfully");
                    else
                    {
                        MessageBox.Show("Format not supported!\nOnly Audios...");
                        return null;
                    }
                }
                return ofd.FileName;
            }
        }

        public List<string> validateMultipleFiles()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Video Files(*.MP4;*.WMV;*.MOV;*.AVI;*.FLV)|*.MP4;*.WMV;*.MOV;*.AVI;*.FLV|All files (*.*)|*.*";
                var result = ofd.ShowDialog();
                if (result == DialogResult.OK && ofd.FileNames.Length != 0)
                {
                    List<string> files = new List<string>();
                    foreach (var file in ofd.FileNames)
                    {
                        if (ofd.FileName.EndsWith(".mp4") || ofd.FileName.EndsWith(".wmv") || ofd.FileName.EndsWith(".mov") || ofd.FileName.EndsWith(".avi") || ofd.FileName.EndsWith(".flv")) ;
                        files.Add(file);
                    }
                    //allSelectedFilesNr += ofd.FileNames.Length;
                    if (files.Count == 0)
                        MessageBox.Show("Format not supported!\nOnly Videos...");
                    else
                    {
                        if (files.Count == 1)
                            MessageBox.Show($"{files.Count } video selected");
                        else
                            MessageBox.Show($"{files.Count } videos selected");
                    }
                    return files;
                }
                return null;
            }
        }

        public List<string> validateMultipleFilesAudio()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Video Files(*.MP3;*.AAC;*.WAV)|*.MP3;*.AAC;*.WAV|All files (*.*)|*.*";
                var result = ofd.ShowDialog();
                if (result == DialogResult.OK && ofd.FileNames.Length != 0)
                {
                    List<string> files = new List<string>();
                    foreach (var file in ofd.FileNames)
                    {
                        if (ofd.FileName.EndsWith(".mp3") || ofd.FileName.EndsWith(".aac") || ofd.FileName.EndsWith(".wav"))
                            files.Add(file);
                    }
                    //allSelectedFilesNrAudio += ofd.FileNames.Length;
                    if (files.Count == 0)
                        MessageBox.Show("Format not supported!\nOnly Audios...");
                    else
                    {
                        if (files.Count == 1)
                            MessageBox.Show($"{files.Count } audios selected");
                        else
                            MessageBox.Show($"{files.Count } audios selected");
                    }
                    return files;
                }
                return null;
            }
        }

        public void saveGeneralFilesToTextFile(string filePath)
        {
            string[] lines = File.ReadAllLines(textPath);
            string tempString = "";
            if (lines.Length == 0)
                File.AppendAllText(textPath, filePath + Environment.NewLine);
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] != "")
                    {
                        if (lines[i].Substring(0, 1) == filePath.Substring(0, 1))
                            tempString += filePath + "\n";
                        else
                            tempString += lines[i] + "\n";
                    }
                }
                File.WriteAllText(textPath, tempString + Environment.NewLine);
            }
        }

        public string checkForGeneralPath(int pathType)
        {
            string[] lines = File.ReadAllLines(textPath);
            foreach (var line in lines)
            {
                if (line.StartsWith(pathType.ToString()) && !line.EndsWith("!"))
                    return line.Substring(1, line.Length - 1);
            }
            return null;
        }

        private void btnGeneralClear_Click(object sender, EventArgs e)
        {
            File.WriteAllText(textPath, "0!\n1!\n2!\n3!\n4!");
            MessageBox.Show("Done");
        }
    }
}
