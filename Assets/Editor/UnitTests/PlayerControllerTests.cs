using UnityEngine;

using NSubstitute;
using NUnit.Framework;

public class PlayerControllerTests
{
    IPlayerStats mockStats = Substitute.For<IPlayerStats>();
    IPlayerController pControllerUnderTest;

    [SetUp]
    public void Init()
    {
        pControllerUnderTest = new PlayerController(mockStats);
        mockStats.Stamina.Returns(100);
    }

    [TearDown]
    public void Dispose()
    {
        mockStats.ClearReceivedCalls();
    }

    [Test]
    public void HandlePause_StopsPlayerMovement()
    {
        pControllerUnderTest.HandlePause();

        Assert.That(pControllerUnderTest.Velocity == Vector3.zero);
    }

    [Test]
    public void HandleMovement_WhileRunning_TellsTheMotorToGoFast()
    {
        var transform = new GameObject().transform;
        pControllerUnderTest.HandleMovement(transform, new Vector2(1f, 0f), true);

        var expectedVelocityVector = new Vector3(3f, 0f, 0f);
        Assert.AreEqual(pControllerUnderTest.Velocity, expectedVelocityVector);
    }

    [Test]
    public void HandleMovement_WhileRunningWithNoStamina_TellsTheMotorToGoFast()
    {
        mockStats.Stamina.Returns(0);
        var transform = new GameObject().transform;
        pControllerUnderTest.HandleMovement(transform, new Vector2(1f, 0f), true);

        var expectedVelocityVector = new Vector3(1f, 0f, 0f);
        Assert.AreEqual(pControllerUnderTest.Velocity, expectedVelocityVector);
    }

    [Test]
    public void HandleMovement_WhileWalking_TellsTheMotorToGoSlower()
    {
        var transform = new GameObject().transform;
        pControllerUnderTest.HandleMovement(transform, new Vector2(1f, 0f), false);

        var expectedVelocityVector = new Vector3(1f, 0f, 0f);
        Assert.AreEqual(pControllerUnderTest.Velocity, expectedVelocityVector);
    }

    [Test]
    public void HandleMovement_WhileIdle_TellsTheMotorToStop()
    {
        var transform = new GameObject().transform;
        pControllerUnderTest.HandleMovement(transform, new Vector2(0f, 0f), false);

        var expectedVelocityVector = Vector3.zero;
        Assert.That(pControllerUnderTest.Velocity == expectedVelocityVector);
    }
}
