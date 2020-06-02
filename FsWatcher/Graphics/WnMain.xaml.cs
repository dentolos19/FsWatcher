using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace FsWatcher.Graphics
{

    public partial class WnMain
    {

        public WnMain()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs args)
        {
            if (BnStart.IsEnabled == false)
                return;
            var directories = LbDirectories.Items.OfType<string>();
            new WnWatch(directories.ToArray()).Show();
            Close();
        }

        private void Exit(object sender, RoutedEventArgs args)
        {
            if (MessageBox.Show("Are you sure that you want to exit this program?", "FsWatcher", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Close();
        }

        private void Add(object sender, RoutedEventArgs args)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
                AddDir(dialog.SelectedPath);
            DirUpdate(null, null);
        }

        private void AddDir(string path)
        {
            var directories = LbDirectories.Items.OfType<string>();
            var isDuplicate = false;
            foreach (var directory in directories)
                if (path.StartsWith(directory))
                    isDuplicate = true;
            if (isDuplicate)
            {
                MessageBox.Show("This directory or parent directory is already added to the list!", "FsWatcher", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            LbDirectories.Items.Add(path);
        }

        private void Remove(object sender, RoutedEventArgs args)
        {
            if (BnRemove.IsEnabled == false)
                return;
            LbDirectories.Items.Remove(LbDirectories.SelectedItem);
            DirUpdate(null, null);
        }

        private void Clear(object sender, RoutedEventArgs args)
        {
            if (BnRemove.IsEnabled == false)
                return;
            LbDirectories.Items.Clear();
            DirUpdate(null, null);
        }

        private void DirUpdate(object sender, SelectionChangedEventArgs args)
        {
            BnRemove.IsEnabled = LbDirectories.SelectedItem != null;
            BnClear.IsEnabled = LbDirectories.Items.Count >= 1;
            BnStart.IsEnabled = LbDirectories.Items.Count >= 1;
        }

        private void DirDrop(object sender, DragEventArgs args)
        {
            if (!args.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            if (!(args.Data.GetData(DataFormats.FileDrop) is string[] items))
                return;
            foreach (var item in items)
                if (File.GetAttributes(item) == FileAttributes.Directory)
                    AddDir(item);
            DirUpdate(null, null);
        }

    }

}