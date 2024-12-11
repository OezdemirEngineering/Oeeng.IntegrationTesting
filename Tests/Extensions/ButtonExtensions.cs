using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Extensions;

internal static class ButtonExtensions
{
    public static void Push(this Button button)
    {
        var postion = button.BoundingRectangle.Center();
        Mouse.MoveTo(postion.X,postion.Y);
        Mouse.Down(MouseButton.Left);
    }


    public static void Release(this Button button)
    {
        Mouse.Up(MouseButton.Left);
    }
}
