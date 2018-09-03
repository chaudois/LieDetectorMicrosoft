using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using Accord.Video.FFMPEG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

namespace BLL
{
    public class FaceRecognizerAzur : IFaceRecognizer
    {
        public bool busy { get; private set; }

        private const string baseUri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0";
        private const string subscriptionKey = "96205c584c3e485f9598b0dc30d19d2c";
        private readonly IFaceClient faceClient = new FaceClient(
            new Uri(baseUri),
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });
        public SortedList<int, string> orderedNotification { get; private set; }

        public string progress { get; private set; }

        public FaceRecognizerAzur()
        {
            busy = false;
            orderedNotification = new SortedList<int, string>();
        }
        public KeyValuePair<int, string>? GetReport()
        {

            if (orderedNotification == null || orderedNotification.Count() == 0) return null;
            KeyValuePair<int, string> result = new KeyValuePair<int, string>(orderedNotification.ElementAt(0).Key, orderedNotification.ElementAt(0).Value);
            orderedNotification.RemoveAt(0);
            return result;
        }

        public void AnalyzeVideo(string videoPath, string saveDirectory = null)
        {
            if (busy) return;
            busy = true;
            Task.Run(async () =>
            {

                if (saveDirectory == null)
                {
                    saveDirectory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
                }
                string videoName = videoPath.Split('\\').ToList()[videoPath.Split('\\').ToList().Count() - 2];
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(videoPath);
                    try
                    {

                        Bitmap videoFrame = reader.ReadVideoFrame();


                        int frameNumber = 1;
                        while (videoFrame != null)
                        {
                            string notification = "";
                            progress = frameNumber + "/" + reader.FrameCount;


                            var faces = await UploadAndDetectFaces(videoFrame);
                            notification = JsonConvert.SerializeObject(faces);
                            orderedNotification.Add(frameNumber, notification);
                            videoFrame = reader.ReadVideoFrame();
                            frameNumber++;
                        }
                        videoFrame.Dispose();
                        reader.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("erreur dans  AnalyseVideoAsync : " + e.Message);
                    }
                }
                busy = false;
            });
        }

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(Bitmap image)
        {
            // The list of Face attributes to return.
            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
                };

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = BmpToStream(image))
                {
                    // The second argument specifies to return the faceId, while
                    // the third argument specifies not to return face landmarks.
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, true, faceAttributes);
                    return faceList;
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                return new List<DetectedFace>();
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                return new List<DetectedFace>();
            }
        }


        private Stream BmpToStream(Bitmap videoFrame)
        {
            MemoryStream memoryStream = new MemoryStream();
            videoFrame.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

    }
}
