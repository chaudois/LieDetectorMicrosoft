using System;
using System.IO;
using System.Threading;
using BLL;
using BLL.Interfaces;
using Unity;

namespace ConsoleUI
{
    class Program
    {
        public static string[] files { get; set; }

        private static void Print(string s)
        {
            Console.WriteLine(s);
        }
        static void Main(string[] args)
        {


            IUnityContainer unity = UnityConfig.Setup();
            var videoSplitter = unity.Resolve<IVideoSplitter>();

            Print("fichiers analysé : ");
            string[] fichiers = videoSplitter.SplitAndFaceRecoAllVideoAsync().ToArray();
            foreach (var item in fichiers)
            {
                Print(item);
            }
            while (!videoSplitter.IsFinished())
            {
                try
                {
                    Console.Clear();
                    foreach (var fichier in fichiers)
                    {

                        Print(fichier);
                        Console.Write("Split : ");
                        int nbImage = int.Parse(videoSplitter.GetSplitProgressReport(fichier).GetReport().Split('/')[1]);
                        Print(videoSplitter.GetSplitProgressReport(fichier).GetReport());
                        Console.Write("FaceReco : ");
                        Print(videoSplitter.GetFaceRecoProgressReport(fichier).GetNotificationCount() + "/" + nbImage);
                        Thread.Sleep(500);
                    }
                }
                catch (Exception)
                {

                }


            }
            Console.ReadLine();
        }
    }
}
