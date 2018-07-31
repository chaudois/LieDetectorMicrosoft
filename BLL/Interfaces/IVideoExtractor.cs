using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Interfaces
{
    public interface IVideoExtractor
    {
        void Extract(string videos);
    }
}