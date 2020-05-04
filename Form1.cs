using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace $safeprojectname$
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> cleanFileList = new List<string>();
            label1.Text = "wybierz folder z plikiem .sln";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label1.Text = fbd.SelectedPath;
                string folderPath = fbd.SelectedPath;
                string[] files = Directory.GetFiles(folderPath, "*.sln", SearchOption.TopDirectoryOnly);
                if (files.Length != 0)
                {
                    foreach (var item in files)
                    {
                        cleanFileList.Add(item);
                    }

                    string file1 = files[0];
                    label1.Text = file1;
                    Array.Clear(files, 0, files.Length);

                    
                    var csProj = Directory.GetFiles(folderPath, "*.csproj", SearchOption.AllDirectories);
                    foreach (string currentFile in csProj)
                    {
                        cleanFileList.Add(currentFile);
                    }
                    
                    var fileLines = from file in Directory.EnumerateFiles(folderPath, "*.csproj", SearchOption.AllDirectories)
                                    from line in File.ReadLines(file)
                                    where line.Contains("Compile Include")
                                    select new
                                    {
                                        File = file,
                                        Line = line
                                    };
                    List<string> fileList = new List<string>();

                    
                    foreach (var item in fileLines)
                    {
                        Console.WriteLine($"{item.File}\t{item.Line}");
                        fileList.Add(item.Line);

                    }
                    Console.WriteLine($"{fileLines.Count().ToString()} file lines found");

                    
                    string temp =  "";
                    int find = 0;
                    foreach (var item in fileList)
                    {
                        temp = item;
                        find = temp.IndexOf("\"");
                        temp = temp.Substring(find+1);
                        find = temp.IndexOf("\"");
                        temp = temp.Substring(0, find);
                        Console.WriteLine(item + " => " + temp);
                        cleanFileList.Add(Path.GetDirectoryName(cleanFileList[1]) + "\\" +temp);
                    }

                    string copyDirectory = fbd.SelectedPath + "\\KOPIA";
                    Directory.CreateDirectory(copyDirectory);
                    foreach (var item in cleanFileList)
                    {
                        File.Copy(item, copyDirectory + "\\" + Path.GetFileName(item), true);
                    }
                    ZipFile.CreateFromDirectory(copyDirectory, copyDirectory + ".zip", CompressionLevel.Fastest, true);
                    
                }
                else
                {
                    label1.Text = "w podanym katalogu nie ma pliku .sln";
                }
            }
        }
    }
}
