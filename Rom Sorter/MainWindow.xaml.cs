using System;
using System.Collections.Generic;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Rom_Sorter
{
    public partial class MainWindow : Window
    {
        Color color;
        Brush foreColor;
        string filePath;
        string extFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Extentions.ext";
        bool? searchRecursivley = false;
        bool deleteFiles = true;

        public MainWindow()
        {
            InitializeComponent();
            color = (Color)ColorConverter.ConvertFromString("#FF0197FF");//Very janky but very beautiful convertions here
            foreColor = new SolidColorBrush(color);
            regionMenu.SelectedIndex = 0;
            foreColor.Opacity = 0.3f;
            extTextBox.Foreground = foreColor;
        }

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
            string region = regionMenu.SelectedValue.ToString();
            string[] tokens = region.Split(':');
            region = tokens[1].Trim();
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
            //TODO: Only search stored file extentions
            List<string> regions = new List<string>();
            List<string> allRegs = new List<string>();

            bool other = false;
            bool keep = false;

            searchRecursivley = RecursiveSearchChk.IsChecked;
            filePath = filePathBox.Text;
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

            string[] files;
            if (searchRecursivley == true)
                files = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(filePath, "*", SearchOption.TopDirectoryOnly);

            string[] extentions = File.ReadAllLines(extFilePath);
            string[] tokens;
            bool isROM = false;
            for (int i = 0; i < extentions.Length; ++i)
            {
                tokens = extentions[i].Split(';');
                extentions[i] = tokens[0];
            }

            for (int i = 0; i < files.Length; ++i)
            {
                if (other == false)
                {
                    for (int j = 0; j < extentions.Length; j++)
                    {
                        if (files[i].Contains(extentions[j]))
                        {
                            keep = regions.Any(files.Contains);
                            if (keep == false)
                                File.Delete(files[i]);
                        }
                        else
                            continue;
                    }
                }
                else if (other == true)
                {
                    for (int j = 0; j < extentions.Length; j++)
                    {
                        if (files[i].Contains(extentions[j]))
                        {
                            keep = allRegs.Any(files.Contains);
                            if (keep == true)
                                File.Delete(files[i]);
                        }
                        else
                            continue;
                    }
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

        public void AddExtention(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(extTextBox.Text) || extTextBox.Text == ".example")
            {
                MessageBox.Show("The extention field is empty", "Error");
                return;
            }
            string extention = extTextBox.Text;

            StringBuilder sbe = new StringBuilder();
            sbe.Append("\n");
            if (!extention.StartsWith("."))
                sbe.Append(".");

            sbe.Append(extention);
            sbe.Append(";");
            File.AppendAllText(extFilePath, sbe.ToString());
            extTextBox.Text = string.Empty;
        }

        private void extTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            extTextBox.Text = string.Empty;
            foreColor.Opacity = 1.0f;
            extTextBox.Foreground = foreColor;
        }

        private void OpenExtentionFile(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", extFilePath);
        }
    }
}
