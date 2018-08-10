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
        string message;
        int notificationCount;
        public GenericObserver()
        {
            notificationCount = 0;
            message = "";
        }
        public string GetMessage()
        {
            return message;
        }

        public int GetNotificationCount()
        {
            return notificationCount;
        }

        public void Notify(string message)
        {
            notificationCount++;
            this.message = message;
        }

        public void Reset()
        {
            notificationCount = 0;
        }

    }
}

