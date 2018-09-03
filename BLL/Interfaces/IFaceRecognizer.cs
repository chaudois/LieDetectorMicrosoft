using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL
{
    public interface IFaceRecognizer
    {
 
        bool busy { get; }
        string progress { get; }
        KeyValuePair<int, string>? GetReport();
        void AnalyzeVideo(string videoPath, string saveDirectory = null);
    }
}