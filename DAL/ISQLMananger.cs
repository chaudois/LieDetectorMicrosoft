namespace DAL
{
    public interface ISQLMananger
    {
        void saveFrame(string videoName, int frameNumber, string resultAzure);
        void saveVideo(string videoName);
        string getFrame(string videoName, int frameNumber);
    }
}