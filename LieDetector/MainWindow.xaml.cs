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

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        IFaceRecognizer faceRecognizer;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        long nbImage;
        string execDirecory;
        VideoFileReader reader;
        int cptVideo = 0;
        public MainWindow()
        {
            execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            faceRecognizer = new FaceRecognizerAzur();
            Thread.CurrentThread.Name = "Main";
            reader = new VideoFileReader();
            timer.Interval = 1;
            timer.Tick += tick;
            InitializeComponent();
        }
 
        private void tick(object sender, EventArgs e)
        {
            try
            {

                BoutonVideo.IsEnabled = faceRecognizer.busy;

                nbImage = long.Parse(faceRecognizer.progress.Split('/')[1]);

                progressFractionnage.Maximum = nbImage;

                progressFractionnage.Value = cptVideo;

                labelAvancementFragmentation.Content = cptVideo + "/" + nbImage;

                var report = faceRecognizer.GetReport();
                if (report != null && report.Value.Key > 0)
                {
                    Bitmap bitmap = null;
                    while (report.Value.Key > cptVideo)
                    {
                        bitmap = reader.ReadVideoFrame();
                        cptVideo++;
                    }
                    if (bitmap != null)
                    {
                         
                         
                        pictureBox1.Source = ConverBitmapToBitmapImage( faceRecognizer.GetFullPicture( bitmap, report.Value.Value));
                        pictureFace.Source = ConverBitmapToBitmapImage(faceRecognizer.GetFacePicture(bitmap, report.Value.Value));
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
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
                string[] filesPath = new string[]
                {
                    "C:\\Users\\d.chaudois\\Videos\\VideoHololLens\\20180724-115023-HoloLens-Verite.mp4"
                };
                nomDuFichier.Content = filesPath[0];
                reader.Open(filesPath[0]);
                faceRecognizer.AnalyzeVideo(filesPath[0], execDirecory);
                progressFractionnage.Foreground = System.Windows.Media.Brushes.Green;

                timer.Start();
            }
            catch (Exception)
            {

            }


        }
    }
}
