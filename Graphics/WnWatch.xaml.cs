using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using FsWatcher.Core.Models;
using Microsoft.Win32;

namespace FsWatcher.Graphics
{

    public partial class WnWatch
    {

        private readonly FileSystemWatcher[] _watchers;
        private List<ActivityItem> _activites;

        private bool _isClosing;
        private bool _isSaved;

        public WnWatch(IEnumerable<string> directories)
        {
            var watchers = new List<FileSystemWatcher>();
            foreach (var item in directories)
            {
                var watcher = new FileSystemWatcher { Path = item, IncludeSubdirectories = true };
                watcher.Created += delegate(object sender, FileSystemEventArgs e)
                {
                    var activityItem = new ActivityItem(e.FullPath, ActivityType.Created);
                    _activites.Add(activityItem);
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                {
                    var activityItem = new ActivityItem(e.FullPath, ActivityType.Modified);
                    _activites.Add(activityItem);
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Renamed += delegate(object sender, RenamedEventArgs e)
                {
                    var activityItem = new ActivityItem(e.FullPath, ActivityType.Renamed);
                    _activites.Add(activityItem);
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Deleted += delegate(object sender, FileSystemEventArgs e)
                {
                    var activityItem = new ActivityItem(e.FullPath, ActivityType.Deleted);
                    _activites.Add(activityItem);
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watchers.Add(watcher);
            }
            _watchers = watchers.ToArray();
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            _activites = new List<ActivityItem>();
            foreach (var watcher in _watchers)
                watcher.EnableRaisingEvents = true;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            foreach (var watcher in _watchers)
                watcher.Dispose();
            var result = MessageBox.Show("Do you want to save activities into file?", "FsWatcher", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var dialog = new SaveFileDialog { Filter = "FsWatcher Activity Log|*.log" };
                if (dialog.ShowDialog() == true)
                {
                    using var writer = new StreamWriter(dialog.FileName);
                    foreach (var item in _activites)
                        writer.WriteLine($"{item.Type}: {item.Path} @ {item.Time}");
                }
            }
            _isSaved = true;
            if (!_isClosing)
                Close();
        }

        private void ClosingStop(object sender, CancelEventArgs e)
        {
            if (_isSaved)
                return;
            _isClosing = true;
            Stop(null, null);
        }

    }

}