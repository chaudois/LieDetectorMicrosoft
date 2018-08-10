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
using BLL;
using Unity;
using BLL.Interfaces;

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        IVideoSplitter videoSplitter;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int nbImage;
        bool paused;
        IObserver observerExtration, observerFaceReco;
        string filePath;
        string execDirecory;
        public MainWindow()
        {
            execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));

            var unity = UnityConfig.Setup();
            videoSplitter = unity.Resolve<IVideoSplitter>();
            Thread.CurrentThread.Name = "Main";
            timer.Tick += tick;
            observerExtration = new GenericObserver();
            observerFaceReco = new GenericObserver();
            videoSplitter.AddObserverToExtractor(ref observerExtration);
            videoSplitter.AddObserverToFaceReco(ref observerFaceReco);
            InitializeComponent();
        }
        private void tick(object sender, EventArgs e)
        {
            try
            {
                BoutonVideo.IsEnabled = videoSplitter.IsFinished();

                nbImage = int.Parse(observerExtration.GetMessage().Split('/')[1]);
                progressFractionnage.Maximum = nbImage;
                progressFractionnage.Value = observerExtration.GetNotificationCount();
                progressFaceReco.Maximum = nbImage;
                progressFaceReco.Value = observerFaceReco.GetNotificationCount();
                avancementFragmentation.Content = observerExtration.GetMessage();
                avancementFaceReco.Content = observerFaceReco.GetNotificationCount() + "/" + nbImage;

            }
            catch (Exception)
            {


            }


        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.ShowDialog();
            videoSplitter.Stop();

            try
            {

                filePath = videoSplitter.SplitAndFaceRecoAllVideo()[0];
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

            string fileName = filePath.Remove(0, filePath.LastIndexOf("\\") + 1).Split('.')[0];
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
                Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);
        }
        private void ButtonDisplayImageFaceReco_Click(object sender, RoutedEventArgs e)
        {

            string fileName = filePath.Remove(0, filePath.LastIndexOf("\\") + 1).Split('.')[0];
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
            string fileName = filePath.Remove(0, filePath.LastIndexOf("\\") + 1).Split('.')[0];
            BoutonOpenFaces.IsEnabled = false;
            BoutonDeleteResultFaceReco.IsEnabled = false;
            if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/visages/" + fileName, true);
            }
        }
        private void BoutonDeleteResultFragmentation_Click(object sender, RoutedEventArgs e)
        {
            string fileName = filePath.Remove(0, filePath.LastIndexOf("\\") + 1).Split('.')[0];
            BoutonOpenImages.IsEnabled = false;
            BoutonDeleteResultFractionnage.IsEnabled = false;
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);
            }
        }
    

    }
}
