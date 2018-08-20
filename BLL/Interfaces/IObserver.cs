using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IObserver
    {
        void Notify(string rapport);
        string GetReport();
        int GetNotificationCount();
        void Reset();
    }
}
