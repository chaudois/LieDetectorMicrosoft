using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Interfaces
{
    public interface IVideoSplitter 
    {
        /// <summary>
        /// summon the IVideoProvider given in the constructor to get a list of file from which extract faces
        /// </summary>
        /// <returns>array of string containing the full path of each chosen files by the user</returns>
        List<string>  SplitAndFaceRecoAllVideoAsync();
        Observer GetSplitProgressReport(string fileName);
        Observer GetFaceRecoProgressReport(string fileName);
        bool IsFinished();
        void Pause();
        void Stop();
    }
}