using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using BLL;
using Unity;
using BLL.Interfaces;
using System.Drawing;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        IVideoSplitter videoSplitter;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int nbImage;
        bool paused;
        string[] filesPath;
        string execDirecory;
        public MainWindow()
        {
            execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            var unity = UnityConfig.Setup();
            videoSplitter = unity.Resolve<IVideoSplitter>();
            Thread.CurrentThread.Name = "Main";
            timer.Tick += tick;
            InitializeComponent();
        }
        private BitmapImage ConverBitmapToBitmapImage(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
        private void tick(object sender, EventArgs e)
        {
            try
            {
                string videoName=filesPath[0].Split('\\')[filesPath[0].Split('\\').Length-1];
                BoutonVideo.IsEnabled = videoSplitter.IsFinished();
                nbImage = int.Parse(videoSplitter.GetSplitProgressReport(filesPath[0]).GetReport().Split('/')[1]);
                progressFractionnage.Maximum = nbImage;
                progressFaceReco.Maximum = nbImage;
                progressFractionnage.Value = videoSplitter.GetSplitProgressReport(filesPath[0]).GetNotificationCount();
                progressFaceReco.Value = videoSplitter.GetFaceRecoProgressReport(videoName).GetNotificationCount();
                labelAvancementFragmentation.Content = videoSplitter.GetSplitProgressReport(filesPath[0]).GetReport();
                labelAvancementFaceReco.Content = videoSplitter.GetFaceRecoProgressReport(videoName).GetNotificationCount() + "/" + nbImage;
                string message = videoSplitter.GetFaceRecoProgressReport(filesPath[0]).GetReport();
                if (message != null)
                {
                    string[] result = message.Split('_');
                    //si l'observeur rapporte une image traité
                    if (result[0] != null && result[0] != "")
                    {
                        //creer une image
                        Bitmap currentPicture = new Bitmap(result[0]);
                        //si l'observeur a trouver un visage
                        if (result[1] != null && result[1] != "[]")
                        {
                            Rectangle[] faces = JsonConvert.DeserializeObject<Rectangle[]>(result[1]);
                            using (Graphics g = Graphics.FromImage(currentPicture))
                            {
                                Pen p = new Pen(Color.Red, (float)10.0);

                                //dessine le rectangle ou se trouve le visage sur l'image
                                g.DrawLine(p,
                                        new System.Drawing.Point(faces[0].X, faces[0].Y),
                                        new System.Drawing.Point(faces[0].X , faces[0].Y + faces[0].Height));
                                g.DrawLine(p,
                                        new System.Drawing.Point(faces[0].X, faces[0].Y + faces[0].Height),
                                        new System.Drawing.Point(faces[0].X + faces[0].Width, faces[0].Y + faces[0].Height));
                                g.DrawLine(p,
                                        new System.Drawing.Point(faces[0].X + faces[0].Width, faces[0].Y + faces[0].Height),
                                        new System.Drawing.Point(faces[0].X + faces[0].Width, faces[0].Y ));
                                g.DrawLine(p,
                                        new System.Drawing.Point(faces[0].X, faces[0].Y  ),
                                        new System.Drawing.Point(faces[0].X + faces[0].Width, faces[0].Y ));
                            }
                        }
                        //affiche l'image avec le rectangle si il y en a un à l'utilisateur
                        pictureBox1.Source = ConverBitmapToBitmapImage(currentPicture);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                int i = 0;


            }


        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.ShowDialog();
            videoSplitter.Stop();

            try
            {

                filesPath = videoSplitter.SplitAndFaceRecoAllVideoAsync().ToArray() ;
                BoutonOpenImages.IsEnabled = true;
                BoutonOpenFaces.IsEnabled = true;
                BoutonPause.IsEnabled = true;
                BoutonCancel.IsEnabled = true;
                BoutonDeleteResultFractionnage.IsEnabled = false; 
                BoutonDeleteResultFaceReco.IsEnabled = false;
                progressFractionnage.Foreground = System.Windows.Media.Brushes.Green;
                progressFaceReco.Foreground = System.Windows.Media.Brushes.Green;

                timer.Start();
            }
            catch (Exception)
            {

            }


        }
        private void ButtonDisaplayFragmentation_Click(object sender, RoutedEventArgs e)
        {
            string execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));

            string fileName = filesPath[0].Remove(0, filesPath[0].LastIndexOf("\\") + 1).Split('.')[0];
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
                Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);
        }
        private void ButtonDisplayImageFaceReco_Click(object sender, RoutedEventArgs e)
        {

            string fileName = filesPath[0].Remove(0, filesPath[0].LastIndexOf("\\") + 1).Split('.')[0];
            if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
                Process.Start(execDirecory + "/resultat/visages/" + fileName);
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            videoSplitter.Stop();

            if (!videoSplitter.IsFinished())
            {
                progressFractionnage.Foreground = System.Windows.Media.Brushes.DarkRed;
                progressFaceReco.Foreground = System.Windows.Media.Brushes.DarkRed;
            }
            BoutonDeleteResultFractionnage.IsEnabled = true;
            BoutonDeleteResultFaceReco.IsEnabled = true;
            BoutonVideo.IsEnabled = true;
        }
        private void BoutonPause_Click(object sender, RoutedEventArgs e)
        {
            videoSplitter.Pause();
            paused = !paused;
            if (paused)
            {
                imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/play.png"));
            }
            else
            {
                imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/pause.png"));
            }
        }
        private void BoutonDeleteResultFaceReco_Click(object sender, RoutedEventArgs e)
        {
            string fileName = filesPath[0].Remove(0, filesPath[0].LastIndexOf("\\") + 1).Split('.')[0];
            BoutonOpenFaces.IsEnabled = false;
            BoutonDeleteResultFaceReco.IsEnabled = false;
            if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/visages/" + fileName, true);
            }
        }
        private void BoutonDeleteResultFragmentation_Click(object sender, RoutedEventArgs e)
        {
            string fileName = filesPath[0].Remove(0, filesPath[0].LastIndexOf("\\") + 1).Split('.')[0];
            BoutonOpenImages.IsEnabled = false;
            BoutonDeleteResultFractionnage.IsEnabled = false;
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);
            }
        }


    }
}
