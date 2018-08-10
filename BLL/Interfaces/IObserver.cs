using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IObserver
    {
        void Notify(string message);
        string GetMessage();
        int GetNotificationCount();
        void Reset();
    }
}
