using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video.FFMPEG;
using BLL.Interfaces;
namespace LieDetector
{
    public class VideoProviderWPF : IVideoProvider
    {
        public List<string> GetFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            return openFileDialog.FileNames.ToList();
        }
    }
}
