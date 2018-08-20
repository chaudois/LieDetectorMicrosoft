using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Interfaces
{
    public interface IFaceRecognizer
    {
        void addObserver(ref IObserver middleWare);
        /// <summary>
        /// analyse a bmp file, and create a new bmp that only contains face detect on the picture
        /// </summary>
        /// <param name="filePath">path to the file to analyse</param>
        /// <param name="saveDirectory">root directory where you want the result directory to be created</param>
        void FaceRecoAsync(string filePath,string saveDirectory,int maxSimultaneousTask);
        void Pause();
        void Stop();
    }
}