using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.UIA3;
using UiAutomationRecorder.Events;
using UiAutomationRecorder.Models;
using UiAutomationRecorder.Utils;

namespace UiAutomationRecorder;

public partial class MainWindow : System.Windows.Window
{
    private AutomationElement _attachedApplication;
    private readonly List<SerializableAutomationElementModel> _recordedActions = new List<SerializableAutomationElementModel>();
    private bool _isRecording;

    public MainWindow()
    {
        InitializeComponent();
        LoadApplications();
    }

    /// <summary>
    /// Loads the list of running applications with a main window.
    /// </summary>
    private void LoadApplications()
    {
        var processes = Process.GetProcesses()
            .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
            .Select(p => new { p.Id, p.MainWindowTitle })
            .ToList();

        ApplicationsComboBox.ItemsSource = processes;
        ApplicationsComboBox.DisplayMemberPath = "MainWindowTitle";
        ApplicationsComboBox.SelectedValuePath = "Id";
    }

    /// <summary>
    /// Attaches to the selected application.
    /// </summary>
    private void AttachButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApplicationsComboBox.SelectedItem is null)
        {
            MessageBox.Show("Please select an application.");
            return;
        }

        var selectedProcessId = (int)ApplicationsComboBox.SelectedValue;
        var process = Process.GetProcessById(selectedProcessId);

        try
        {
            var automation = new UIA3Automation();
            _attachedApplication = automation.FromHandle(process.MainWindowHandle);

            if (_attachedApplication != null)
            {
                MessageBox.Show($"Successfully attached to {process.MainWindowTitle}!");
            }
            else
            {
                MessageBox.Show("Could not find the window.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error attaching: {ex.Message}");
        }
    }

    /// <summary>
    /// Starts recording user actions.
    /// </summary>
    private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_attachedApplication == null)
        {
            MessageBox.Show("Please select and attach an application first.");
            return;
        }

        if (_isRecording)
        {
            MessageBox.Show("Recording is already in progress.");
            return;
        }

        _isRecording = true;
        MessageBox.Show("Recording started.");

        MouseEvents.StartTracking();
        MouseEvents.MouseClicked += OnMouseClick;

        // Start Keyboard Listener
        KeyboardEvents.StartTracking();
        KeyboardEvents.KeyPressed += OnKeyPress;
    }

    /// <summary>
    /// Handles mouse click events during recording.
    /// </summary>
    private void OnMouseClick()
    {
        if (!_isRecording) return;

        var mousePosition = Mouse.Position;
        var drawingPoint = new System.Drawing.Point((int)mousePosition.X, (int)mousePosition.Y);
        var element = _attachedApplication.FindAllDescendants().FirstOrDefault(e => e.BoundingRectangle.Contains(drawingPoint));

        if (element != null)
        {
            RecordMouseClick(element);
        }
    }

    /// <summary>
    /// Handles key press events during recording.
    /// </summary>
    private void OnKeyPress(string key)
    {
        if (!_isRecording) return;

        var focusedElement = _attachedApplication?.FindAllDescendants().FirstOrDefault(e => e.Properties.HasKeyboardFocus);

        if (focusedElement != null && focusedElement.ControlType == ControlType.Edit)
        {
            RecordTextEntry(focusedElement, key);
        }
    }

    /// <summary>
    /// Stops recording user actions.
    /// </summary>
    private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isRecording)
        {
            MessageBox.Show("No recording in progress to stop.");
            return;
        }

        _isRecording = false;
        MessageBox.Show("Recording stopped.");

        KeyboardEvents.StopTracking();
        MouseEvents.StopTracking();
        MouseEvents.MouseClicked -= OnMouseClick;
        KeyboardEvents.KeyPressed -= OnKeyPress;
        RecordFileManagerUtil.SaveActionsToFileDialog(_recordedActions);
    }

    /// <summary>
    /// Records a mouse click action.
    /// </summary>
    private void RecordMouseClick(AutomationElement element)
    {
        Dispatcher.Invoke(() =>
        {
            _recordedActions.Add(new SerializableAutomationElementModel
            {
                Name = element.Name,
                ControlType = element.ControlType.ToString(),
                AutomationId = element.AutomationId,
                BoundingRectangle = element.BoundingRectangle.ToString(),
                ClassName = element.ClassName,
                Action = "MouseClick",
                Timestamp = DateTime.Now
            });
            RecordedActionsListView.ItemsSource = null; // Refresh
            RecordedActionsListView.ItemsSource = _recordedActions;
        });
    }

    /// <summary>
    /// Records a text entry action.
    /// </summary>
    private void RecordTextEntry(AutomationElement element, string action)
    {
        Dispatcher.Invoke(() =>
        {
            _recordedActions.Add(new SerializableAutomationElementModel
            {
                Name = element.Name,
                ControlType = element.ControlType.ToString(),
                AutomationId = element.AutomationId,
                BoundingRectangle = element.BoundingRectangle.ToString(),
                ClassName = element.ClassName,
                Action = action,
                Timestamp = DateTime.Now
            });
            RecordedActionsListView.ItemsSource = null; // Refresh
            RecordedActionsListView.ItemsSource = _recordedActions;
        });
    }

    /// <summary>
    /// Runs the recorded actions.
    /// </summary>
    private async void RunButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_recordedActions.Any())
        {
            MessageBox.Show("No actions to execute.");
            return;
        }

        if (RecordExecutionerUtil.IsRunning)
        {
            MessageBox.Show("Actions are already being executed.");
            return;
        }

        MessageBox.Show("Execution started.");

        RecordExecutionerUtil.Run(_attachedApplication, _recordedActions, 500);

        while (RecordExecutionerUtil.IsRunning)
        {
            RecordedActionsListView.SelectedIndex = RecordExecutionerUtil.Index;
            await Task.Delay(100);
        }

        MessageBox.Show("Execution finished.");
    }

    /// <summary>
    /// Stops the execution of recorded actions.
    /// </summary>
    private void StopExecutionButton_Click(object sender, RoutedEventArgs e)
    {
        if (!RecordExecutionerUtil.IsRunning)
        {
            MessageBox.Show("No running process to stop.");
            return;
        }

        RecordExecutionerUtil.Stop();
        MessageBox.Show("Execution stopped.");
    }

    /// <summary>
    /// Loads recorded actions from a file.
    /// </summary>
    private void LoadActionsFromFile()
    {
        _recordedActions.Clear();
        _recordedActions.AddRange(RecordFileManagerUtil.LoadActionsFromFile());

        Dispatcher.Invoke(() =>
        {
            RecordedActionsListView.ItemsSource = null; // Refresh
            RecordedActionsListView.ItemsSource = _recordedActions;
        });
    }

    /// <summary>
    /// Handles the Load Actions button click event.
    /// </summary>
    private void LoadActionsButton_Click(object sender, RoutedEventArgs e)
    {
        LoadActionsFromFile();
    }

    private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
    "Oezdemir Engineering (OEENG)\n\n" +
    "Email: info@oeeng.de\n" +
    "Phone: +49 163 340 5013\n" +
    "Website: www.oeeng.de\n\n" +
    "Address:\n" +
    "Oezdemir Engineering\n" +
    "Brünestraße 23\n" +
    "52531 Übach-Palenberg, Germany",
    "Information",
    MessageBoxButton.OK,
    MessageBoxImage.Information);
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


}
