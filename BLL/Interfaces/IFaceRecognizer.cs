using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace BLL
{
    public interface IFaceRecognizer
    {
 
        bool busy { get; }
        string progress { get; }
        void AnalyzeVideo(string videoPath );
        Bitmap GetFacePicture(Bitmap bitmap, string serializedModel);
        Bitmap GetFullPicture(Bitmap bitmap, string serializedModel);
        KeyValuePair<int,string> popFaceRecoResult();
    }
}