

using Accessor.Base.Utils;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using System.Drawing;
using System.IO;

namespace Accessor.Base;

public class AccessorBase
{

    private Point _location;
    public Bitmap ParentBitmap { private set; get; }
    public readonly  AutomationElement Parent;
    public AccessorBase(AutomationElement parent, string automationId, int timeout = 200)
    {
        Element = parent.FindFirstDescendant(
            cf=>cf.ByAutomationId(automationId)).WaitUntilEnabled(TimeSpan.FromMilliseconds(timeout));
        _location.X = Element.BoundingRectangle.X;
        _location.Y = Element.BoundingRectangle.Y;
        Parent = parent;
    }

    public AccessorBase(AutomationElement parent, string name, ControlType type, int timeout = 200)
    {     
        Element = parent.FindFirstDescendant(
            cf => cf.ByName(name).And(cf.ByControlType(type))).WaitUntilEnabled(TimeSpan.FromMilliseconds(timeout));
        _location.X = Element.BoundingRectangle.X;
        _location.Y = Element.BoundingRectangle.Y;
        Parent = parent;
    }

    public AccessorBase(AutomationElement parent, Bitmap element)
    {
        Parent = parent;
        ParentBitmap = parent.Capture();
        _location = ImageRecoginitionUtil.LocateElement(ParentBitmap, element);
        _location.X += parent.BoundingRectangle.X;
        _location.Y += parent.BoundingRectangle.Y;

    }


    public AutomationElement Element { get; }

    protected void Click()
    {
        if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Capture"))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Capture");
        }

        if (Element is null)
        {
            Mouse.MoveTo(_location.X, _location.Y);
            Mouse.Click();
        }
        else
        {
            Element.Click();
        }
        ParentBitmap = Parent.Capture();
        var now = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        string fileName = string.Format("{0}.png",now);
        
        ParentBitmap.Save(Directory.GetCurrentDirectory() + "\\Capture\\" + fileName);
    }

}
