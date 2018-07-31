using BLL.Interfaces; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LieDetectorConsole
{
     
    public class VideosProviderFromCommandLine : IVideosProvider
    {
        public IEnumerable<string> GetVideos()
        {
            Console.WriteLine("Hello world!");
            return new string[]
            {
                "coucou.mp4",
                "fichier2.avi"
            };
        }
    }
}