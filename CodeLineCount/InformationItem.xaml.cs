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
        enum CodeType
        {
            If,
            Ifdef,
            Ifndef,
            Else,
            Endif
        }
        private class Sentence
        {

            public int Index
            {
                get
                {
                    return index;
                }

                set
                {
                    index = value;
                }
            }
            private int index;

            public string Code
            {
                get
                {
                    return code;
                }

                set
                {
                    code = value;
                }
            }
            private string code;

            public CodeType CodeType
            {
                get
                {
                    return codeType;
                }

                set
                {
                    codeType = value;
                }
            }
            private CodeType codeType;

            public Sentence IfSentence
            {
                get
                {
                    return ifSentence;
                }

                set
                {
                    ifSentence = value;
                }
            }
            private Sentence ifSentence;

            public Sentence ElseSentence
            {
                get
                {
                    return elseSentence;
                }

                set
                {
                    elseSentence = value;
                }
            }
            private Sentence elseSentence;

            public Sentence EndIfSentence
            {
                get
                {
                    return endIfSentence;
                }

                set
                {
                    endIfSentence = value;
                }
            }
            private Sentence endIfSentence;

            public Sentence(int index, string code, CodeType type)
            {
                Index = index;
                Code = code;
                CodeType = type;

                switch (type)
                {
                    case InformationItem.CodeType.If:
                    case InformationItem.CodeType.Ifdef:
                    case InformationItem.CodeType.Ifndef:
                        IfSentence = this;
                        break;
                    case InformationItem.CodeType.Else:
                        ElseSentence = this;
                        break;
                    case InformationItem.CodeType.Endif:
                        EndIfSentence = this;
                        break;
                    default:
                        break;
                }
            }
        }
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

            string[] codes = FileText.Split('\n');
            Stack<Sentence> defines = new Stack<Sentence>();
            List<Sentence> ifs = new List<Sentence>();
            for (int i = 0; i < codes.Count(); i++)
            {
                if (codes[i].Contains("#ifdef"))
                {
                    Sentence sentence = new Sentence(i, codes[i], CodeType.Ifdef);
                    defines.Push(sentence);
                    ifs.Add(sentence);
                }
                else if (codes[i].Contains("#ifndef"))
                {
                    Sentence sentence = new Sentence(i, codes[i], CodeType.Ifndef);
                    defines.Push(sentence);
                    ifs.Add(sentence);
                }
                else if (codes[i].Contains("#if"))
                {
                    Sentence sentence = new Sentence(i, codes[i], CodeType.If);
                    defines.Push(sentence);
                    ifs.Add(sentence);
                }
                else if (codes[i].Contains("#else"))
                {
                    Sentence sentence = new Sentence(i, codes[i], CodeType.Else);
                    defines.Peek().ElseSentence = sentence;
                }
                else if (codes[i].Contains("#endif"))
                {
                    Sentence sentence = new Sentence(i, codes[i], CodeType.Endif);
                    defines.Pop().EndIfSentence = sentence;
                }
            }
            
            foreach (Sentence select in ifs)
            {
                if (select.Code.Contains("PMTMGMP_UNUSED_MY_CODE"))
                {
                    switch (select.CodeType)
                    {
                        case CodeType.If:
                            if (select.ElseSentence != null)
                            {
                                MineLines += select.EndIfSentence.Index - select.ElseSentence.Index + 2;
                            }
                            else
                            {
                                MineLines += 2;
                            }
                            break;
                        case CodeType.Ifdef:
                            if (select.ElseSentence != null)
                            {
                                MineLines += select.EndIfSentence.Index - select.ElseSentence.Index + 2;
                            }
                            else
                            {
                                MineLines += 2;
                            }
                            break;
                        case CodeType.Ifndef:
                            if (select.ElseSentence != null)
                            {
                                MineLines += select.ElseSentence.Index - select.IfSentence.Index + 2;
                            }
                            else
                            {
                                MineLines += select.EndIfSentence.Index - select.IfSentence.Index + 1;
                            }
                            break;
                        case CodeType.Else:
                            break;
                        case CodeType.Endif:
                            break;
                        default:
                            break;
                    }
                }
            }
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
