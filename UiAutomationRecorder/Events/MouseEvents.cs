using Gma.System.MouseKeyHook;
using System;
using System.Windows;
using System.Windows.Input;

public static class MouseEvents
{
    private static IKeyboardMouseEvents _globalHook;

    // Event für Mausklicks mit Mausposition
    public static event Action MouseClicked;

    // Startet die Überwachung von Mausklicks
    public static void StartTracking()
    {
        // Initialisiere den globalen Hook
        _globalHook = Hook.GlobalEvents();

        // Abonniere Mausereignisse
        _globalHook.MouseDownExt += OnMouseDown;
    }

    // Stoppt die Überwachung von Mausklicks
    public static void StopTracking()
    {
        _globalHook?.Dispose();
    }

    // Callback für Mausereignisse
    private static void OnMouseDown(object sender, MouseEventExtArgs e)
    {
        // Bestimme die Art des Klicks
        string button = e.Button switch
        {
            System.Windows.Forms.MouseButtons.Left => "LeftClick",
            System.Windows.Forms.MouseButtons.Right => "RightClick",
            System.Windows.Forms.MouseButtons.Middle => "MiddleClick",
            _ => "UnknownClick"
        };

        // Löse das MouseClicked-Event aus und übergebe die Position
        MouseClicked?.Invoke();
    }
}
