using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Models;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;

namespace BLL
{
    public class FaceRecognizer : IFaceRecognizer
    {
        private List<IObserver> middleWares;
        const string XML_LOCATION = @"D:\sourcesexpaceo\git\LieDetector\LieDetector\Ressources\xml\haarcascade_frontalface_alt_tree.xml";
        ConcurrentQueue<string> pictureStack;
        bool pause, stop;
        int isRunning;
        List<Task> tasks;
        public FaceRecognizer()
        {
            tasks = new List<Task>();
            pictureStack = new ConcurrentQueue<string>();
            middleWares = new List<IObserver>();
            isRunning = 0;
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
        public void Stop()
        {
            stop = true;
            foreach (var middleWare in middleWares)
            {
                middleWare.Reset();
            }
            pictureStack = new ConcurrentQueue<string>();
        }
        public void addObserver(ref IObserver middleWare)
        {
            middleWares.Add(middleWare);
        }
        public void FaceReco(string filePath, string saveDirectory)
        {
            string VideoName = "";
            while (pictureStack.Count() > 0 && !stop)
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

                                //c'est sur cette ligne que ce fait la reconnaissance facial
                                Rectangle[] faces = new CascadeClassifier(XML_LOCATION).DetectMultiScale(normalizedMasterImage,
                                    1.05,
                                    3,
                                    Size.Empty);
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



                                foreach (var middleWare in middleWares)
                                {
                                    string message = pathPicture + "_" + JsonConvert.SerializeObject(faces) + "_";
                                    middleWare.Notify(message);
                                }
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


        public void FaceRecoAsync(string filePath, string saveDirectory, int maxSimultaneousTask)
        {
            pictureStack.Enqueue(filePath);
            isRunning++;
            if (isRunning < maxSimultaneousTask)
            {
                tasks.Add( new Task(() =>
                     FaceReco(filePath, saveDirectory)
                     ));
                stop = false;
                tasks.Last().Start();

            }
        }

    }
}
