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
    public class VideoConverter : IVideoConverter
    {
        private List<IObserver> middleWares;
        const string XML_LOCATION = @"D:\sourcesexpaceo\git\LieDetector\LieDetector\Ressources\xml\haarcascade_frontalface_alt_tree.xml";
        Stack<string> pictureStack;
        private bool finish;
        /// <summary>
        /// notify that the extraction is over and now the process can stop after the stack of picture is empty
        /// </summary>
        public void ExtractionIsOver()
        {
            finish = true;
        }
        public VideoConverter()
        {
            pictureStack = new Stack<string>();
            middleWares = new List<IObserver>();
        }
        public void AddPicture(string pathPicture)
        {
            pictureStack.Push(pathPicture);
        }
        public void addObserver(ref IObserver middleWare)
        {
            middleWares.Add(middleWare);
        }

        public void FaceReco(string execDirectory)
        {
            Task.Run(() =>
            {

                string VideoName = "";
                while (pictureStack.Count() > 0 || !finish)
                {
                    if (pictureStack.Count == 0) continue;
                    string pathPicture = pictureStack.Pop();
                    if (pathPicture != null)
                    {
                        string fileName = pathPicture.Split('\\').ToList().Last();
                        if (VideoName == "")
                        {
                            VideoName = pathPicture.Split('\\').ToList()[pathPicture.Split('\\').ToList().Count() - 2];
                            Thread.CurrentThread.Name = "ProcessPicture_" + VideoName + "_" + fileName;
                            Directory.CreateDirectory(execDirectory + "\\resultat\\visages\\" + VideoName);
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

                                                target.Save(execDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
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
                                    middleWare.Notify(execDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
                                }

                            }
                        }
                    }
                }
            });
        }

    }
}
