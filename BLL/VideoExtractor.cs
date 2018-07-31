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
        bool stop, pause;
        List<IProgress<string>> middleWares;
        IVideoConverter _videoConverter;
        public VideoExtractor(IVideoConverter videoConverter)
        {
            this._videoConverter = videoConverter;
            middleWares = new List<IProgress<string>>();

        }
        public void Stop()
        {
            stop = true;
        }
        public void Pause()
        {
            pause = !pause;
        }

        public void addNotification(IProgress<string> middleWare)
        {
            middleWares.Add(middleWare);
            _videoConverter.addMiddleWare(middleWare);
        }
        public void Extract(string videoLocation)
        {
            VideoFileReader reader = new VideoFileReader();


            reader.Open(videoLocation);
            long frameCount = reader.FrameCount;
            Bitmap videoFrame = reader.ReadVideoFrame();
            string fileName = videoLocation.Split('\\').ToList().Last().Split('.')[0];
            Directory.CreateDirectory("resultat\\fragmentation\\" + fileName);
            Thread.CurrentThread.Name = "Extract_" + fileName;


            int frame = 0;
            //creer une image par frame de la vidéo, et la stock dans un dossier du nom du fichier video
            while (videoFrame != null && !stop)
            {
                while (pause)
                {

                }
                string pathPicture = "resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp";

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
                        middleWare.Report(frame.ToString() + "/" + frameCount + "/" + pathPicture);
                    }
                }
                videoFrame.Dispose();
                videoFrame=null;
                _videoConverter.ProcessPicture(pathPicture);
                videoFrame = reader.ReadVideoFrame();
                frame++;
            }

            //fermeture du flux video
            reader.Close();
        }
    }
}
