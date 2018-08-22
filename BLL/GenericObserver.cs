using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;

namespace BLL
{
    public class GenericObserver : IObserver
    {
        string rapport;
        int notificationCount;
        public GenericObserver()
        {
            notificationCount = 0;
        }
        public string GetReport()
        {
            return rapport;
        }

        public int GetNotificationCount()
        {
            return notificationCount;
        }

        public void Notify(string newRapport)
        {
            notificationCount++;
            rapport = newRapport == null?rapport: newRapport;
        }

        public void Reset()
        {
            notificationCount = 0;
        }

    }
}

