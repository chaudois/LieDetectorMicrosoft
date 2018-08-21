using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
namespace ConsoleUI
{
    class VideoProviderConsole : IVideoProvider
    {
        private bool CheckFilesAllExist(string[] files)
        {
            foreach (var item in files)
            {
                if (!File.Exists(item))
                {
                    Console.WriteLine(item + " : Fichier introuvable");

                    return false;
                }
            }
            return true;
        }
        public List<string> GetFiles()
        {
            List<string> args = Environment.GetCommandLineArgs().ToList();
            args.RemoveAt(0);

            while (args == null || args.Count() == 0 || !CheckFilesAllExist(args.ToArray()))
            {

                Console.WriteLine("Veuillez entrer l'URL d'un ou plusieurs fichier(s) vidéo");
                args = Console.ReadLine().Split(' ').ToList();
             }
            return args.ToList();
        }
    }
}
