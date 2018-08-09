using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using BLL.Interfaces;
namespace BLL
{
    public class VideoExtractor : IVideoExtractor
    {
        bool stop, pause, finished;
        List<IObserver> middleWares;
        long   frameCount, PictureAnalysed, FaceFound;
        IVideoConverter _videoConverter;
        IVideoProvider _videoProvider;
        Stack<string> TaskStack;
        int taskRunning = 0;
        public bool IsFinished()
        {
            return finished;
        }
        public VideoExtractor(IVideoConverter videoConverter, IVideoProvider videoProvider)
        {

            TaskStack = new Stack<string>();
            _videoProvider = videoProvider;
            _videoConverter = videoConverter;
            middleWares = new List<IObserver>();

        }
        public void Stop()
        {
            stop = true;
        }
        public void Pause()
        {
            pause = !pause;
        }
        public void AddObserverToExtractor(ref IObserver middleWare)
        {
            middleWares.Add(middleWare);
        }
        public void AddObserverToFaceReco(ref IObserver middleWare)
        {
            _videoConverter.addObserver(ref middleWare);
        }
        public void Extract(string videoLocation, string execDirectory)
        {
            string fileName = videoLocation.Split('\\').ToList().Last().Split('.')[0];
            Directory.CreateDirectory(execDirectory + "\\resultat\\fragmentation\\" + fileName);
            Thread.CurrentThread.Name = "Extract_" + fileName;
            _videoConverter.FaceReco(execDirectory);
            using (VideoFileReader reader = new VideoFileReader())
            {


                reader.Open(videoLocation);


                PictureAnalysed = FaceFound = 0;
                frameCount = reader.FrameCount;

                for (int i = 0; i < reader.FrameCount && !stop; i++)
                {
                    
                    using (Bitmap videoFrame = reader.ReadVideoFrame())
                    {
                        while (pause)
                        {

                        }
                        string pathPicture = execDirectory + "\\resultat\\fragmentation\\" + fileName + "\\" + i + ".bmp";

                        //si la frame exist déja, ne la remplace pas, skip le traitement
                        if (!File.Exists(pathPicture))
                        {

                            videoFrame.Save(pathPicture);

                        }

                        //on informe tout les observeurs
                        if (middleWares != null && middleWares.Count() > 0)
                        {
                            foreach (var middleWare in middleWares)
                            {
                                middleWare.Notify(i.ToString() + "/" + frameCount);
                            }
                        }
                        videoFrame.Dispose();
                         _videoConverter.AddPicture(pathPicture);

                    }

                }
                _videoConverter.ExtractionIsOver();
                finished = true;

            }

        }
        public List<string> ExtractAllVideo()
        {
            string execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            List<string> files = _videoProvider.GetFiles();
            foreach (string file in files)
            {


                Task.Run(() => Extract(file, execDirecory));
            }
            return files;
        }
        
    }
}
