using NUnit.Framework;
using System.Collections.Generic;

public class PlayerStatsTests
{
    PlayerStats playerStatsUnderTests;
    Dictionary<string, bool> inputs;

    [SetUp]
    public void Init()
    {
        playerStatsUnderTests = new PlayerStats();
        inputs = new Dictionary<string, bool>();
        inputs["IsRunning"] = false;
        inputs["IsJumping"] = false;
        PauseMenu.IsOn = false;
    }

    [Test]
    public void Constructor_ShouldSetExpectedDefaultStats()
    {
        var expectedStatValue = 1f;
        Assert.AreEqual(playerStatsUnderTests.Health, expectedStatValue);
        Assert.AreEqual(playerStatsUnderTests.Stamina, expectedStatValue);
        Assert.AreEqual(playerStatsUnderTests.IsDead, false);
    }

    [Test]
    public void Update_ShouldRegenStats_WhenStatsLessThanMax()
    {
        playerStatsUnderTests.EmptyStats();
        Assert.AreEqual(playerStatsUnderTests.Health, 0);
        Assert.AreEqual(playerStatsUnderTests.Stamina, 0);

        playerStatsUnderTests.Update(inputs, 0.5f);
        Assert.That(playerStatsUnderTests.Health > 0);
        Assert.That(playerStatsUnderTests.Stamina > 0);
    }

    [Test]
    public void Update_ShouldNotRegenStats_PastMaximum()
    {
        var max = 1f;
        playerStatsUnderTests.Update(inputs, 1f);
        Assert.AreEqual(playerStatsUnderTests.Health, max);
        Assert.AreEqual(playerStatsUnderTests.Stamina, max);
    }

    [Test]
    public void Update_ShouldConsumeStats_GivenInputs()
    {
        inputs["IsRunning"] = true;
        playerStatsUnderTests.Update(inputs, 0.5f);
        Assert.That(playerStatsUnderTests.Stamina < 1f);
    }

    [Test]
    public void Update_ShouldNotConsumeStats_GivenInputsButPaused()
    {
        PauseMenu.IsOn = true;
        inputs["IsRunning"] = true;
        playerStatsUnderTests.Update(inputs, 0.5f);
        Assert.That(playerStatsUnderTests.Stamina == 1f);
    }

    [Test]
    public void TakeDamage_ShouldNotSet_HealthNegative()
    {
        playerStatsUnderTests.TakeDamage(10000000);
        Assert.That(playerStatsUnderTests.Health == 0);
    }

    [Test]
    public void TakeDamage_ShouldSetIsDead_WhenHealthIsZero()
    {
        playerStatsUnderTests.TakeDamage(100);
        Assert.That(playerStatsUnderTests.Health == 0);
        Assert.That(playerStatsUnderTests.IsDead == true);
    }
}
