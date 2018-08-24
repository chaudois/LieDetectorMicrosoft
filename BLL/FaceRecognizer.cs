using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Interfaces;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
namespace BLL
{
    internal class FaceRecognizer : IFaceRecognizer
    {
        private Observer observer { get; set; }
        ConcurrentQueue<string> pictureStack;
        bool pause, stop;
        int isRunning;
        public int maxSimultaneousTask { get; set; }
        List<Task> tasks;
        public FaceRecognizer()
        {



            tasks = new List<Task>();
            pictureStack = new ConcurrentQueue<string>();
            observer = new Observer();
            isRunning = 0;
            maxSimultaneousTask = 6;
        }
        /// <summary>
        /// pause all Tasks
        /// </summary>
        public void Pause()
        {
            pause = !pause;
        }
        /// <summary>
        /// finish all task, reset all properties
        /// </summary>
        public void StopAll()
        {
            stop = true;
            observer.Reset();

            pictureStack = new ConcurrentQueue<string>();

        }
        public void FaceRecoFromQueue(string videoName, string saveDirectory)
        {
            string VideoName = "";
            while (pictureStack.Count() > 0 && !stop && isRunning <= maxSimultaneousTask)
            {
                while (pause)
                {
                    Thread.Sleep(100);
                }
                string pathPicture = "";
                pictureStack.TryDequeue(out pathPicture);
                if (pathPicture == null || pathPicture == "") continue;

                if (pathPicture != null)
                {
                    string fileName = pathPicture.Split('\\').ToList().Last();
                    if (VideoName == "")
                    {
                        VideoName = pathPicture.Split('\\').ToList()[pathPicture.Split('\\').ToList().Count() - 2];
                        Thread.CurrentThread.Name = "ProcessPicture_" + VideoName + "_" + isRunning;
                        Directory.CreateDirectory(saveDirectory + "\\resultat\\visages\\" + VideoName);
                    }
                    using (Bitmap masterImage = new Bitmap(pathPicture))
                    {

                        using (Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(masterImage))
                        {

                            try
                            {
                                Rectangle[] faces;
                                //c'est sur cette ligne que ce fait la reconnaissance facial
                                using (var cascadeClassifier = new CascadeClassifier(Properties.Resources.xml))
                                {
                                    faces = cascadeClassifier.DetectMultiScale(normalizedMasterImage,
                                       1.05,
                                       -1,
                                       Size.Empty);

                                }
                                foreach (var face in faces)
                                {

                                    try
                                    {

                                        //fabrication d'une image qui contient la tete localisé dans le resultat
                                        using (Bitmap target = new Bitmap(face.Width, face.Height))
                                        {
                                            using (Graphics g = Graphics.FromImage(target))
                                            {
                                                g.DrawImage(masterImage, new Rectangle(0, 0, target.Width, target.Height),
                                                                 face,
                                                                 GraphicsUnit.Pixel);
                                            }
                                            if (!File.Exists(saveDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName))
                                                target.Save(saveDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
                                        }


                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }


                                }
                                string message = pathPicture + "_" + JsonConvert.SerializeObject(faces) + "_";
                                observer.Notify(message);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            isRunning--;


        }
        public void FaceRecoAsync(string filePath, string saveDirectory)
        {
            string videoName = filePath.Split('\\')[filePath.Split('\\').Length - 2];
            pictureStack.Enqueue(filePath);


            if (isRunning < maxSimultaneousTask)
            {
                isRunning++;
                tasks.Add(new Task(() =>
                {
                    FaceRecoFromQueue(videoName, saveDirectory);
                }));
                stop = false;
                tasks.Last().Start();

            }
        }

        public Observer GetReport()
        {
            return observer;
        }
    }
}
