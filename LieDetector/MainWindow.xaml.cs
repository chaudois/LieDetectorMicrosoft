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

namespace LieDetector
{

    public partial class MainWindow : Window
    {
        string fileName = null;
        string execPath = null;
        string execDirecory = null;
        System.Windows.Forms.Timer timer;
        string imageSourceUrl;
        private CascadeClassifier _cascadeClassifier;
        long frameCount;
        bool pause, stop, skipFragment, skipFaceReco, fragFinished;
        Stack<string> fragments;

        public MainWindow()
        {

            execPath = Assembly.GetEntryAssembly().Location;
            execDirecory = execPath.Remove(execPath.LastIndexOf('\\'));
            timer = new System.Windows.Forms.Timer();
            timer.Tick += tick;
            timer.Start();
            _cascadeClassifier = new CascadeClassifier(@"D:\sourcesexpaceo\git\LieDetector\LieDetector\Ressources\xml\haarcascade_frontalface_alt_tree.xml");
            fragments = new Stack<string>();
            InitializeComponent();
        }

        private void tick(object sender, EventArgs e)
        {
            if (imageSourceUrl != null && imageSourceUrl != "" && imageSourceUrl != pictureBox1.Source?.ToString().Replace("file:///", ""))
            {

                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(imageSourceUrl);
                bmp.EndInit();

                pictureBox1.Source = bmp;
            }
        }

        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            fragFinished = false;
            //récuperation du fichier video
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            fileName = openFileDialog.FileName.Split('\\').ToList().Last().Split('.')[0];

            if (fileName != "")
            {
                skipFragment = false;
                skipFaceReco = false;
                stop = false;
                pause = false;
                BoutonDeleteResultFractionnage.IsEnabled = false;
                BoutonDeleteResultFaceReco.IsEnabled = false;
                BoutonPause.IsEnabled = true;
                BoutonCancel.IsEnabled = true;
                progressFractionnage.Foreground = System.Windows.Media.Brushes.Green;

                //lancement du thread d'extraction des images et de découpage des visages
                BoutonVideo.IsEnabled = false;
                nomDuFichier.Content = openFileDialog.FileName.Remove(0, openFileDialog.FileName.LastIndexOf('\\') + 1);
                Task.Run(() => fragmentVideo(openFileDialog.FileName));
                Task.Run(() => faceReco());
            }

        }
        private void fragmentVideo(string FileName)
        {
            try
            {

                //ouverture du flux d'image depuis l'emplacement du fichier video
                VideoFileReader reader = new VideoFileReader();
                reader.Open(FileName);
                frameCount = reader.FrameCount;
                System.Drawing.Image videoFrame = reader.ReadVideoFrame();
                progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Maximum = frameCount; });
                progressFaceReco.Dispatcher.Invoke(() => { progressFaceReco.Maximum = frameCount; });

                Directory.CreateDirectory("resultat\\fragmentation\\" + fileName);

                BoutonImages.Dispatcher.Invoke(() => { BoutonImages.IsEnabled = true; });

                int frame = 1;
                //creer une image par frame de la vidéo, et la stock dans un dossier du nom du fichier video
                while (videoFrame != null && !stop && !skipFragment)
                {
                    while (pause)
                    {
                        progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Foreground = System.Windows.Media.Brushes.DarkGoldenrod; });
                    }
                    progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Foreground = System.Windows.Media.Brushes.Green; });


                    //si la frame exist déja, ne la remplace pas, skip le traitement
                    if (!File.Exists(execDirecory + "\\resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp"))
                    {
                        videoFrame.Save(execDirecory + "\\resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp");
                    }
                    videoFrame.Dispose();
                    progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Value = frame; });//met à jour la progressBar 'progressFractionnage'
                    videoFrame = reader.ReadVideoFrame();
                    imageSourceUrl = execDirecory + "\\resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp";
                    fragments.Push(imageSourceUrl);
                    avancementFragmentation.Dispatcher.Invoke(() => { avancementFragmentation.Content = frame + "/" + frameCount; });
                    frame++;
                }


                if (stop)
                {
                    progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Foreground = System.Windows.Media.Brushes.Red; });
                }
                //fermeture du flux video
                reader.Close();
                fragFinished=true;

            }
            catch (Exception e)
            {
                throw e;
            }

        }
        //prend chaque image, en extrait les visages
        private void faceReco()
        {
            Directory.CreateDirectory("resultat\\visages\\" + fileName);
            BoutonOpenFaces.Dispatcher.Invoke(() => { BoutonOpenFaces.IsEnabled = true; });
            string oldFile = imageSourceUrl;
            int cpt = 0;
            while (!fragFinished || fragments.Count()>0)
            {
                if (stop || skipFaceReco) break;
                while (pause)
                {
                    progressFaceReco.Dispatcher.Invoke(() => { progressFaceReco.Foreground = System.Windows.Media.Brushes.DarkGoldenrod; });
                }
                progressFaceReco.Dispatcher.Invoke(() => { progressFaceReco.Foreground = System.Windows.Media.Brushes.Green; });

                if (fragments.Count() == 0) continue;
                Bitmap masterImage = new Bitmap(fragments.Pop());

                Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(masterImage);


                var faces = _cascadeClassifier.DetectMultiScale(normalizedMasterImage, 1.1, 1, System.Drawing.Size.Empty); //the actual face detection happens here
                foreach (var face in faces)
                {
                    //fabrication d'une image qui contient la tete localisé dans le resultat
                    Bitmap target = new Bitmap(face.Width, face.Height);

                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(masterImage, new Rectangle(0, 0, target.Width, target.Height),
                                         face,
                                         GraphicsUnit.Pixel);
                    }

                    target.Save(execDirecory + "\\resultat\\visages\\" + fileName + "\\" + cpt + ".bmp");
                   // imageSourceUrl = execDirecory + "\\resultat\\visages\\" + fileName + "\\" + cpt + ".bmp";

                }
                cpt++;
                progressFaceReco.Dispatcher.Invoke(() => { progressFaceReco.Value = cpt; });//met à jour la progressBar
                avancementFaceReco.Dispatcher.Invoke(() => { avancementFaceReco.Content = cpt + "/" + frameCount; });

            }
            if (stop)
            {
                progressFaceReco.Dispatcher.Invoke(() => { progressFaceReco.Foreground = System.Windows.Media.Brushes.Red; });

            }
            BoutonDeleteResultFaceReco.Dispatcher.Invoke(() => { BoutonDeleteResultFaceReco.IsEnabled = true; });
            BoutonDeleteResultFractionnage.Dispatcher.Invoke(() => { BoutonDeleteResultFractionnage.IsEnabled = true; });
            BoutonVideo.Dispatcher.Invoke(() => { BoutonVideo.IsEnabled = true; });


        }

        private void ButtonDisaplayFragmentation_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
                Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);
        }
        private void ButtonDisplayImageFaceReco_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
                Process.Start(execDirecory + "/resultat/visages/" + fileName);
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            stop = true;
            BoutonCancel.IsEnabled = false;
            BoutonDeleteResultFractionnage.IsEnabled = true;
            BoutonDeleteResultFaceReco.IsEnabled = true;
            BoutonPause.IsEnabled = false;
            BoutonVideo.IsEnabled = true;
        }

        private void BoutonPause_Click(object sender, RoutedEventArgs e)
        {
            if (pause)
            {
                pause = false;
                imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/pause.png"));
            }
            else
            {
                pause = true;
                imagePause.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Ressources/Images/play.png"));

            }

        }

        private void ButtonSkipFaceReco_Click(object sender, RoutedEventArgs e)
        {
            skipFaceReco = true;

        }

        private void BoutonDeleteResultFaceReco_Click(object sender, RoutedEventArgs e)
        {
            BoutonDeleteResultFaceReco.IsEnabled = false;
            if (Directory.Exists(execDirecory + "/resultat/visages/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/visages/" + fileName, true);
                BoutonDeleteResultFaceReco.IsEnabled = false;
                progressFractionnage.Value = 0;
            }
        }

        private void ButtonSkipFragment_Click(object sender, RoutedEventArgs e)
        {
            skipFragment = true;
            
        }

        private void ButtonRemoveImagesFragmentation_Click(object sender, RoutedEventArgs e)
        {
            BoutonDeleteResultFractionnage.IsEnabled = false;

            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
            {
                try
                {
                    Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);
                    progressFractionnage.Value = 0;

                }
                catch
                {
                    Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName, true);

                }

            }
        }
    }
}
