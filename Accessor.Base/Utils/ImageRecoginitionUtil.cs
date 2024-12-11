using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace Accessor.Base.Utils;

public static class ImageRecoginitionUtil
{
    private static Mat BitmapToMat(Bitmap bitmap)
    {        
        Bitmap convertedBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);

        Mat mat = convertedBitmap.ToMat();

        if (mat.Depth != DepthType.Cv8U || mat.NumberOfChannels != 3)
            throw new Exception("Das Bild muss 8-Bit-Tiefe und 3 Kanäle (BGR) haben.");

        return mat;
    }

    public static Point LocateElement(Bitmap main, Bitmap element)
    {
        var resultPoint = new Point(-1, -1);
        Mat screenshotMat = BitmapToMat(main);
        Mat elementMat = BitmapToMat(element);

        // Template Matching durchführen
        Mat result = new Mat();
        CvInvoke.MatchTemplate(
            screenshotMat, elementMat, result, TemplateMatchingType.CcoeffNormed);

        // Variablen für MinMaxLoc
        double minVal = 0, maxVal = 0;
        System.Drawing.Point minLoc = new System.Drawing.Point();
        System.Drawing.Point maxLoc = new System.Drawing.Point();
        // Bestes Match finden
        CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
        if (maxVal > 0.8) 
        {  
            resultPoint.X=  maxLoc.X + elementMat.Width / 2;
            resultPoint.Y = maxLoc.Y + elementMat.Height / 2;
        }
        else
        {
            throw new Exception("Button-Bild nicht im Fenster gefunden.");
        }

        return resultPoint;
    }

    public static bool AreImagesEqual(Bitmap img1, Bitmap img2)
    {
        Mat mat1 = BitmapToMat(img1);
        Mat mat2 = BitmapToMat(img2);
        if (img1.Size != img2.Size)
            return false;

        var diff = new Mat();
        CvInvoke.AbsDiff(mat1, mat2, diff);
        var nonZeroCount = CvInvoke.CountNonZero(diff);
        return nonZeroCount == 0;
    }
}
