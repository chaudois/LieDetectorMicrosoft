using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Interfaces
{
    public interface IVideoConverter
    {
        void ProcessPicture(string pathPicture,string execDirectory);
        void addMiddleWare(IProgress<string> middleWare); 
    }
}