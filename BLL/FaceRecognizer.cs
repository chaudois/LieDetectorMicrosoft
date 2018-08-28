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
        private Dictionary<string, CascadeClassifier> haarcascade_frontalface_alt_tree;
        private bool extractionIsOver;

        public FaceRecognizer()
        {


            haarcascade_frontalface_alt_tree = new Dictionary<string, CascadeClassifier>();
            tasks = new List<Task>();
            pictureStack = new ConcurrentQueue<string>();
            observer = new Observer();
            isRunning = 0;
            maxSimultaneousTask = 18;
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
        /// <summary>
        /// dequeue la queue, traite l'image avec emgu.cv en notifiant l'observer du resultat
        /// </summary>
        /// <param name="saveDirectory">dossier ou enregistrer l'image après avoir detecter un rectangle de visage</param>
        private void FaceRecoFromQueue(string TaskNumber, string saveDirectory)
        {
            string VideoName = "";


            while (!stop && !extractionIsOver)
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
                    try
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

                                Rectangle[] faces;
                                string foundWith = "";
                                //c'est sur ce block que ce fait la reconnaissance facial
                                faces = haarcascade_frontalface_alt_tree[TaskNumber].DetectMultiScale(normalizedMasterImage,
                                   1.05,
                                   -1,
                                   Size.Empty);
                                foundWith = "haarcascade_frontalface_alt_tree";


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
                                string message = pathPicture + "&" + JsonConvert.SerializeObject(faces) + "&" + foundWith;
                                observer.Notify(message);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("Erreur dans FaceRecognizer.FaceRecoFromQueue(string saveDirectory) : " + e.Message);
                    }

                }
            }
            haarcascade_frontalface_alt_tree.Remove(TaskNumber);



        }
        public void FaceRecoAsync(string filePath, string saveDirectory)
        {
            pictureStack.Enqueue(filePath);


            if (isRunning < maxSimultaneousTask)
            {
                isRunning++;
                if (!File.Exists(saveDirectory + "/xml/haarcascade_frontalface_alt_tree_" + isRunning + ".xml"))
                {
                    File.Copy(saveDirectory + "/xml/haarcascade_frontalface_alt_tree.xml", saveDirectory + "/xml/haarcascade_frontalface_alt_tree_" + isRunning + ".xml");

                }
                haarcascade_frontalface_alt_tree.Add(isRunning.ToString(), new CascadeClassifier(saveDirectory + "/xml/haarcascade_frontalface_alt_tree_" + isRunning + ".xml"));
                Task.Run(() =>
                {
                    FaceRecoFromQueue(isRunning.ToString(), saveDirectory);
                    isRunning--;

                });
                stop = false;

            }
        }

        public Observer GetReport()
        {
            return observer;
        }

        public void ExtractionIsOver()
        {
            extractionIsOver = true;
        }
    }
}
