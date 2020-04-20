using System;

namespace FsWatcher.Core.Models
{

    public class ActivityItem
    {

        public string Path { get; set; }

        public ActivityType Type { get; set; }

        public DateTime Time { get; set; }

        public ActivityItem(string path, ActivityType type)
        {
            Path = path;
            Type = type;
            Time = DateTime.Now;
        }

    }

}