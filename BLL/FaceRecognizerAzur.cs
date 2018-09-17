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
using DAL;

namespace BLL
{
    public class FaceRecognizerAzur : IFaceRecognizer
    {
        public bool busy { get; private set; }
        private PictureDrawer pictureDrawer = new PictureDrawer();
        private const string baseUri = "https://westeurope.api.cognitive.microsoft.com/face/v1.0";
        private const string subscriptionKey = "96205c584c3e485f9598b0dc30d19d2c";
        private readonly IFaceClient faceClient = new FaceClient(
            new Uri(baseUri),
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });
        public string progress { get; private set; }
        int poped = 0;
        private ISQLMananger _SQLMananger = new SQLMananger();
        string videoName = null;
        public FaceRecognizerAzur()
        {
            busy = false;


        }


        public void AnalyzeVideo(string videoPath)
        {
            if (busy) return;
            busy = true;
            Task.Run(async () =>
            {
                videoName = videoPath.Split('\\').ToList()[videoPath.Split('\\').ToList().Count() - 2];
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(videoPath);
                    try
                    {

                        int frameNumber = 0;
                        Bitmap videoFrame = reader.ReadVideoFrame();
                        while (videoFrame != null)
                        {
                            progress = frameNumber + "/" + reader.FrameCount;

                            if (_SQLMananger.getFrame(videoName, frameNumber) == null )
                            {
                                var faces = await UploadAndDetectFaces(videoFrame);
                                string resultAzure = JsonConvert.SerializeObject(faces);
                                _SQLMananger.saveFrame(videoName, frameNumber, resultAzure);
                            }
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

        public Bitmap GetFacePicture(Bitmap bitmap, string serializedModel)
        {
            IList<DetectedFace> marqueurs = JsonConvert.DeserializeObject<IList<DetectedFace>>(serializedModel);
            Bitmap result = null;
            if (marqueurs != null && marqueurs.Count() > 0)
            {
                var visage = marqueurs[0];
                result = pictureDrawer.DrawMarqueurs(visage.FaceLandmarks, bitmap);
                result = pictureDrawer.CutRectangleFromBitmap(result,
                    new Rectangle(visage.FaceRectangle.Left,
                    visage.FaceRectangle.Top,
                    visage.FaceRectangle.Width,
                    visage.FaceRectangle.Height)
                    );
            }
            return result;
        }

        public Bitmap GetFullPicture(Bitmap bitmap, string serializedModel)
        {
            IList<DetectedFace> marqueurs = JsonConvert.DeserializeObject<IList<DetectedFace>>(serializedModel);
            if (marqueurs != null && marqueurs.Count() > 0)
            {
                var visage = marqueurs[0];
                bitmap = pictureDrawer.DrawRectangleOnBmp(new Rectangle[]{
                                    new Rectangle(visage.FaceRectangle.Left,visage.FaceRectangle.Top,visage.FaceRectangle.Width,visage.FaceRectangle.Height)
                                }, bitmap, Color.Red, 1);
                //bitmap = pictureDrawer.DrawMarqueurs(visage.FaceLandmarks, bitmap);
            }
            return bitmap;
        }

        public KeyValuePair<int, string> popFaceRecoResult()
        {
            string result = _SQLMananger.getFrame(videoName, poped);
            if(result!=null && result != "")
            {
                var retour = new KeyValuePair<int, string>(poped,  result);
                poped++;
                return retour;
            }
            return new KeyValuePair<int, string>(poped,null);
        }
    }
}
