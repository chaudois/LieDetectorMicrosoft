using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        long frameCut, frameCount, PictureAnalysed, FaceFounded;
        IVideoConverter _videoConverter;
        public bool IsFinished()
        {
            return finished;
        }
        public VideoExtractor(IVideoConverter videoConverter)
        {
            this._videoConverter = videoConverter;
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
            VideoFileReader reader = new VideoFileReader();

            PictureAnalysed = FaceFounded = 0;
            reader.Open(videoLocation);
            frameCount = reader.FrameCount;
            Bitmap videoFrame = reader.ReadVideoFrame();
            string fileName = videoLocation.Split('\\').ToList().Last().Split('.')[0];
            Directory.CreateDirectory(execDirectory + "\\resultat\\fragmentation\\" + fileName);
            Thread.CurrentThread.Name = "Extract_" + fileName;


            frameCut = 0;
            //creer une image par frame de la vidéo, et la stock dans un dossier du nom du fichier video
            while (videoFrame != null && !stop)
            {
                while (pause)
                {

                }
                string pathPicture = execDirectory + "\\resultat\\fragmentation\\" + fileName + "\\" + frameCut + ".bmp";

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
                        middleWare.Notify(frameCut.ToString() + "/" + frameCount);
                    }
                }
                videoFrame.Dispose();
                videoFrame = null;
                Task.Run(() => _videoConverter.ProcessPicture(pathPicture, execDirectory));
                videoFrame = reader.ReadVideoFrame();
                frameCut++;
            }
            finished = true;
            //fermeture du flux video
            reader.Close();
        }

    }
}
