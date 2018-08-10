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
    public class VideoSplitter : IVideoSplitter
    {
        bool stop, pause, finished;
        List<IObserver> middleWares;
        long   frameCount, PictureAnalysed, FaceFound;
        IFaceRecognizer _faceRecognizer;
        IVideoProvider _videoProvider;
        public bool IsFinished()
        {
            return finished;
        }
        public VideoSplitter(IFaceRecognizer faceRecognizer, IVideoProvider videoProvider)
        {

            _videoProvider = videoProvider;
            _faceRecognizer = faceRecognizer;
            middleWares = new List<IObserver>();

        }
        public void Stop()
        {
            stop = true;
            
            _faceRecognizer.Stop();
            foreach (var item in middleWares)
            {
                item.Reset();
            }
        }
        public void Pause()
        {
            pause = !pause;
            _faceRecognizer.Pause();
        }
        public void AddObserverToExtractor(ref IObserver middleWare)
        {
            middleWares.Add(middleWare);
        }
        public void AddObserverToFaceReco(ref IObserver middleWare)
        {
            _faceRecognizer.addObserver(ref middleWare);
        }
        public void Split(string videoLocation, string execDirectory)
        {
            stop = false;
            finished = false;
            string fileName = videoLocation.Split('\\').ToList().Last().Split('.')[0];
            Directory.CreateDirectory(execDirectory + "\\resultat\\fragmentation\\" + fileName);
            Thread.CurrentThread.Name = "Extract_" + fileName;
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
                         _faceRecognizer.FaceRecoAsync(pathPicture,execDirectory);

                    }

                }
                finished = true;

            }

        }
        public List<string> SplitAndFaceRecoAllVideo()
        {
            string execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            List<string> files = _videoProvider.GetFiles();
            foreach (string file in files)
            {
                Task.Run(() => Split(file, execDirecory));
            }
            return files;
        }
        
    }
}
