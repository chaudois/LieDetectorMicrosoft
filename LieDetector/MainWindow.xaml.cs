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
using BLL;
using System.Drawing;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using Accord.Video.FFMPEG;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Windows.Forms;

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        IFaceRecognizer faceRecognizer;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        long nbImage;
        VideoFileReader reader;
        Bitmap currentImage = null;
        int cptVideo = 0;
        private bool paused;

        public MainWindow()
        {
            faceRecognizer = new FaceRecognizerAzur();
            Thread.CurrentThread.Name = "Main";
            reader = new VideoFileReader();
            timer.Interval = 1;
            timer.Tick += tick;
            InitializeComponent();
            //ButtonVideo_Click(null, null);
        }

        private void tick(object sender, EventArgs e)
        {

            try
            {
                updateWindows();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }


        }

        private void updateWindows()
        {
            DateTime dateTime = DateTime.Now;
            BoutonVideo.IsEnabled = faceRecognizer.busy;

            nbImage = long.Parse(faceRecognizer.progress.Split('/')[1]);

            progressFractionnage.Maximum = nbImage;

            progressFractionnage.Value = cptVideo;

            labelAvancementFragmentation.Content = cptVideo + "/" + nbImage;

            KeyValuePair<int, string> result = faceRecognizer.popFaceRecoResult();

            while (cptVideo < result.Key)
            {
                currentImage = reader.ReadVideoFrame();
                cptVideo++;
            }

            if (result.Value != null && result.Value != "")
            {
                if (checkBoxWatchFaceMarks.IsChecked == true)
                {
                    var tmp = faceRecognizer.GetFacePicture(currentImage, result.Value);
                    pictureFace.Source = ConverBitmapToBitmapImage(tmp);
                }
                else
                {
                    pictureFace.Source = null;
                }
                currentImage = faceRecognizer.GetFullPicture(currentImage, result.Value);
                pictureBox1.Source = ConverBitmapToBitmapImage(currentImage);
                tempsDeTraitement.Content = "temps de traitement pour un image : " + (DateTime.Now - dateTime).ToString("fff") + "ms";
            }
        }

        public BitmapImage ConverBitmapToBitmapImage(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] filesPath = null;
                var dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    filesPath = dialog.FileNames;
                    nomDuFichier.Content = filesPath[0];
                    reader.Open(filesPath[0]);
                    faceRecognizer.AnalyzeVideo(filesPath[0]);
                    progressFractionnage.Foreground = System.Windows.Media.Brushes.Green;

                    timer.Start();
                    PauseButton.IsEnabled = true;
                    imagePause.Source = new BitmapImage(new Uri("pack://application:,,,/Ressources/Images/pause.png"));
                }

            }
            catch (Exception)
            {

            }


        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                timer.Start();
                imagePause.Source = new BitmapImage(new Uri("pack://application:,,,/Ressources/Images/pause.png"));
            }
            else
            {
                timer.Stop();
                imagePause.Source = new BitmapImage(new Uri("pack://application:,,,/Ressources/Images/play.png"));
            }
            paused = !paused;
        }
    }
}
