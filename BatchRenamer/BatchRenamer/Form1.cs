using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BatchRenamer
{
    public partial class Form1 : Form
    {
        private string _rootPath;
        private int _renameCount;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult res = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _rootPath = fbd.SelectedPath;
                    txtRootPath.Text = fbd.SelectedPath;

                    lblNoFiles.Text = FileCount().ToString();
                    lblExtensions.Text = String.Join(",", GetFileExtensions(GetFiles(_rootPath)).Distinct());
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rootPath))
            {
                MessageBox.Show("Set root path.", Application.ProductName, MessageBoxButtons.OK);
                txtRootPath.Focus();

                return;
            }

            if (string.IsNullOrEmpty(txtNewName.Text))
            {
                MessageBox.Show("Set new file name.", Application.ProductName, MessageBoxButtons.OK);
                txtNewName.Focus();

                return;
            }

            if (!RenameFiles(txtNewName.Text, GetFiles(_rootPath)))
            {
                MessageBox.Show("Something happened.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Successfully renamed {_renameCount} files.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearValues();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ClearValues()
        {
            _rootPath = "";
            _renameCount = 0;
            txtRootPath.Text = "";
            txtNewName.Text = "";
        }

        private int FileCount()
        {
            return GetFiles(_rootPath).Length + 1;
        }

        private string[] GetFiles(string rootPath)
        {
            string[] files = Directory.GetFiles(rootPath);

            return files;
        }

        private List<string> GetFileExtensions(string[] filePaths)
        {
            List<string> extensions = new List<string>();

            filePaths.ToList().ForEach(file => extensions.Add(Path.GetExtension(file)));

            return extensions;
        }

        private bool RenameFiles(string fileName, string[] files)
        {
            int start = 1;

            foreach (var file in files)
            {
                string newTitle = fileName + start.ToString("D2");
                string directoryName = Path.GetDirectoryName(file);
                string newFile = Path.Combine(directoryName, newTitle + Path.GetExtension(file));

                try
                {
                    File.Move(file, newFile);
                    _renameCount++;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }

                start++;
            }

            return true;
        }
    }
}
