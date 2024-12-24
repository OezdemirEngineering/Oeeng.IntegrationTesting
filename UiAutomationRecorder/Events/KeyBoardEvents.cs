using Gma.System.MouseKeyHook;
using System;
using System.Windows.Forms;

namespace UiAutomationRecorder.Events;

public static class KeyboardEvents
{
    private static IKeyboardMouseEvents _globalHook;

    // Event, das ausgelöst wird, wenn eine Taste gedrückt wird
    public static event Action<string> KeyPressed;

    // Event, das ausgelöst wird, wenn eine Taste losgelassen wird
    public static event Action<string> KeyReleased;

    // Startet die Überwachung der Tastatureingaben
    public static void StartTracking()
    {
        _globalHook = Hook.GlobalEvents();

        // Abonniere KeyPress-Ereignisse
        _globalHook.KeyPress += OnKeyPress;

    }

    // Stoppt die Überwachung der Tastatureingaben
    public static void StopTracking()
    {
        _globalHook?.Dispose();
    }

    // Callback für KeyPress (nur sichtbare Zeichen)
    private static void OnKeyPress(object sender, KeyPressEventArgs e)
    {
        KeyPressed?.Invoke($"{e.KeyChar}");
    }

}
