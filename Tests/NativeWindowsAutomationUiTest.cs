using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Tests;

public class NativeWindowsAutomationUiTest
{
    [Fact]
    public void TestLoginFunctionality()
    {
        string appPath = @"Login.exe";
        var process = Process.Start(appPath);
        Thread.Sleep(2000);

        try
        {
            var mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "MainWindow"));

            var usernameBox = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "UsernameBox"));

            var passwordBox = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "PasswordBox"));

            var loginButton = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "LoginButton"));

            if (usernameBox.TryGetCurrentPattern(ValuePattern.Pattern, out var usernamePattern))
            {
                ((ValuePattern)usernamePattern).SetValue("admin");
            }

            if (passwordBox.TryGetCurrentPattern(ValuePattern.Pattern, out var passwordPattern))
            {
                ((ValuePattern)passwordPattern).SetValue("password");
            }

            if (loginButton.TryGetCurrentPattern(InvokePattern.Pattern, out var loginPattern))
            {
                ((InvokePattern)loginPattern).Invoke();
            }

            Thread.Sleep(1000); // Warten, bis die Aktion abgeschlossen ist

            var dashboardPanel = mainWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "DashBoard"));
            Assert.NotNull(dashboardPanel);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
    }
}


