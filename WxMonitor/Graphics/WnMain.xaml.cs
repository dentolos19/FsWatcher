using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using WxMonitor.Core.Bindings;
using AdonisMessageBox = AdonisUI.Controls.MessageBox;
using AdonisMessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using AdonisMessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using AdonisMessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace WxMonitor.Graphics
{

    public partial class WnMain
    {

        private bool _isPaused;

        private bool _isRunning;
        private FileSystemWatcher[] _watchers;

        public WnMain()
        {
            InitializeComponent();
            StopButton.IsEnabled = false;
            RemoveButton.IsEnabled = false;
        }

        private void Start(object sender, RoutedEventArgs args)
        {
            if (_isRunning)
            {
                if (_isPaused)
                {
                    foreach (var watcher in _watchers)
                        watcher.EnableRaisingEvents = true;
                    StartButton.Content = "Pause Monitoring";
                    _isPaused = false;
                }
                else
                {
                    foreach (var watcher in _watchers)
                        watcher.EnableRaisingEvents = false;
                    StartButton.Content = "Resume Monitoring";
                    _isPaused = true;
                }
                return;
            }
            var dialog = new WnStart { Owner = this };
            if (dialog.ShowDialog() != true)
                return;
            var watchers = new List<FileSystemWatcher>();
            foreach (var directory in dialog.Directories)
            {
                var watcher = new FileSystemWatcher { Path = directory, IncludeSubdirectories = true };
                watcher.Created += (from, parameters) =>
                {
                    var item = new ActivityItemBinding { FileName = Path.GetFileName(parameters.FullPath), FileLocation = parameters.FullPath, ActionType = "Created", ActivityTime = DateTime.Now };
                    Dispatcher.Invoke(() => { ActivityList.Items.Add(item); });
                };
                watcher.Changed += (from, parameters) =>
                {
                    var item = new ActivityItemBinding { FileName = Path.GetFileName(parameters.FullPath), FileLocation = parameters.FullPath, ActionType = "Modified", ActivityTime = DateTime.Now };
                    Dispatcher.Invoke(() => { ActivityList.Items.Add(item); });
                };
                watcher.Renamed += (from, parameters) =>
                {
                    var item = new ActivityItemBinding { FileName = Path.GetFileName(parameters.FullPath), FileLocation = parameters.FullPath, ActionType = "Renamed", ActivityTime = DateTime.Now };
                    Dispatcher.Invoke(() => { ActivityList.Items.Add(item); });
                };
                watcher.Deleted += (from, parameters) =>
                {
                    var item = new ActivityItemBinding { FileName = Path.GetFileName(parameters.FullPath), FileLocation = parameters.FullPath, ActionType = "Removed", ActivityTime = DateTime.Now };
                    Dispatcher.Invoke(() => { ActivityList.Items.Add(item); });
                };
                watchers.Add(watcher);
            }
            _watchers = watchers.ToArray();
            foreach (var watcher in _watchers)
                watcher.EnableRaisingEvents = true;
            StartButton.Content = "Pause Monitoring";
            StopButton.IsEnabled = true;
            _isRunning = true;
        }

        private void Stop(object sender, RoutedEventArgs args)
        {
            if (!_isRunning)
                return;
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _isPaused = false;
            _isRunning = false;
            StartButton.Content = "Start Monitoring";
            StopButton.IsEnabled = false;
        }

        private void Remove(object sender, RoutedEventArgs args)
        {
            var answer = AdonisMessageBox.Show("Are you sure that you want to remove this monitored activity?", "WxMonitor", AdonisMessageBoxButton.YesNo, AdonisMessageBoxImage.Question);
            if (answer == AdonisMessageBoxResult.Yes)
                ActivityList.Items.Remove(ActivityList.SelectedItem);
        }

        private void Clear(object sender, RoutedEventArgs args)
        {
            if (!(ActivityList.Items.Count >= 1))
                return;
            var answer = AdonisMessageBox.Show("Are you sure that you want to clear all monitored activities?", "WxMonitor", AdonisMessageBoxButton.YesNo, AdonisMessageBoxImage.Question);
            if (answer == AdonisMessageBoxResult.Yes)
                ActivityList.Items.Clear();
        }

        private void Save(object sender, RoutedEventArgs args)
        {
            if (_isRunning)
            {
                AdonisMessageBox.Show("You can't save while the monitor is running!", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
                return;
            }
            if (!(ActivityList.Items.Count >= 1))
            {
                AdonisMessageBox.Show("You must at least have one monitored activity in the list.", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
                return;
            }
            var dialog = new SaveFileDialog { Filter = "WxMonitor Log|*.fsl" };
            if (dialog.ShowDialog() != true)
                return;
            var items = ActivityList.Items.OfType<ActivityItemBinding>();
            var data = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(dialog.FileName, data);
            AdonisMessageBox.Show("Saved activities into file!", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
        }

        private void Load(object sender, RoutedEventArgs args)
        {
            if (_isRunning)
            {
                AdonisMessageBox.Show("You can't load while the monitor is running!", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
                return;
            }
            var dialog = new OpenFileDialog { Filter = "WxMonitor Log|*.fsl" };
            if (dialog.ShowDialog() != true)
                return;
            var data = File.ReadAllText(dialog.FileName);
            var items = JsonConvert.DeserializeObject<IEnumerable<ActivityItemBinding>>(data);
            ActivityList.Items.Clear();
            foreach (var item in items)
                ActivityList.Items.Add(item);
            AdonisMessageBox.Show("Loaded activities from file!", "WxMonitor", AdonisMessageBoxButton.OK, AdonisMessageBoxImage.Information);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateActivitySelection(object sender, SelectionChangedEventArgs args)
        {
            RemoveButton.IsEnabled = ActivityList.SelectedItem != null;
        }

        private void CopyFileLocation(object sender, RoutedEventArgs args)
        {
            if (ActivityList.SelectedItem != null)
                Clipboard.SetText(((ActivityItemBinding)ActivityList.SelectedItem).FileLocation);
        }

    }

}