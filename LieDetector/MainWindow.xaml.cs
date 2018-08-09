using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Accord.Video.FFMPEG;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using BLL;
using Unity;
using BLL.Interfaces;

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        IVideoExtractor videoExtractor;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int nbImage;
        IObserver observerExtration, observerFaceReco;
        string fileName;
        public MainWindow()
        {
            var unity = UnityConfig.Setup();
            videoExtractor = unity.Resolve<IVideoExtractor>();
            Thread.CurrentThread.Name = "Main";
            timer.Tick += tick;
            observerExtration = new GenericObserver();
            observerFaceReco = new GenericObserver();
            videoExtractor.AddObserverToExtractor(ref observerExtration);
            videoExtractor.AddObserverToFaceReco(ref observerFaceReco);
            InitializeComponent();
        }
        private void tick(object sender, EventArgs e)
        {
            BoutonVideo.IsEnabled = videoExtractor.IsFinished();
            try
            {

                nbImage = int.Parse(observerExtration.getMessage().Split('/')[1]);
                progressFractionnage.Maximum = nbImage;
                progressFractionnage.Value = observerExtration.getNotificationCount();
                progressFaceReco.Maximum = nbImage;
                progressFaceReco.Value = observerFaceReco.getNotificationCount();
                avancementFragmentation.Content = observerExtration.getMessage();
                avancementFaceReco.Content = observerFaceReco.getNotificationCount() + "/" + nbImage;

            }
            catch (Exception)
            {


            }


        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.ShowDialog();

            BoutonOpenImages.IsEnabled = true;
            fileName=videoExtractor.ExtractAllVideo()[0];
            timer.Start();


        }
        private void ButtonDisaplayFragmentation_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("  resultat/fragmentation/" + fileName))
                Process.Start("resultat/fragmentation/" + fileName);
        }
        private void ButtonDisplayImageFaceReco_Click(object sender, RoutedEventArgs e)
        {
            //if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
            //    Process.Start(execDirecory + "/resultat/visages/" + fileName);
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            //stop = true;
            //BoutonCancel.IsEnabled = false;
            //BoutonDeleteResultFractionnage.IsEnabled = true;
            //BoutonDeleteResultFaceReco.IsEnabled = true;
            //BoutonPause.IsEnabled = false;
            //BoutonVideo.IsEnabled = true;
        }
        private void BoutonPause_Click(object sender, RoutedEventArgs e)
        {
            //if (pause)
            //{
            //    pause = false;
            //    imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/pause.png"));
            //}
            //else
            //{
            //    pause = true;
            //    imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/play.png"));

            //}

        }
        private void ButtonSkipFaceReco_Click(object sender, RoutedEventArgs e)
        {
            //skipFaceReco = true;

        }
        private void BoutonDeleteResultFaceReco_Click(object sender, RoutedEventArgs e)
        {
            //BoutonDeleteResultFaceReco.IsEnabled = false;
            //if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
            //{
            //    Directory.Delete(execDirecory + "/resultat/visages/" + fileName, true);
            //    BoutonDeleteResultFaceReco.IsEnabled = false;
            //    progressFractionnage.Value = 0;
            //}
        }
        private void ButtonSkipFragment_Click(object sender, RoutedEventArgs e)
        {
            //skipFragment = true;

        }
        private void ButtonRemoveImagesFragmentation_Click(object sender, RoutedEventArgs e)
        {
            //BoutonDeleteResultFractionnage.IsEnabled = false;

            //if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
            //{
            //    try
            //    {
            //        Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);
            //        progressFractionnage.Value = 0;

            //    }
            //    catch
            //    {
            //        Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);

            //    }

            //}
        }

    }
}
