using Accessor.Base;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Accessors
{
    public class TextBoxAccessor: AccessorBase
    {
        public TextBoxAccessor(AutomationElement parent, string automationId) : base(parent, automationId)
        {
        }
        public TextBoxAccessor(AutomationElement parent, string name, ControlType type) : base(parent, name, type)
        {
        }
        public TextBoxAccessor(AutomationElement parent, Bitmap element) : base(parent, element)
        {
        }

        public string Text
        {
            set
            {
                Element.AsTextBox().Text = value;
            }
            get
            {
                return Element.AsTextBox().Text;
            }
        }
    }
}
