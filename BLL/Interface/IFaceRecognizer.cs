namespace BLL
{
    public interface IFaceRecognizer
    {
        bool busy { get; }
        Observer observer { get; set; }

        void AnalyzeVideo(string videoPath, string saveDirectory = null);
    }
}