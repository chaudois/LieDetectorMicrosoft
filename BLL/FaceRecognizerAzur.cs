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

        public Observer observer { get; set; }
        private const string baseUri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0";
        private const string subscriptionKey = "96205c584c3e485f9598b0dc30d19d2c";
        private readonly IFaceClient faceClient = new FaceClient(
            new Uri(baseUri),
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });


        public FaceRecognizerAzur()
        {
            observer = new Observer();
            busy = false;
        }
        public async Task AnalyzeVideo(string videoPath, string saveDirectory = null)
        {
                if (busy) return;

                busy = true;
                if (saveDirectory == null)
                {
                    saveDirectory = Assembly.GetEntryAssembly().Location.Remove(Assembly.GetEntryAssembly().Location.LastIndexOf('\\'));
                }
                string videoName = videoPath.Split('\\').ToList()[videoPath.Split('\\').ToList().Count() - 2];
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(videoPath);
                    observer.frameCount = reader.FrameCount;
                    try
                    {

                        Bitmap videoFrame = reader.ReadVideoFrame();


                        int frameNumber = 1;
                        while (videoFrame != null)
                        {
                            string notification = "";
                            Rectangle[] faces = null;
                            Rectangle[] eyes = null;

                            try
                            {
                                faces = await  FindFaces(videoFrame);

                                using (Bitmap target = new Bitmap(faces[0].Width, faces[0].Height))
                                {
                                    using (Graphics g = Graphics.FromImage(target))
                                    {
                                        g.DrawImage(videoFrame, new Rectangle(0, 0, target.Width, target.Height),
                                                         faces[0],
                                                         GraphicsUnit.Pixel);
                                    }
                                    eyes = FindEyes(target);
                                    if (eyes != null)
                                    {
                                        for (int i = 0; i < eyes.Length; i++)
                                        {
                                            eyes[i].X += faces[0].X;
                                            eyes[i].Y += faces[0].Y;
                                        }

                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine("erreur dans  AnalyseVideoAsync : " + e.Message);
                            }
                            notification = JsonConvert.SerializeObject(new
                            {

                                faces,
                                eyes
                            });
                            observer.Notify(frameNumber, notification);
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
        }

        private Rectangle[] FindEyes(Bitmap target)
        {
            throw new NotImplementedException();
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
        private async Task<Rectangle[] > FindFaces(Bitmap videoFrame)
        {
            var  faces = await UploadAndDetectFaces(videoFrame);
            List<Rectangle> result = new List<Rectangle>();
 
 
            if (faces.Count > 0)
            {
                foreach (DetectedFace face in faces)
                {
                    result.Add(new Rectangle(
                face.FaceRectangle.Left  ,
                face.FaceRectangle.Top  ,
                face.FaceRectangle.Width  ,
                face.FaceRectangle.Height  
                ));
                }
            }
            return result.ToArray();

        }

        private Stream BmpToStream(Bitmap videoFrame)
        {
            MemoryStream memoryStream = new MemoryStream();
            videoFrame.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Seek(0,SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
