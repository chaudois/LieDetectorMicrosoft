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
            for (int i = 0; i < rectangleAmount; i++)
            {
                if (faces.Length - 1 >= i)
                {

                    using (Graphics g = Graphics.FromImage(picture))
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
            return picture;
        }
        public Bitmap DrawMarqueurs(FaceLandmarks faceLandmarks, Bitmap bitmap)
        {
            bitmap = DrawPointOnBitmap(faceLandmarks.EyebrowLeftInner.X, faceLandmarks.EyebrowLeftInner.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyebrowLeftOuter.X, faceLandmarks.EyebrowLeftOuter.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyebrowRightInner.X, faceLandmarks.EyebrowRightInner.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyebrowRightOuter.X, faceLandmarks.EyebrowRightOuter.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeLeftBottom.X, faceLandmarks.EyeLeftBottom.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeLeftInner.X, faceLandmarks.EyeLeftInner.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeLeftOuter.X, faceLandmarks.EyeLeftOuter.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeLeftTop.X, faceLandmarks.EyeLeftTop.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeRightBottom.X, faceLandmarks.EyeRightBottom.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeRightInner.X, faceLandmarks.EyeRightInner.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeRightOuter.X, faceLandmarks.EyeRightOuter.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.EyeRightTop.X, faceLandmarks.EyeRightTop.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.MouthLeft.X, faceLandmarks.MouthLeft.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.MouthRight.X, faceLandmarks.MouthRight.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseLeftAlarOutTip.X, faceLandmarks.NoseLeftAlarOutTip.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseLeftAlarTop.X, faceLandmarks.NoseLeftAlarTop.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseRightAlarOutTip.X, faceLandmarks.NoseRightAlarOutTip.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseRightAlarTop.X, faceLandmarks.NoseRightAlarTop.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseRootLeft.X, faceLandmarks.NoseRootLeft.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseRootRight.X, faceLandmarks.NoseRootRight.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.NoseTip.X, faceLandmarks.NoseTip.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.PupilLeft.X, faceLandmarks.PupilLeft.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.PupilRight.X, faceLandmarks.PupilRight.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.UnderLipBottom.X, faceLandmarks.UnderLipBottom.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.UnderLipTop.X, faceLandmarks.UnderLipTop.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.UpperLipBottom.X, faceLandmarks.UpperLipBottom.Y, bitmap);
            bitmap = DrawPointOnBitmap(faceLandmarks.UpperLipTop.X, faceLandmarks.UpperLipTop.Y, bitmap);
            return bitmap;
        }
        public Bitmap DrawPointOnBitmap(double x, double y, Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen p = new Pen(Color.Yellow, (float)1.0);

                if (p != null)
                {

                    g.FillRectangle(p.Brush, (float)x, (float)y, 3, 3);
                }
            }
            return bitmap;
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
