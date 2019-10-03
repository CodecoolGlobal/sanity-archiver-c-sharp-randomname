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
            SelectedImagePath = string.Empty;
        }

        private string SelectedImagePath { get; set; }

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

            SelectedImagePath = string.Empty;
            string temp1 = string.Empty;
            string temp2 = string.Empty;
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = string.Empty;
                }

                SelectedImagePath = (temp1 + temp2 + SelectedImagePath).ToString();
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }

                temp = (TreeViewItem)temp.Parent;
                temp2 = @"\";
            }

            listBox.Items.Clear();

            var fileEntries = Directory.GetFileSystemEntries(SelectedImagePath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => !new FileInfo(f).Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System));
            foreach (var fileName in fileEntries)
            {
                ListBoxItem itm = new ListBoxItem();
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                TextBlock textBlock = new TextBlock();
                textBlock.Text = Path.GetFileName(fileName);
                textBlock.Width = 300;
                CheckBox checkBox = new CheckBox();
                panel.Children.Add(textBlock);
                panel.Children.Add(checkBox);
                itm.Content = panel;
                listBox.Items.Add(itm);
            }
        }
    }
}
