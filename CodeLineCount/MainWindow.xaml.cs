using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CodeLineCount
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = Path.Text;
            if (!Directory.Exists(projectPath))
            {
                projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "选择代码路径";
            folderDialog.SelectedPath = projectPath;
            folderDialog.ShowDialog();
            if (folderDialog.SelectedPath != String.Empty)
                Path.Text = folderDialog.SelectedPath;
        }

        private void Count_Click(object sender, RoutedEventArgs e)
        {
            List.Items.Clear();
            if (!Directory.Exists(Path.Text))
            {
                MessageBox.Show("找不到路径对应的文件", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CheckFileInDirectory(new DirectoryInfo(Path.Text), Path.Text);

            int fileCount = 0, totalCount = 0, mineCount = 0;
            foreach (InformationItem select in List.Items)
            {
                fileCount++;
                totalCount += select.TotalLines;
                mineCount += select.MineLines;
            }
            Result.Content = "文件总数：" + fileCount.ToString() + "，总行数：" + totalCount.ToString() + "自己写行数：" + mineCount.ToString();
        }

        private void CheckFileInDirectory(DirectoryInfo dir, string parentPath)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo select in files)
            {
                if (select.Extension == ".cc" || select.Extension == ".h")
                    List.Items.Add(new InformationItem(select, parentPath));
            }
            DirectoryInfo[] folders = dir.GetDirectories();
            foreach (DirectoryInfo select in folders)
            {
                CheckFileInDirectory(select, parentPath);
            }
        }
    }
}
