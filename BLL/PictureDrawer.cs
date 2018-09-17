using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BLL
{
    internal class PictureDrawer
    {
        public Bitmap DrawRectangleOnBmp(Rectangle[] faces, Bitmap picture, Color couleur, int rectangleAmount)
        {
            Bitmap result = new Bitmap(picture);
            for (int i = 0; i < rectangleAmount; i++)
            {
                if (faces.Length - 1 >= i)
                {

                    using (Graphics g = Graphics.FromImage(result))
                    {
                        Pen p = new Pen(couleur, (float)4.0);

                        if (p != null)
                        {

                            //dessine le rectangle ou se trouve le visage sur l'image
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y),
                                    new System.Drawing.Point(faces[i].X, faces[i].Y + faces[i].Height));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y + faces[i].Height),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y + faces[i].Height));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y + faces[i].Height),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y));
                            g.DrawLine(p,
                                    new System.Drawing.Point(faces[i].X, faces[i].Y),
                                    new System.Drawing.Point(faces[i].X + faces[i].Width, faces[i].Y));
                        }
                    }
                }

            }
            return result;
        }
        public Bitmap DrawMarqueurs(FaceLandmarks faceLandmarks, Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap);
            result = DrawPointOnBitmap(faceLandmarks.EyebrowLeftInner.X, faceLandmarks.EyebrowLeftInner.Y, result);
            result = DrawPointOnBitmap(faceLandmarks.EyebrowLeftOuter.X, faceLandmarks.EyebrowLeftOuter.Y, result);
            result = DrawPointOnBitmap(faceLandmarks.EyebrowRightInner.X, faceLandmarks.EyebrowRightInner.Y, result);
            result = DrawPointOnBitmap(faceLandmarks.EyebrowRightOuter.X, faceLandmarks.EyebrowRightOuter.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeLeftBottom.X, faceLandmarks.EyeLeftBottom.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeLeftInner.X, faceLandmarks.EyeLeftInner.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeLeftOuter.X, faceLandmarks.EyeLeftOuter.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeLeftTop.X, faceLandmarks.EyeLeftTop.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeRightBottom.X, faceLandmarks.EyeRightBottom.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeRightInner.X, faceLandmarks.EyeRightInner.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeRightOuter.X, faceLandmarks.EyeRightOuter.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.EyeRightTop.X, faceLandmarks.EyeRightTop.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.MouthLeft.X, faceLandmarks.MouthLeft.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.MouthRight.X, faceLandmarks.MouthRight.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseLeftAlarOutTip.X, faceLandmarks.NoseLeftAlarOutTip.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseLeftAlarTop.X, faceLandmarks.NoseLeftAlarTop.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseRightAlarOutTip.X, faceLandmarks.NoseRightAlarOutTip.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseRightAlarTop.X, faceLandmarks.NoseRightAlarTop.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseRootLeft.X, faceLandmarks.NoseRootLeft.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseRootRight.X, faceLandmarks.NoseRootRight.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.NoseTip.X, faceLandmarks.NoseTip.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.PupilLeft.X, faceLandmarks.PupilLeft.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.PupilRight.X, faceLandmarks.PupilRight.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.UnderLipBottom.X, faceLandmarks.UnderLipBottom.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.UnderLipTop.X, faceLandmarks.UnderLipTop.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.UpperLipBottom.X, faceLandmarks.UpperLipBottom.Y, result);
            result= DrawPointOnBitmap(faceLandmarks.UpperLipTop.X, faceLandmarks.UpperLipTop.Y, result);
            return result;
        }
        public Bitmap DrawPointOnBitmap(double x, double y, Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap);
            using (Graphics g = Graphics.FromImage(result))
            {
                Pen p = new Pen(Color.Yellow, (float)1.0);

                if (p != null)
                {

                    g.FillRectangle(p.Brush, (float)x, (float)y, 3, 3);
                }
            }
            return result;
        }
        public Bitmap CutRectangleFromBitmap(Bitmap bitmap, Rectangle rectangle)
        {
            Bitmap result = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics g = Graphics.FromImage(result))
            {

                g.DrawImage(bitmap,
                    new Rectangle(0,0,result.Width,result.Height),
                    rectangle.X,
                    rectangle.Y,
                    rectangle.Width,
                    rectangle.Height,GraphicsUnit.Pixel);
              
            }
            return result;
        }
    }
}
