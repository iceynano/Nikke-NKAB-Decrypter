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
using System.Diagnostics;

namespace Nikke_NKAB_Decrypter
{
    public partial class GUI : Form
    {
        bool _IsBusy = false;
        public GUI()
        {
            InitializeComponent();
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            try
            {
                tbDirPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "com_proximabeta", "NIKKE", "eb");
            }
            catch { }
        }

        private void btnDirSelect_Click(object sender, EventArgs e)
        {
            string folderPath = Utils.FolderBrowser("eb");
            if (!string.IsNullOrEmpty(folderPath))
            {
                tbDirPath.Text = folderPath;
            }
        }

        private void btnExportSelect_Click(object sender, EventArgs e)
        {
            string folderPath = Utils.FolderBrowser("Decrypt path");
            if (!string.IsNullOrEmpty(folderPath))
            {
                tbExportPath.Text = folderPath;
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (!_IsBusy && tbDirPath.Text.Trim().Length > 0 && tbExportPath.Text.Trim().Length > 0) 
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        string[] files = Directory.GetFiles(tbDirPath.Text, "*", SearchOption.TopDirectoryOnly);
                        for (int i = 0;i < files.Length; i++)
                        {
                            Decrypter.DecryptV3(files[i], Path.Combine(tbExportPath.Text, Path.GetFileName(files[i]) + ".bundle"));
                            int percent = (int)Math.Ceiling((double)(i * 100) / files.Length);
                            progressBar.BeginInvoke((MethodInvoker)delegate
                            {
                                progressBar.Value = percent >= 100 ? 100 : percent;
                            });

                        }
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        MessageBox.Show(err.Message, "Error");
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; progressBar.Value = 100; });
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (!_IsBusy && tbExportPath.Text.Trim().Length > 0 && tbEncryptPath.Text.Trim().Length > 0)
            {
                _IsBusy = true;
                Task.Run(() =>
                {
                    try
                    {
                        string[] files = Directory.GetFiles(tbExportPath.Text, "*.bundle", SearchOption.TopDirectoryOnly);
                        for (int i = 0; i < files.Length; i++)
                        {
                            Decrypter.EncryptV3(files[i], Path.Combine(tbEncryptPath.Text, Path.GetFileNameWithoutExtension(files[i])));
                            int percent = (int)Math.Ceiling((double)(i * 100) / files.Length);
                            progressBar.BeginInvoke((MethodInvoker)delegate
                            {
                                progressBar.Value = percent >= 100 ? 100 : percent;
                            });

                        }
                    }
                    catch (Exception err)
                    {
                        _IsBusy = false;
                        MessageBox.Show(err.Message, "Error");
                    }
                }).GetAwaiter().OnCompleted(() => { _IsBusy = false; progressBar.Value = 100; });
            }
        }

        private void btnEncryptSelect_Click(object sender, EventArgs e)
        {
            string folderPath = Utils.FolderBrowser("Encrypt path");
            if (!string.IsNullOrEmpty(folderPath))
            {
                tbEncryptPath.Text = folderPath;
            }
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/lehieugch68/Nikke-NKAB-Decrypter");
        }
    }
}
