using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Interfaces
{
    public interface IVideoConverter
    {
        void AddPicture(string pathPicture);
        void ExtractionIsOver();
        void addObserver(ref IObserver middleWare);
        void FaceReco(string execDirectory);
    }
}