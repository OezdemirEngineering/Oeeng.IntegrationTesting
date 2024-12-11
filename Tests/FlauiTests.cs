using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

public class FlauiTests
{
    [Fact]
    public void Login_ShouldNavigateToDashboard()
    {
        // Arrange: WPF-Anwendung starten
        var process = Process.Start("Login.exe");

        var automation = new UIA3Automation();
        var app = FlaUI.Core.Application.Attach(process);
        var mainWindow = app.GetMainWindow(automation).WaitUntilEnabled(new TimeSpan(0, 0, 0, 0, 100));

        var usernameBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameBox")).AsTextBox();
        var passwordBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox();
        var loginButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")).AsTextBox();

        // Act.
        usernameBox.Text = "admin";
        passwordBox.Text = "password";
        loginButton.Click();


        // Assert
        mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard")).WaitUntilEnabled();
        var dashboardPanel = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard"));
        Assert.NotNull(dashboardPanel);
        mainWindow.Close();
    }
}
