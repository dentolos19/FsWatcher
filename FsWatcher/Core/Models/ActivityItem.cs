using System;

namespace FsWatcher.Core.Models
{

    public class ActivityItem
    {

        public ActivityItem(string path, ActivityType type)
        {
            Path = path;
            Type = type;
            Time = DateTime.Now;
        }

        public string Path { get; }

        public ActivityType Type { get; }

        public DateTime Time { get; }

    }

}