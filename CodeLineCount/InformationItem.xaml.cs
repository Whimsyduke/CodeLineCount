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
using System.Text.RegularExpressions;

namespace CodeLineCount
{
    /// <summary>
    /// InformationItem.xaml 的交互逻辑
    /// </summary>
    public partial class InformationItem : UserControl
    {
        public string FileText
        {
            get
            {
                return fileText;
            }

            set
            {
                fileText = value;
            }
        }
        private string fileText;

        public int TotalLines
        {
            get
            {
                return totalLines;
            }

            set
            {
                totalLines = value;
            }
        }        
        private int totalLines = 0;

        public int MineLines
        {
            get
            {
                return mineLines;
            }

            set
            {
                mineLines = value;
            }
        }
        private int mineLines = 0;

        public InformationItem()
        {
            InitializeComponent();
        }

        public InformationItem(FileInfo file, string parentPath)
        {
            InitializeComponent();
            LoadFile(file);
            Name.Content = file.Name;
            Type.Content = file.Extension;
            Path.Content = file.DirectoryName.Replace(parentPath, "");
            TotalLines = FileText.Split('\n').Count();
            Total.Content = TotalLines;
        }

        private void LoadFile(FileInfo file)
        {
            try
            {
                StreamReader streamReader = new StreamReader(file.FullName);
                fileText = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch
            {
                MessageBox.Show("无法打开文件" + file.FullName, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
