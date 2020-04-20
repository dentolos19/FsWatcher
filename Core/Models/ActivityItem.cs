using System;

namespace FsWatcher.Core.Models
{

    public class ActivityItem
    {

        public string Path { get; }

        public ActivityType Type { get; }

        public DateTime Time { get; }

        public ActivityItem(string path, ActivityType type)
        {
            Path = path;
            Type = type;
            Time = DateTime.Now;
        }

    }

}