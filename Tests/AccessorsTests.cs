using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Tests.Accessors;

namespace Tests;

/// <summary>
/// Contains tests for the accessors used in the application.
/// </summary>
public class AccessorsTests
{
    /// <summary>
    /// Tests that the login process navigates to the dashboard.
    /// </summary>
    [Fact]
    public void Login_ShouldNavigateToDashboard()
    {
        // Arrange: Start the WPF application
        var process = Process.Start("Login.exe");

        var automation = new UIA3Automation();
        var app = FlaUI.Core.Application.Attach(process);
        var mainWindow = app.GetMainWindow(automation).WaitUntilEnabled(new TimeSpan(0, 0, 0, 0, 100));

        var usernameBox = new TextBoxAccessor(mainWindow, "UsernameBox");
        var passwordBox = new TextBoxAccessor(mainWindow, "PasswordBox");
        var loginButton1 = new ButtonAccessor(mainWindow, "LoginButton");
        //var loginButton2 = new ButtonAccessor(mainWindow, new Bitmap(".\\Images\\LoginButton.png"));
        //var loginButton3 = new ButtonAccessor(mainWindow, "Login", ControlType.Button, new Bitmap(".\\Images\\LoginButtonPressed.png"));

        // Act
        usernameBox.Text = "admin";
        passwordBox.Text = "password";

        loginButton1.Click();

        // Assert
        mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard")).WaitUntilEnabled();
        var dashboardPanel = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard"));
        Assert.NotNull(dashboardPanel);
        mainWindow.Close();
    }

    /// <summary>
    /// Tests that the user can log in, wait for 5 seconds, and then log out successfully.
    /// </summary>
    [Fact]
    public void Login_WaitAndLogout_ShouldReturnToLoginScreen()
    {
        // Arrange: Start the WPF application
        var process = Process.Start("Login.exe");

        var automation = new UIA3Automation();
        var app = FlaUI.Core.Application.Attach(process);
        var mainWindow = app.GetMainWindow(automation).WaitUntilEnabled(new TimeSpan(0, 0, 0, 0, 100));

        var usernameBox = new TextBoxAccessor(mainWindow, "UsernameBox");
        var passwordBox = new TextBoxAccessor(mainWindow, "PasswordBox");
        var loginButton = new ButtonAccessor(mainWindow, "LoginButton");

        // Act: Login
        usernameBox.Text = "admin";
        passwordBox.Text = "password";
        loginButton.Click();

        // Wait for dashboard to appear
        mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard")).WaitUntilEnabled();
        var dashboardPanel = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DashBoard"));
        Assert.NotNull(dashboardPanel);

        // Wait for 5 seconds
        Thread.Sleep(5000);

        // Logout using ButtonAccessor
        var logoutButton = new ButtonAccessor(mainWindow, "Logout", ControlType.Button);
        logoutButton.Click();

        // Assert: Should return to login screen (UsernameBox should be enabled again)
        var usernameBoxAfterLogout = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameBox")).AsTextBox();
        Assert.NotNull(usernameBoxAfterLogout);
        Assert.True(usernameBoxAfterLogout.IsEnabled);

        mainWindow.Close();
    }
}