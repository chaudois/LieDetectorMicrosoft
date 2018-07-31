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
        public MainWindow()
        {
            Thread.CurrentThread.Name = "Main";

            InitializeComponent();
        }
        private void tick(object sender, EventArgs e)
        {
            //if (imageSourceUrl != null && imageSourceUrl != "" && imageSourceUrl != pictureBox1.Source?.ToString().Replace("file:///", ""))
            //{

            //    System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
            //    bmp.BeginInit();
            //    bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            //    bmp.UriSource = new Uri(imageSourceUrl);
            //    bmp.EndInit();

            //    pictureBox1.Source = bmp;
            //}
        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            var unity = UnityConfig.Setup();
            videoExtractor = unity.Resolve<IVideoExtractor>();
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.ShowDialog();
            Task.Run(() => videoExtractor.Extract(@"C:\Users\d.chaudois\Videos\video1.mp4"));
            BoutonVideo.IsEnabled = false;


        }
        private void ButtonDisaplayFragmentation_Click(object sender, RoutedEventArgs e)
        {
            //if (Directory.Exists(execDirecory + " / resultat/fragmentation/" + fileName))
            //    Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);
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
