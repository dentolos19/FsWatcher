using System;

namespace FsWatcher.Core.Models
{

    public class ActivityItem
    {

        public string Location { get; set; }

        public ActivityType Type { get; set; }

        public DateTime Time { get; } = DateTime.Now;

    }

}