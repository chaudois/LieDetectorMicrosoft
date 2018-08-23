using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using BLL;
using BLL.Interfaces;
using Unity.Resolution;

namespace ConsoleUI
{
    class UnityConfig
    {
        public static IUnityContainer Setup()
        {

            var result = new UnityContainer();
            result.RegisterType<IVideoSplitter, VideoSplitter>();
            result.RegisterType<IVideoProvider, VideoProviderConsole>();
            return result;

        }
    }
}
