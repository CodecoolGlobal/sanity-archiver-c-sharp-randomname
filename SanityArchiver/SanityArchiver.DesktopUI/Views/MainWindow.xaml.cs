namespace SanityArchiver.DesktopUI.Views
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object _dummyNode = null;

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
            LoadDrives();
        }

        private void LoadDrives()
        {
            foreach (string driveName in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = driveName;
                item.Tag = driveName;
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
                try
                {
                    foreach (string directoryName in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = directoryName.Substring(directoryName.LastIndexOf("\\") + 1);
                        subitem.Tag = directoryName;
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
            string temp2 = string.Empty;
            while (true)
            {
                string temp1 = temp.Header.ToString();
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
            GetDirectories();
            GetFiles();
        }

        private string[] GetDirectories()
        {
            string[] fileEntries = Directory.GetDirectories(SelectedImagePath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string fileName in fileEntries)
            {
                CreateListBoxItem(fileName, "dir");
            }

            return fileEntries;
        }

        private string[] GetFiles()
        {
            string[] fileEntries = Directory.GetFiles(SelectedImagePath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string fileName in fileEntries)
            {
                CreateListBoxItem(fileName, "file");
            }

            return fileEntries;
        }

        private void CreateListBoxItem(string fileName, string type)
        {
            ListBoxItem itm = new ListBoxItem();
            StackPanel panel = new StackPanel();
            itm.Content = panel;
            panel.Orientation = Orientation.Horizontal;
            TextBlock textBlock = new TextBlock();
            panel.Children.Add(textBlock);
            textBlock.Text = Path.GetFileName(fileName);
            textBlock.Width = 300;
            if (type.Equals("file"))
            {
                CheckBox checkBox = new CheckBox();
                panel.Children.Add(checkBox);
            }

            listBox.Items.Add(itm);
        }
    }
}
