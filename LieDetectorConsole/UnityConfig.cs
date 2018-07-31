using BLL;
using BLL.Interfaces; 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity;
using Microsoft.Practices.Unity;
using BLL.Models;

namespace LieDetectorConsole
{
    class UnityConfig
    {
        public static IUnityContainer Configure()
        {
            var result = new UnityContainer();
            result.RegisterType<IVideoConverter, VideoConverter>();
            result.RegisterType<IVideoExtractor, VideoExtractor>();
            result.RegisterType<IProgress<ProgressModel>, ProgressForConsole>();

            result.RegisterType<IVideosProvider, VideosProviderFromCommandLine>();
             
            return result;
        }
    }
}