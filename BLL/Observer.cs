using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using System.Collections.Specialized;
using System.Drawing;

namespace BLL
{
    public class Observer
    {
        int notificationCount;
        SortedList<int, string> orderedNotification;
        public long frameCount { get; set; }
        public Observer()
        {
            notificationCount = 0;
            orderedNotification = new SortedList<int, string>();
        }
 
        public KeyValuePair<int,string>? GetReport()
        {
 
            if (orderedNotification==null || orderedNotification.Count() == 0) return null;
            KeyValuePair<int, string> result = new KeyValuePair<int, string>(orderedNotification.ElementAt(0).Key, orderedNotification.ElementAt(0).Value);
            orderedNotification.RemoveAt(0);
            return result;
        }

        public int GetNotificationCount()
        {
            return notificationCount;
        }

         public void Notify(int imageNumber, string rectangles)
        {
            orderedNotification.Add(imageNumber, rectangles);
            notificationCount++;

        }

        public void Reset()
        {
            notificationCount = 0;
            orderedNotification = new SortedList<int, string>();
        }

    }
}

