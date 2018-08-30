using Accord.Video.FFMPEG;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public class FaceRecognizer
    {
        public Observer observer { get; set; }
        public bool busy { get; private set; }
        public FaceRecognizer()
        {
            observer = new Observer();
            busy = false;
        }
        private CascadeClassifier GetCascadeClassifier(string videoName, string haarcascadeType, string saveDirectory)
        {
            if (!File.Exists(saveDirectory + "/xml/" + haarcascadeType + "_" + videoName + ".xml"))
            {
                File.Copy(saveDirectory + "/xml/" + haarcascadeType + ".xml", saveDirectory + "/xml/" + haarcascadeType + "_" + videoName + ".xml");

            }
            return new CascadeClassifier(saveDirectory + "/xml/" + haarcascadeType + "_" + videoName + ".xml");
        }
        public void AnalyzeVideo(string videoPath, string saveDirectory = null)
        {
            Task.Run(() =>
            {
                if (busy) return;

                busy = true;
                if (saveDirectory == null)
                {
                    saveDirectory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
                }
                string videoName = videoPath.Split('\\').ToList()[videoPath.Split('\\').ToList().Count() - 2];
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(videoPath);
                    observer.frameCount = reader.FrameCount;
                    try
                    {

                        Bitmap videoFrame = reader.ReadVideoFrame();


                        CascadeClassifier haarcascade_frontalface_alt_tree = GetCascadeClassifier(videoName, "haarcascade_frontalface_alt_tree", saveDirectory);
                        CascadeClassifier haarcascade_eye = GetCascadeClassifier(videoName, "haarcascade_eye", saveDirectory);
                        int frameNumber = 1;
                        while (videoFrame != null)
                        {
                            using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(videoFrame))
                            {

                                //c'est sur ce block que ce fait la reconnaissance facial
                                Rectangle[] faces = FindRectangles(videoFrame, haarcascade_frontalface_alt_tree);
                                Rectangle[] eyes = FindRectangles(videoFrame, haarcascade_eye);
                                var notification = JsonConvert.SerializeObject(new
                                {

                                    faces,
                                    eyes
                                });
                                observer.Notify(frameNumber, notification);
                            }
                            videoFrame = reader.ReadVideoFrame();
                            frameNumber++;
                        }
                        videoFrame.Dispose();
                        reader.Dispose();
                        haarcascade_frontalface_alt_tree.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("erreur dans  AnalyseVideoAsync : " + e.Message);
                    }
                }
                busy = false;
            });
        }


        private Rectangle[] FindRectangles(Bitmap videoFrame, CascadeClassifier haarcascade)
        {
            using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(videoFrame))
            {

                //c'est sur ce block que ce fait la reconnaissance facial
                return haarcascade.
                     DetectMultiScale(
                     normalizedMasterImage,
                     1.05,
                     -1,
                     Size.Empty);
            }
        }
    }
}

