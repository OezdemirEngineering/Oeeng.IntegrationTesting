using Accessor.Base;
using Accessor.Base.Utils;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Accessors;

internal class ButtonAccessor:AccessorBase
{
    private Bitmap _buttonPressedBitmap;

    public ButtonAccessor(AutomationElement parent, string automationId) : base(parent, automationId)
    {

    }

    public ButtonAccessor(AutomationElement parent, string name, ControlType type, Bitmap buttonPressedBitmap = null) : base(parent, name, type)
    {
        _buttonPressedBitmap = buttonPressedBitmap;
    }

    public ButtonAccessor(AutomationElement parent, Bitmap element) : base(parent, element)
    {
    }


    public void Click()
    {
        base.Click();
    }

    public bool CLickWithFeedback()
    {
        Mouse.Position = Element.BoundingRectangle.Center();
        Mouse.Down(MouseButton.Left);
        Thread.Sleep(100);
        var currentButton =  Element.Capture();
        var result = CompareBitmaps(currentButton, _buttonPressedBitmap);
        Mouse.Up(MouseButton.Left);

        var threshold = 6;
        return result<=threshold;
    }

    public static double CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
    {
        if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height)
            throw new ArgumentException("Bitmaps must have the same dimensions");

        long diff = 0;

        for (int y = 0; y < bmp1.Height; y++)
        {
            for (int x = 0; x < bmp1.Width; x++)
            {
                Color c1 = bmp1.GetPixel(x, y);
                Color c2 = bmp2.GetPixel(x, y);

                diff += Math.Abs(c1.A - c2.A);
                diff += Math.Abs(c1.R - c2.R);
                diff += Math.Abs(c1.G - c2.G);
                diff += Math.Abs(c1.B - c2.B);
            }
        }

        double totalPixels = bmp1.Width * bmp1.Height * 4; // A,R, G, B
        double avgDiff = diff / totalPixels;
        return avgDiff; 
    }
}
