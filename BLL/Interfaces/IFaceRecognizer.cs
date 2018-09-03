using System.Threading.Tasks;

namespace BLL
{
    public interface IFaceRecognizer
    {
        bool busy { get; }
        Observer observer { get; set; }

        Task AnalyzeVideo(string videoPath, string saveDirectory = null);
    }
}