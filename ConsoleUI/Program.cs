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
            

            IObserver observerSplitter, observerFaceReco;
            IUnityContainer unity = UnityConfig.Setup();
            var videoSplitter = unity.Resolve<IVideoSplitter>();

            observerSplitter = new GenericObserver();
            observerFaceReco = new GenericObserver();
            videoSplitter.AddObserverToExtractor(ref observerSplitter);
            videoSplitter.AddObserverToFaceReco(ref observerFaceReco);
            Print("fichiers analysé : ");
            foreach (var item in videoSplitter.SplitAndFaceRecoAllVideoAsync(6))
            {
                Print(item);
            }
            while (!videoSplitter.IsFinished())
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("Split : ");
                    int nbImage = int.Parse(observerSplitter.GetReport().Split('/')[1]);
                    Print(observerSplitter.GetReport());
                    Console.WriteLine("FaceReco : ");
                    Print(observerFaceReco.GetNotificationCount() + "/" + nbImage);
                    Thread.Sleep(100);
                }
                catch (Exception)
                {

                }


            }
            Console.ReadLine();
        }
    }
}
