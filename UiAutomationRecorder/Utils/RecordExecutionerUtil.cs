using Accessor.Base;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using UiAutomationRecorder.Models;

namespace UiAutomationRecorder.Utils;

internal class RecordExecutionerUtil
{
    public static bool IsRunning { get; private set; }
    public static int Index { get; private set; }

    /// <summary>
    /// Runs the recorded actions on the specified application.
    /// </summary>
    /// <param name="application">The application to run the actions on.</param>
    /// <param name="records">The list of recorded actions.</param>
    /// <param name="stepDurationInMs">The duration between each action in milliseconds.</param>
    public static async void Run(AutomationElement application, List<SerializableAutomationElementModel> records, int stepDurationInMs)
    {
        IsRunning = true;
        foreach (var rec in records)
        {
            if (!IsRunning)
                break;
            Index = records.IndexOf(rec);

            if (application == null) return;

            ExecuteAction(application, rec);
            await Task.Delay(stepDurationInMs); // Delay between actions
        }

        IsRunning = false;
    }

    /// <summary>
    /// Stops the execution of recorded actions.
    /// </summary>
    public static void Stop()
    {
        IsRunning = false;
    }

    /// <summary>
    /// Executes a single recorded action on the specified application.
    /// </summary>
    /// <param name="application">The application to execute the action on.</param>
    /// <param name="action">The recorded action to execute.</param>
    private static void ExecuteAction(AutomationElement application, SerializableAutomationElementModel action)
    {
        if (application == null) return;

        AccessorBase accessor;

        if (string.IsNullOrEmpty(action.AutomationId))
        {
            accessor = new AccessorBase(application, action.Name, (ControlType)Enum.Parse(typeof(ControlType), action.ControlType));
        }
        else
        {
            accessor = new AccessorBase(application, action.AutomationId);
        }

        var element = accessor.Element;

        if (element == null)
        {
            MessageBox.Show($"Element {action.Name} could not be found.");
            return;
        }

        if (action.Action == "MouseClick")
        {
            FlaUI.Core.Input.Mouse.Position = element.BoundingRectangle.Center();
            FlaUI.Core.Input.Mouse.LeftClick();
        }
        else if (action.ControlType == ControlType.Edit.ToString())
        {
            element.Focus();
            var textBox = element.AsTextBox();
            if (textBox != null)
            {
                if (string.IsNullOrEmpty(action.Action) && textBox.Text.Length > 0)
                {
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1); // Remove last character
                }
                else
                {
                    textBox.Focus();
                    FlaUI.Core.Input.Keyboard.Type(action.Action);
                }
            }
        }
    }
}
