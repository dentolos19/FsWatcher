using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using FsWatcher.Core.Models;
using Microsoft.Win32;

namespace FsWatcher.Graphics
{

    public partial class WnWatch
    {

        private bool _stopped;
        private readonly FileSystemWatcher[] _watchers;

        public WnWatch(string[] directories)
        {
            var watchers = new List<FileSystemWatcher>();
            foreach (var item in directories)
            {
                var watcher = new FileSystemWatcher { Path = item, IncludeSubdirectories = true };
                watcher.Created += delegate (object sender, FileSystemEventArgs args)
                {
                    var activityItem = new ActivityItem { Location = args.FullPath, Type = ActivityType.Created };
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Changed += delegate (object sender, FileSystemEventArgs args)
                {
                    var activityItem = new ActivityItem { Location = args.FullPath, Type = ActivityType.Modified };
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Renamed += delegate (object sender, RenamedEventArgs args)
                {
                    var activityItem = new ActivityItem { Location = args.FullPath, Type = ActivityType.Renamed };
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watcher.Deleted += delegate (object sender, FileSystemEventArgs args)
                {
                    var activityItem = new ActivityItem { Location = args.FullPath, Type = ActivityType.Removed };
                    Dispatcher.Invoke(() => { LvActivities.Items.Add(activityItem); });
                };
                watchers.Add(watcher);
            }
            _watchers = watchers.ToArray();
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs args)
        {
            foreach (var watcher in _watchers)
                watcher.EnableRaisingEvents = true;
        }

        private void Stop(object sender, RoutedEventArgs args)
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _stopped = true;
            if (MessageBox.Show("Do you want to save this activities to file?", "FsWatcher", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                Close();
            var dialog = new SaveFileDialog {Filter = "Activity Log File|*.log"};
            if (dialog.ShowDialog() == false)
                return;
            var activities = LvActivities.Items.OfType<ActivityItem>().ToArray();
            var writer = new StreamWriter(dialog.FileName);
            foreach (var activity in activities)
                writer.WriteLine($"{activity.Time} @ {activity.Location} ({activity.Type})");
            writer.Close();
            Close();
        }

        private void Pause(object sender, RoutedEventArgs args)
        {
            if (BnPause.Content.ToString() == "Pause")
            {
                foreach (var watcher in _watchers)
                    watcher.EnableRaisingEvents = false;
                BnPause.Content = "Resume";
            }
            else
            {
                foreach (var watcher in _watchers)
                    watcher.EnableRaisingEvents = true;
                BnPause.Content = "Pause";
            }
        }

        private void Halt(object sender, CancelEventArgs args)
        {
            if (_stopped)
                return;
            MessageBox.Show("Stop the monitor before closing this program!", "FsWatcher", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            args.Cancel = true;
        }

    }

}