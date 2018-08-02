using System;
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

namespace BLL
{
    public class VideoConverter : IVideoConverter
    {
        private bool pause;
        private List<IProgress<string>> middleWares;
        const string XML_LOCATION = @"D:\sourcesexpaceo\git\LieDetector\LieDetector\Ressources\xml\haarcascade_frontalface_alt_tree.xml";
        public VideoConverter()
        {
            middleWares = new List<IProgress<string>>();
        }

        public void ProcessPicture(string pathPicture, string execDirectory)
        {

            if (pathPicture != null)
            {
                string VideoName = pathPicture.Split('\\').ToList()[pathPicture.Split('\\').ToList().Count() - 2];
                string fileName = pathPicture.Split('\\').ToList().Last();
                // Thread.CurrentThread.Name = "ProcessPicture_" + VideoName+"_"+fileName;
                Directory.CreateDirectory(execDirectory + "\\resultat\\visages\\" + VideoName);

                while (pause)
                {
                }
                Bitmap masterImage = new Bitmap(pathPicture);

                Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(masterImage);




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
                            Bitmap target = new Bitmap(face.Width, face.Height);

                            using (Graphics g = Graphics.FromImage(target))
                            {
                                g.DrawImage(masterImage, new Rectangle(0, 0, target.Width, target.Height),
                                                 face,
                                                 GraphicsUnit.Pixel);
                            }

                            target.Save(execDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
                            target.Dispose();
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
                normalizedMasterImage.Dispose();
                masterImage.Dispose();
                foreach (var middleWare in middleWares)
                {
                    middleWare.Report(execDirectory + "\\resultat\\visages\\" + VideoName + "\\" + fileName);
                }
            }
        }
        public void Pause()
        {
            pause = !pause;
        }
        public void addMiddleWare(IProgress<string> middleWare)
        {
            middleWares.Add(middleWare);
        }

    }
}
