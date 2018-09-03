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
        string[] filesPath;
        string execDirecory;
        VideoFileReader reader;
        int cptVideo = 0;
        public MainWindow()
        {
            execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
            faceRecognizer = new FaceRecognizerEmguCV();
            Thread.CurrentThread.Name = "Main";
            reader = new VideoFileReader();
            timer.Interval = 1;
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
        private Bitmap DrawRectangleOnBmp(Rectangle[] faces, Bitmap picture, Color couleur, int rectangleAmount)
        {
            for (int i = 0; i < rectangleAmount; i++)
            {
                if (faces.Length - 1 >= i)
                {

                    using (Graphics g = Graphics.FromImage(picture))
                    {
                        Pen p = new Pen(couleur, (float)4.0);

                        if (p != null)
                        {

                            //dessine le rectangle ou se trouve le visage sur l'image
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y),
                                    new System.Drawing.Point(faces[i].X, faces[i].Y + faces[i].Height));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y + faces[i].Height),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y + faces[i].Height));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y + faces[i].Height),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y));
                        }
                    }
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

                        Rectangle[] face = null;
                        Rectangle[] eyes = null;
                        IList<DetectedFace> marqueurs = null;
                        try
                        {
                            face = JsonConvert.DeserializeObject<Rectangle[]>(JsonConvert.DeserializeObject<dynamic>(report.Value.Value).faces.ToString());

                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            eyes = JsonConvert.DeserializeObject<Rectangle[]>(JsonConvert.DeserializeObject<dynamic>(report.Value.Value).eyes.ToString());

                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            marqueurs = JsonConvert.DeserializeObject<IList<DetectedFace>>(JsonConvert.DeserializeObject<dynamic>(report.Value.Value).ToString());
                        }
                        catch (Exception)
                        {
                        }

                        if (face != null && face.Count() > 0)
                        {
                            bitmap = DrawRectangleOnBmp(face, bitmap, Color.Red, 1);

                            if (eyes != null && eyes.Length > 1)
                            {

                                bitmap = DrawRectangleOnBmp(eyes, bitmap, Color.Yellow, 2);
                                bitmap = DrawRectangleOnBmp(eyes, bitmap, Color.Yellow, 2);
                            }
                        }
                        if (marqueurs != null)
                        {
                            foreach (var visage in marqueurs)
                            {
                                bitmap = DrawRectangleOnBmp(new Rectangle[]{
                                    new Rectangle(visage.FaceRectangle.Left,visage.FaceRectangle.Top,visage.FaceRectangle.Width,visage.FaceRectangle.Height)
                                }, bitmap, Color.Red, 1);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyebrowLeftInner.X, visage.FaceLandmarks.EyebrowLeftInner.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyebrowLeftOuter.X, visage.FaceLandmarks.EyebrowLeftOuter.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyebrowRightInner.X, visage.FaceLandmarks.EyebrowRightInner.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyebrowRightOuter.X, visage.FaceLandmarks.EyebrowRightOuter.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeLeftBottom.X, visage.FaceLandmarks.EyeLeftBottom.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeLeftInner.X, visage.FaceLandmarks.EyeLeftInner.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeLeftOuter.X, visage.FaceLandmarks.EyeLeftOuter.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeLeftTop.X, visage.FaceLandmarks.EyeLeftTop.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeRightBottom.X, visage.FaceLandmarks.EyeRightBottom.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeRightInner.X, visage.FaceLandmarks.EyeRightInner.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeRightOuter.X, visage.FaceLandmarks.EyeRightOuter.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.EyeRightTop.X, visage.FaceLandmarks.EyeRightTop.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.MouthLeft.X, visage.FaceLandmarks.MouthLeft.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.MouthRight.X, visage.FaceLandmarks.MouthRight.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseLeftAlarOutTip.X, visage.FaceLandmarks.NoseLeftAlarOutTip.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseLeftAlarTop.X, visage.FaceLandmarks.NoseLeftAlarTop.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseRightAlarOutTip.X, visage.FaceLandmarks.NoseRightAlarOutTip.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseRightAlarTop.X, visage.FaceLandmarks.NoseRightAlarTop.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseRootLeft.X, visage.FaceLandmarks.NoseRootLeft.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseRootRight.X, visage.FaceLandmarks.NoseRootRight.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.NoseTip.X, visage.FaceLandmarks.NoseTip.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.PupilLeft.X, visage.FaceLandmarks.PupilLeft.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.PupilRight.X, visage.FaceLandmarks.PupilRight.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.UnderLipBottom.X, visage.FaceLandmarks.UnderLipBottom.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.UnderLipTop.X, visage.FaceLandmarks.UnderLipTop.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.UpperLipBottom.X, visage.FaceLandmarks.UpperLipBottom.Y, bitmap);
                                bitmap = DrawPointOnBitmap(visage.FaceLandmarks.UpperLipTop.X, visage.FaceLandmarks.UpperLipTop.Y, bitmap);
                            }
                        }
                        pictureBox1.Source = ConverBitmapToBitmapImage(bitmap);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }


        }

        private Bitmap DrawPointOnBitmap(double x, double y, Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen p = new Pen(Color.Yellow, (float)1.0);

                if (p != null)
                {

                    g.FillRectangle(p.Brush, (float)x, (float)y, 4, 4);
                }
            }
            return bitmap;
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
