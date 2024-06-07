using Microsoft.AspNetCore.Components;
using Moq;
using Pomodoro.Components.Pages;

namespace PomodoroTests.Components.Pages;

[TestFixture]
[TestOf(typeof(Home))]
public class HomeTest
{
    [SetUp]
    public void Setup()
    {
        // Setup
        _home = new Home();
        _mock = new Mock<Home>();
    }

    private Home _home;
    private Mock<Home> _mock;

    [Test]
    public void StartTimer_StartsTimer_WhenNotRunning()
    {
        // Arrange
        var mock = new Mock<Home> { CallBase = true };
        mock.Setup(m => m.InvokeStateHasChanged()).Verifiable();
        var home = mock.Object;
        home.IsRunning = false;

        // Act
        home.StartTimer();

        Assert.Multiple(() =>
        {
            Assert.That(home.IsRunning, Is.True);
            Assert.That(home.SessionStarted, Is.True);
        });
        mock.Verify(m => m.InvokeStateHasChanged(), Times.Once);
    }

    [Test]
    public void PauseTimer_StopsTimer_WhenRunning()
    {
        // Arrange
        var mock = new Mock<Home> { CallBase = true };
        mock.Setup(m => m.InvokeStateHasChanged()).Verifiable();
        var home = mock.Object;
        home.IsRunning = true;

        // Act
        home.PauseTimer();

        // Assert
        Assert.That(home.IsRunning, Is.False);
        mock.Verify(m => m.InvokeStateHasChanged(), Times.Once);
    }

    [Test]
    public void ResetTimer_ResetsTimer()
    {
        // Arrange
        var mock = new Mock<Home> { CallBase = true };
        mock.Setup(m => m.InvokeStateHasChanged()).Verifiable();
        var home = mock.Object;
        home.IsRunning = true;
        home.Minutes = 10;
        home.Seconds = 30;

        // Act
        home.ResetTimer();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(home.IsRunning, Is.False);
            Assert.That(home.Minutes, Is.EqualTo(25));
            Assert.That(home.Seconds, Is.EqualTo(0));
        });
        mock.Verify(m => m.InvokeStateHasChanged(), Times.Once);
    }

    [Test]
    public void WorkTimeChanged_ChangesWorkTime()
    {
        // Arrange
        var e = new ChangeEventArgs { Value = "45" };

        // Act
        _home.DefineWorkAndBreakPeriods(e);

        // Assert
        Assert.That(_home.WorkTime, Is.EqualTo(45));
    }

    [Test]
    public void SaveSessions()
    {
        // Arrange
        _home.Sessions = ["Session started with 25/5"];
        _home.WorkTime = 25;
        _home.BreakTime = 5;

        // Act
        _home.SaveSessions();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_home.Sessions, Is.Not.Null);
            Assert.That(_home.Sessions, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void ExportSessions()
    {
        // Arrange
        _home.Sessions = ["Session started with 25/5"];

        // Act
        _home.ExportSessions();

        // Assert
        Assert.That(_home.Sessions, Has.Count.EqualTo(1));
    }

    [Test]
    public void DisplayTime_ShouldReturnCorrectFormat()
    {
        var mock = new Mock<Home> { CallBase = true };
        mock.Setup(m => m.InvokeStateHasChanged()).Verifiable();
        var home = mock.Object;
        home.Minutes = 5;
        home.Seconds = 30;

        var result = home.DisplayTime;

        Assert.That(result, Is.EqualTo("05:30"));
    }
}