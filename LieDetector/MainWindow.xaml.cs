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

namespace LieDetector
{

    public partial class MainWindow : Window
    {
        string fileName = null;
        string execPath = null;
        string execDirecory = null;
        CancellationTokenSource tokenSource2 = null;
        CancellationToken ct;
        public MainWindow()
        {
            tokenSource2 = new CancellationTokenSource();
            ct = tokenSource2.Token;
            ;
            execPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            execDirecory = execPath.Remove(execPath.LastIndexOf('\\'));
            InitializeComponent();
        }
        private void ButtonVideo_Click(object sender, RoutedEventArgs e)
        {
            if ((string)BoutonVideo.Content == "Video...")
            {

                BoutonVideo.Content = "Cancel";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                fileName = openFileDialog.FileName.Split('\\').ToList().Last().Split('.')[0];
                if (fileName != "")
                {
                    Task.Run(() => fragmentVideo(openFileDialog.FileName)); 
                }
            }
            else
            {
                tokenSource2.Cancel();
                BoutonVideo.Content = "Video...";

            }


        }
        private void fragmentVideo(string FileName)
        {
            VideoFileReader reader = new VideoFileReader();
            reader.Open(FileName);
            Bitmap videoFrame = reader.ReadVideoFrame();
            if (!Directory.Exists(execDirecory + "\\resultat\\fragmentation\\" + fileName))
            {

                Directory.CreateDirectory("resultat\\fragmentation\\" + fileName);
                int frame = 0;
                while (videoFrame != null && !ct.IsCancellationRequested)
                {
                    videoFrame.Save(execDirecory + "\\resultat\\fragmentation\\" + fileName + "\\" + frame + ".bmp");
                    videoFrame.Dispose();
                    videoFrame = reader.ReadVideoFrame();
                    frame++;
                }
                reader.Close();

            } 
            //BoutonImages.IsEnabled = true;
            //BoutonVideo.Content = "Video...";

        }
        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {

            Process.Start(execDirecory + "/resultat/fragmentation/" + fileName);

        }

    }
}
