using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Video.FFMPEG;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;

namespace LieDetector
{

    public partial class MainWindow : Window
    {
        string fileName = null;
        string execPath = null;
        string execDirecory = null;
        CancellationTokenSource tokenSource2;
        CancellationToken ct;
        public MainWindow()
        {

            execPath = Assembly.GetEntryAssembly().Location;
            execDirecory = execPath.Remove(execPath.LastIndexOf('\\'));
            InitializeComponent();
        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            //récuperation du fichier video
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            fileName = openFileDialog.FileName.Split('\\').ToList().Last().Split('.')[0];


            if (fileName != "")
            {
                progressFractionnage.Foreground = System.Windows.Media.Brushes.Green;

                //lancement du thread d'extraction des images et de découpage des visages
                tokenSource2 = new CancellationTokenSource();
                ct = tokenSource2.Token;
                BoutonVideo.IsEnabled = false;
                BoutonImages.IsEnabled = false;
                Task.Run(() => fragmentVideo(openFileDialog.FileName));
            }

        }
        private void fragmentVideo(string FileName)
        {
            //ouverture du flux d'image depuis l'emplacement du fichier video
            VideoFileReader reader = new VideoFileReader();
            reader.Open(FileName);
            long frameCount = reader.FrameCount;
            Bitmap videoFrame = reader.ReadVideoFrame();
            progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Maximum = frameCount; });
            BoutonCancelFractionnage.Dispatcher.Invoke(() => { BoutonCancelFractionnage.IsEnabled = true; });

            //si un dossier avec le nom du fichier à extraire éxiste déja, ne fait rien,
            //se comporte comme si toutes les images avait été extraites
            if (!Directory.Exists(execDirecory + "\\resultat\\fragmentation\\" + fileName))
            {

                Directory.CreateDirectory("resultat\\fragmentation\\" + fileName);


                int frame = 0;

                //creer une image par frame de la vidéo, et la stock dans un dossier du nom du fichier
                while (videoFrame != null && !ct.IsCancellationRequested)
                {

                    progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Value = frame; });//met à jour la progressBar

                    videoFrame.Save(execDirecory + "\\resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp");
                    videoFrame.Dispose();
                    videoFrame = reader.ReadVideoFrame();
                    frame++;
                }

            }
            else
            {
                progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Value = frameCount; });
            }

            if (!ct.IsCancellationRequested)
            {
                CropFaces(FileName);
            }
            else
            {
                progressFractionnage.Dispatcher.Invoke(() => { progressFractionnage.Foreground = System.Windows.Media.Brushes.Red; });
                BoutonVideo.Dispatcher.Invoke(() => {BoutonVideo.IsEnabled = true;});
            }
            //fermeture du token d'interuption et du flux video
            tokenSource2.Dispose();
            reader.Close();

            BoutonImages.Dispatcher.Invoke(() => { BoutonImages.IsEnabled = true; });

        }
        private void CropFaces(string fileName)
        {
            throw new NotImplementedException();
        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
                Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            tokenSource2.Cancel();
            BoutonCancelFractionnage.IsEnabled = false;
            BoutonDeleteResultFractionnage.IsEnabled = true;
        }
        
        private void ButtonRemoveImages_Click(object sender, RoutedEventArgs e)
        {
            if(Directory.Exists(execDirecory + "/resultat/fragmentation/" + fileName))
            {
                Directory.Delete(execDirecory + "/resultat/fragmentation/" + fileName,true);
                BoutonDeleteResultFractionnage.IsEnabled = false;
                BoutonImages.IsEnabled = false;
                progressFractionnage.Value = 0;
            }
        }
    }
}
