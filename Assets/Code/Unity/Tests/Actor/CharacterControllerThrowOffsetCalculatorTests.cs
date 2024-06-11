using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

public class CharacterControllerThrowOffsetCalculatorTests
{
    [Test]
    public void WhenNoVelocityRecorded_CalculatesNoOffset()
    {
        var subject = new CharacterControllerThrowOffsetCalculator();

        Assert.That(subject.Calculate(1), Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void WhenVelocitiesRecorded_CalculatesOffsetFromVelocityAvg()
    {
        int nFrames = 2;
        var subject = new CharacterControllerThrowOffsetCalculator(nFrames);
        subject.RecordVelocity(new Vector3(1, 2, 3));
        subject.RecordVelocity(new Vector3(4, 5, 6));
        var scale = 2;
        var expected = -(new Vector3(5, 7, 9) / nFrames) * scale;

        Assert.That(subject.Calculate(scale), Is.EqualTo(expected));
    }

    [Test]
    public void WhenEnoughVelocitiesRecorded_CalculatesOffsetViaOverridingVelocityRecord()
    {
        var subject = new CharacterControllerThrowOffsetCalculator(1);
        var scale = 2;
        subject.RecordVelocity(new Vector3(1, 2, 3));
        var last = new Vector3(4, 5, 6);
        var expected = scale * -last;
        subject.RecordVelocity(last);

        Assert.That(expected, Is.EqualTo(subject.Calculate(scale)));
    }
}
