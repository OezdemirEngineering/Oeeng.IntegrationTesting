using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Tests.Extensions;
using Xunit;

namespace Tests;

public class FlauiTests
{
    [Fact]
    [Trait("Category", "UI Test")]
    public void Login_ShouldNavigateToDashboard()
    {
        // Arrange: WPF-Anwendung starten
        var process = Process.Start("Login.exe");

        var automation = new UIA3Automation();
        var app = FlaUI.Core.Application.Attach(process);
        var mainWindow = app.GetMainWindow(automation).WaitUntilEnabled(new TimeSpan(0, 0, 0, 0, 100));

        var usernameBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameBox")).AsTextBox();
        var passwordBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox();
        var loginButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")).AsButton();

        // Act.
        usernameBox.Text = "admin";
        passwordBox.Text = "password";

        loginButton.Push();
        Thread.Sleep(5000);
        loginButton.Release();

        // Assert
        mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard")).WaitUntilEnabled();
        var dashboardPanel = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard"));
        Assert.NotNull(dashboardPanel);
        mainWindow.Close();
    }

    [Fact]
    [Trait("Category", "UI Test")]
    public void Logout_ShouldNavigateToDashboard()
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
        var lougoutButotn = mainWindow.FindFirstDescendant(cf => cf.ByName("Logout").And(cf.ByControlType(ControlType.Button))).AsButton();
        Assert.NotNull(dashboardPanel);
        mainWindow.Close();
    }

    [Fact]
    [Trait("Category", "UI Test")]
    public void Login_WithWrongPassword_ShouldShowPopup()
    {
        // Arrange: Start WPF application
        var process = Process.Start("Login.exe");

        var automation = new UIA3Automation();
        var app = FlaUI.Core.Application.Attach(process);
        var mainWindow = app.GetMainWindow(automation).WaitUntilEnabled(new TimeSpan(0, 0, 0, 0, 100));

        var usernameBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameBox")).AsTextBox();
        var passwordBox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox();
        var loginButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")).AsButton();

        // Act
        usernameBox.Text = "admin";
        passwordBox.Text = "wrongpassword";
        loginButton.Push();
        Thread.Sleep(1000);
        loginButton.Release();

        // Assert: Check for popup element "Login Failed" and button with AutomationId "2" inside mainWindow
        var popup = mainWindow.FindFirstDescendant(cf => cf.ByName("Login Failed"));
        Assert.NotNull(popup);

        var popupButton = popup.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
        Assert.NotNull(popupButton);

        popupButton.Click();

        mainWindow.Close();
    }
}
