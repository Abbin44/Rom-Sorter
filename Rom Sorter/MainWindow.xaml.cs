using System;
using System.Collections.Generic;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.IO;
using System.Linq;

namespace Rom_Sorter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            regionMenu.SelectedIndex = 0;
        }

        string filePath;
        private void browseDir_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderDlg = new WinForms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog.  
            WinForms.DialogResult result = folderDlg.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                filePath = folderDlg.SelectedPath;
                filePathBox.Text = filePath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void sortBtn_Click(object sender, RoutedEventArgs e)
        {
            if (filePathBox.Text.Length == 0)
            {
                MessageBox.Show("Please enter a file path", "Error");
                return;
            }

            if (regionMenu.SelectedItem == null)
            {
                MessageBox.Show("Please select a region", "Error");
                return;
            }

            string region = regionMenu.SelectedItem.ToString();

            switch (region)
            {
                case "Europe":
                    RunScan("E");
                    break;
                case "USA":
                    RunScan("U");
                    break;
                case "Japan":
                    RunScan("J");
                    break;
                case "Others":
                    RunScan("O");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            if (moveFilesChk.IsChecked == true)
                MoveFilesToRoot();
        }

        public void RunScan(string region)
        {
            List<string> regions = new List<string>();
            List<string> allRegs = new List<string>();

            bool other = false;
            bool keep = false;

            if (region == "E")
            {
                regions.Add("(E)");
                regions.Add("(EU)");
                regions.Add("(EUR)");
                regions.Add("(Europe)");
            }
            else if (region == "U")
            {
                regions.Add("(U)");
                regions.Add("(US)");
                regions.Add("(USA)");
            }
            else if (region == "J")
            {
                regions.Add("(J)");
                regions.Add("(JP)");
                regions.Add("(Japan)");
            }
            else if (region == "O")
            {
                other = true;
                allRegs.Add("(E)");
                allRegs.Add("(EU)");
                allRegs.Add("(EUR)");
                allRegs.Add("(Europe)");
                allRegs.Add("(U)");
                allRegs.Add("(US)");
                allRegs.Add("(USA)");
                allRegs.Add("(J)");
                allRegs.Add("(JP)");
                allRegs.Add("(Japan)");
            }

            filePath = filePathBox.Text;
            DirectoryInfo di = new DirectoryInfo(filePath);

            foreach (FileInfo file in di.EnumerateFiles())
            {
                if (other == false)
                {
                    keep = regions.Any(file.Name.Contains);
                    if (keep == false)
                        file.Delete();
                }
                else if (other == true)
                {
                    keep = allRegs.Any(file.Name.Contains);
                    if (keep == true)
                        file.Delete();
                }
            }
        }

        public void MoveFilesToRoot()
        {
            string name = string.Empty;
            int index = 0;
            foreach (string file in Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories))
            {
                index = file.LastIndexOf(@"\");
                if (index > 0)
                    name = file.Substring(index, file.Length - index);

                File.Move(file, filePath + name);
            }
        }
    }
}
