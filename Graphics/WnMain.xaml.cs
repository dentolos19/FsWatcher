using System.IO;
using System.Linq;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace FsWatcher.Graphics
{

    public partial class WnMain
    {

        public WnMain()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (LbDirectories.Items.Count == 0)
            {
                MessageBox.Show("You need to add input directories before starting!", "FsWatcher");
                return;
            }
            var directories = LbDirectories.Items.OfType<string>().ToArray();
            new WnWatching(directories).Show();
            Close();
        }

        private void AddDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
                LbDirectories.Items.Add(dialog.SelectedPath);
        }

        private void RemoveDirectory(object sender, RoutedEventArgs e)
        {
            if (LbDirectories.SelectedItem != null)
                LbDirectories.Items.Remove(LbDirectories.SelectedItem);
        }

        private void ClearDirectories(object sender, RoutedEventArgs e)
        {
            LbDirectories.Items.Clear();
        }

        private void DropDirectory(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            var items = e.Data.GetData(DataFormats.FileDrop) as string[];
            foreach (var item in items)
            {
                if (File.GetAttributes(item) == FileAttributes.Directory)
                    LbDirectories.Items.Add(item);
            }
        }

    }

}