using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using BLL;
using BLL.Interfaces;

namespace LieDetector
{
    class UnityConfig
    {
        public static IUnityContainer Setup()
        {

            var result = new UnityContainer();
            result.RegisterType<IVideoConverter, VideoConverter>();
            result.RegisterType<IVideoExtractor, VideoExtractor>();
            result.RegisterType<IVideoProvider, VideoProviderWPF>();
            return result;

        }
    }
}
