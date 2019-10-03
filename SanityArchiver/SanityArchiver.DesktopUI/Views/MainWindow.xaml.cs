namespace SanityArchiver.DesktopUI.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object _dummyNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SelectedItemPath = string.Empty;
        }

        private string SelectedItemPath { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(_dummyNode);
                item.Expanded += new RoutedEventHandler(Folder_Expanded);
                FoldersItem.Items.Add(item);
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == _dummyNode)
            {
                item.Items.Clear();
                var folders = Directory.GetDirectories(item.Tag.ToString())
                    .Where(d => !new DirectoryInfo(d).Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System));

                try
                {
                    foreach (string s in folders)
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(_dummyNode);
                        subitem.Expanded += new RoutedEventHandler(Folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void FoldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = (TreeViewItem)tree.SelectedItem;

            if (temp == null)
            {
                return;
            }

            SelectedItemPath = string.Empty;
            string temp1 = string.Empty;
            string temp2 = string.Empty;
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = string.Empty;
                }

                SelectedItemPath = (temp1 + temp2 + SelectedItemPath).ToString();
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }

                temp = (TreeViewItem)temp.Parent;
                temp2 = @"\";
            }

            listBox.Items.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(SelectedItemPath);
            FileInfo[] fileEntries = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo fileName in fileEntries)
            {
                if (!fileName.Attributes.HasFlag(FileAttributes.Hidden) || !fileName.Attributes.HasFlag(FileAttributes.System))
                {
                    ListBoxItem itm = new ListBoxItem();
                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = fileName.Name;
                    textBlock.Width = 200;
                    TextBlock creationDateText = new TextBlock();
                    creationDateText.Text = fileName.CreationTime.ToString("yyyy.MM.dd");
                    creationDateText.Width = 100;
                    TextBlock size = new TextBlock();
                    size.Text = GetBytesReadable(fileName.Length);
                    size.Width = 100;
                    CheckBox checkBox = new CheckBox();
                    panel.Children.Add(textBlock);
                    panel.Children.Add(creationDateText);
                    panel.Children.Add(size);
                    panel.Children.Add(checkBox);
                    itm.Content = panel;
                    listBox.Items.Add(itm);
                }
            }
        }

        private string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }

            readable = (readable / 1024);
            return readable.ToString("0.### ") + suffix;
        }
    }
}
