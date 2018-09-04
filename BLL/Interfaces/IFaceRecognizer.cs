using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace BLL
{
    public interface IFaceRecognizer
    {
 
        bool busy { get; }
        string progress { get; }
        KeyValuePair<int, string>? GetReport();
        void AnalyzeVideo(string videoPath, string saveDirectory = null);
        Bitmap GetFacePicture(Bitmap bitmap, string serializedModel);
        Bitmap GetFullPicture(Bitmap bitmap, string serializedModel);
    }
}