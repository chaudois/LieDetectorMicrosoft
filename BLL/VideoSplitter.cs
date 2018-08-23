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
        private Dictionary<string, Observer> observers { get; set; }
        private Dictionary<string, FaceRecognizer> _faceRecognizer { get; set; }
        long frameCount;
        const int SIMULTANEOUS_TASK = 6;
        IVideoProvider _videoProvider;
        public bool IsFinished()
        {
            return finished;
        }
        public VideoSplitter(IVideoProvider videoProvider)
        {

            _videoProvider = videoProvider;
            observers = new Dictionary<string, Observer>();
            _faceRecognizer = new Dictionary<string, FaceRecognizer>();

        }
        public void Stop()
        {
            stop = true;
            foreach (var item in _faceRecognizer.Values)
            {

                item.StopAll();
            }
            foreach (var item in observers.Values)
            {
                item.Reset();
            }
        }
        public void Pause()
        {
            pause = !pause;
            foreach (var item in _faceRecognizer.Values)
            {
                item.Pause();
            }
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


                frameCount = reader.FrameCount;
                _faceRecognizer.Add(videoLocation, new FaceRecognizer());
                for (int i = 0; i < reader.FrameCount && !stop; i++)
                {

                    using (Bitmap videoFrame = reader.ReadVideoFrame())
                    {
                        while (pause)
                        {
                            Thread.Sleep(100);

                        }
                        string pathPicture = execDirectory + "\\resultat\\fragmentation\\" + fileName + "\\" + i + ".bmp";

                        //si la frame exist déja, ne la remplace pas, skip le traitement
                        if (!File.Exists(pathPicture))
                        {
                            videoFrame.Save(pathPicture);
                        }

                        //on informe tout les observeurs
                        if (observers != null && observers.Count() > 0)
                        {
                            foreach (var middleWare in observers)
                            {
                                observers[videoLocation].Notify(i.ToString() + "/" + frameCount);
                            }
                        }
                        videoFrame.Dispose();
                        //lance un Task pour faire la reconaissance de visage sur cette image en async
                        _faceRecognizer[videoLocation].FaceRecoAsync(pathPicture, execDirectory);

                    }
                }
                finished = true;
            }

        }
        public List<string> SplitAndFaceRecoAllVideoAsync()
        {
            string execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            List<string> files = _videoProvider.GetFiles();
            foreach (string file in files)
            {
                observers.Add(file, new Observer());
                Task.Run(() => Split(file, execDirecory));
            }
            return files;
        }
        /// <summary>
        /// retourne l'objet observer 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Observer GetSplitProgressReport(string fileName)
        {
            return observers[fileName];
        }

        public Observer GetFaceRecoProgressReport(string fileName)
        {
            return _faceRecognizer[fileName].GetReport();
        }
    }
}
