using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Interfaces;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BLL
{
    public class FaceRecognizer : IFaceRecognizer
    {
        private List<IObserver> middleWares;
        const string XML_LOCATION = @"D:\sourcesexpaceo\git\LieDetector\LieDetector\Ressources\xml\haarcascade_frontalface_alt_tree.xml";
        Stack<string> pictureStack;
        bool pause,stop;
        int isRunning;
        public FaceRecognizer()
        {
            pictureStack = new Stack<string>();
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
            pictureStack = new Stack<string>();
        }
        public void addObserver(ref IObserver middleWare)
        {
            middleWares.Add(middleWare);
        }

        public void FaceRecoAsync(string filePath, string saveDirectory)
        {
            if (isRunning>6)
            {
                pictureStack.Push(filePath);
            }
            else
            {
                stop = false;
                isRunning++;
                pictureStack.Push(filePath);
                Task.Run(() =>
                {
                    //Thread.CurrentThread.Name = "FaceReco_" + filePath.Split('\\').ToList().Last();
                    string VideoName = "";
                    while (pictureStack.Count() > 0 && !stop)
                    {
                        while (pause)
                        {

                        }
                        string pathPicture = pictureStack.Pop();
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

                                    //c'est sur cette ligne que ce fait la reconnaissance facial
                                    Rectangle[] faces = null;
                                    try
                                    {

                                        faces = new CascadeClassifier(XML_LOCATION).DetectMultiScale(normalizedMasterImage, 1.1, 1, System.Drawing.Size.Empty);
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
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    //si on a trouvé des visages, le notifie aux observeurs avec le path de sauvegarde
                                    foreach (var middleWare in middleWares)
                                    {
                                        middleWare.Notify(saveDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
                                    }

                                }
                            }
                        }
                    }
                isRunning--;
                });

            }
        }

    }
}
