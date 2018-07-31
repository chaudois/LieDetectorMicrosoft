using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using BLL.Interfaces;
namespace LieDetectorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var Unity = UnityConfig.Configure();
            //la ligne ci dessous est égale à new VIdeoConverter
            var converter = Unity.Resolve<IVideoConverter>();
            Console.ReadLine();
        }
    }
}
