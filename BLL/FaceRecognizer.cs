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
        CascadeClassifier haarcascade_frontalface_alt_tree;
        CascadeClassifier haarcascade_eye;

        public FaceRecognizer()
        {
            observer = new Observer();
            busy = false;
            haarcascade_frontalface_alt_tree = new CascadeClassifier("xml/haarcascade_frontalface_alt_tree.xml");
            haarcascade_eye = new CascadeClassifier("xml/haarcascade_eye.xml");
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


                        int frameNumber = 1;
                        while (videoFrame != null)
                        {
                            using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(videoFrame))
                            {

                                //c'est sur ce block que ce fait la reconnaissance facial
                                Rectangle[] faces = FindFaces(videoFrame);
                                Rectangle[] eyes = null;
                                string notification = "";
                                try
                                {

                                    using (Bitmap target = new Bitmap(faces[0].Width, faces[0].Height))
                                    {
                                        using (Graphics g = Graphics.FromImage(target))
                                        {
                                            g.DrawImage(videoFrame, new Rectangle(0, 0, target.Width, target.Height),
                                                             faces[0],
                                                             GraphicsUnit.Pixel);
                                        }
                                        eyes = FindEyes(target);
                                        if (eyes != null)
                                        {
                                            for (int i = 0; i < eyes.Length; i++)
                                            {
                                                eyes[i].X += faces[0].X;
                                                eyes[i].Y += faces[0].Y;
                                            }

                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("erreur dans  AnalyseVideoAsync : " + e.Message);
                                }
                                notification = JsonConvert.SerializeObject(new
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


        private Rectangle[] FindFaces(Bitmap videoFrame )
        {
            using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(videoFrame))
            {

                //c'est sur ce block que ce fait la reconnaissance facial
                return haarcascade_frontalface_alt_tree.
                     DetectMultiScale(
                     normalizedMasterImage,
                     1.1,
                     -1,
                     Size.Empty);
            }
        }
        private Rectangle[] FindEyes(Bitmap videoFrame )
        {
            using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(videoFrame))
            {

                //c'est sur ce block que ce fait la reconnaissance facial
                return haarcascade_eye.
                     DetectMultiScale(
                     normalizedMasterImage,
                     1.1,
                     -1,
                     Size.Empty);
            }
        }
    }
}

