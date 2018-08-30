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
using Accord.Video.FFMPEG;

namespace LieDetector
{

    public partial class MainWindow : Window
    {

        FaceRecognizer faceRecognizer;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        long nbImage;
        string[] filesPath;
        string execDirecory;
        VideoFileReader reader;
        int cptVideo = 0;
        public MainWindow()
        {
            execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            faceRecognizer = new FaceRecognizer();
            Thread.CurrentThread.Name = "Main";
            reader = new VideoFileReader();
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
        private Bitmap DrawRectangleOnBmp(Rectangle face, Bitmap picture,Color couleur)
        {
            using (Graphics g = Graphics.FromImage(picture))
            {
                Pen p = new Pen(couleur, (float)10.0);

                if (p != null)
                {

                    //dessine le rectangle ou se trouve le visage sur l'image
                    g.DrawLine(p,
                            new System.Drawing.Point(face.X, face.Y),
                            new System.Drawing.Point(face.X, face.Y + face.Height));
                    g.DrawLine(p,
                            new System.Drawing.Point(face.X, face.Y + face.Height),
                            new System.Drawing.Point(face.X + face.Width, face.Y + face.Height));
                    g.DrawLine(p,
                            new System.Drawing.Point(face.X + face.Width, face.Y + face.Height),
                            new System.Drawing.Point(face.X + face.Width, face.Y));
                    g.DrawLine(p,
                            new System.Drawing.Point(face.X, face.Y),
                            new System.Drawing.Point(face.X + face.Width, face.Y));
                }
            }
            return picture;
        }
        private void tick(object sender, EventArgs e)
        {
            try
            {
                nomDuFichier.Content = filesPath[0].Split('\\')[filesPath[0].Split('\\').Length - 1];
                BoutonVideo.IsEnabled = faceRecognizer.busy;
                nbImage = faceRecognizer.observer.frameCount;

                progressFractionnage.Maximum = nbImage;

                progressFractionnage.Value = faceRecognizer.observer.GetNotificationCount();

                labelAvancementFragmentation.Content = faceRecognizer.observer.GetNotificationCount() + "/" + faceRecognizer.observer.frameCount;

                KeyValuePair<int, string>? progress = faceRecognizer.observer.GetReport();

                if (progress != null && progress.Value.Key > 0)
                {
                    Bitmap bitmap = null;
                    while (progress.Value.Key > cptVideo)
                    {
                        bitmap = reader.ReadVideoFrame();
                        cptVideo++;
                    }
                    if (bitmap != null)
                    {
                        Rectangle[] face = JsonConvert.DeserializeObject<Rectangle[]>(JsonConvert.DeserializeObject<dynamic>(progress.Value.Value).faces.ToString());
                        Rectangle[] eyes = JsonConvert.DeserializeObject<Rectangle[]>(JsonConvert.DeserializeObject<dynamic>(progress.Value.Value).eyes.ToString());
                        Bitmap pictureSource = DrawRectangleOnBmp(face[0], bitmap,Color.Red);
                        pictureSource = DrawRectangleOnBmp(eyes[0], bitmap,Color.Yellow);
                        pictureSource = DrawRectangleOnBmp(eyes[1], bitmap,Color.Yellow);
                        pictureBox1.Source = ConverBitmapToBitmapImage(pictureSource);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }


        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                filesPath = new string[]
                {
                    "C:\\Users\\d.chaudois\\Videos\\VideoHololLens\\20180724-115023-HoloLens-Verite.mp4"
                };
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
