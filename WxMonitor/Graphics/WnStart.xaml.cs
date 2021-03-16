using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;
using AdonisMessageBox = AdonisUI.Controls.MessageBox;
using AdonisMessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using AdonisMessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using AdonisMessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace WxMonitor.Graphics
{

    public partial class WnStart
    {

        public WnStart()
        {
            InitializeComponent();
            RemoveButton.IsEnabled = false;
        }

        public string[] Directories { get; private set; }

        private void Start(object sender, RoutedEventArgs args)
        {
            if (!(DirectoryList.Items.Count >= 1))
            {
                AdonisMessageBox.Show("You must at least have one directory in the list!", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Stop);
                return;
            }
            Directories = DirectoryList.Items.OfType<string>().ToArray();
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void Add(object sender, RoutedEventArgs args)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            if (DirectoryList.Items.OfType<string>().Any(directory => directory == dialog.SelectedPath))
            {
                AdonisMessageBox.Show("The directory is already in the list.", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
                return;
            }
            DirectoryList.Items.Add(dialog.SelectedPath);
        }

        private void Remove(object sender, RoutedEventArgs args)
        {
            var answer = AdonisMessageBox.Show("Are you sure that you want remove this directory?", "WxMonitor", AdonisMessageBoxButton.YesNo, AdonisMessageBoxImage.Question);
            if (answer == AdonisMessageBoxResult.Yes)
                DirectoryList.Items.Remove(DirectoryList.SelectedItem);
        }

        private void Clear(object sender, RoutedEventArgs args)
        {
            if (!(DirectoryList.Items.Count >= 1))
                return;
            var answer = AdonisMessageBox.Show("Are you sure that you want to clear all directories?", "WxMonitor", AdonisMessageBoxButton.YesNo, AdonisMessageBoxImage.Question);
            if (answer == AdonisMessageBoxResult.Yes)
                DirectoryList.Items.Clear();
        }

        private void UpdateDirectorySelection(object sender, SelectionChangedEventArgs args)
        {
            RemoveButton.IsEnabled = DirectoryList.SelectedItem != null;
        }

    }

}