using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

public class VelocityAverageCalculatorTests
{
    [Test]
    public void WhenNoVelocityRecorded_CalculatesNoVelocity()
    {
        var subject = new VelocityAverageCalculator();

        Assert.That(subject.Calculate(), Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void WhenVelocitiesRecorded_CalculateCorrectVelocityAvg()
    {
        int nFrames = 2;
        var subject = new VelocityAverageCalculator(nFrames);
        subject.Record(new Vector3(1, 2, 3));
        subject.Record(new Vector3(4, 5, 6));
        var expected = (new Vector3(5, 7, 9) / nFrames);

        Assert.That(subject.Calculate(), Is.EqualTo(expected));
    }

    [Test]
    public void WhenEnoughVelocitiesRecorded_CalculatesAverageViaOverridingVelocityRecord()
    {
        var subject = new VelocityAverageCalculator(1);
        subject.Record(new Vector3(1, 2, 3));
        var last = new Vector3(4, 5, 6);
        subject.Record(last);

        Assert.That(last, Is.EqualTo(subject.Calculate()));
    }
}
