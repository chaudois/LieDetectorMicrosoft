using System;
using System.Reflection;
using BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLieDetector
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            FaceRecognizer faceRecognizer2 = new FaceRecognizer();
            string execDirecory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));

            faceRecognizer2.AnalyzeVideo("C:\\Users\\d.chaudois\\Videos\\VideoHololLens\\20180724-115023-HoloLens-Verite.mp4", execDirecory);
            Assert.IsTrue(true);
        }
    }
}
