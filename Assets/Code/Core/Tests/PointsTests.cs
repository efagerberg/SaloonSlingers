using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using NUnit.Framework;

using SaloonSlingers.Core;

public class PointsTests
{
    [Test]
    public void TestPointsSetButUnchangedDoesNotEmitEvent()
    {
        var subject = new Points(2);
        void handler(Points sender, ValueChangeEvent<uint> e)
        {
            Assert.That(true == false);
        }
        subject.OnPointsChanged += handler;
        subject.Value = subject.Value;
    }

    [Test]
    public void TestPointsChangedEmitsEvent()
    {
        var subject = new Points(2);
        bool eventHandled = false;
        void handler(Points sender, ValueChangeEvent<uint> e)
        {
            eventHandled = true;
            Assert.AreEqual(e.Before, 2);
            Assert.AreEqual(e.After, 1);
        }
        subject.OnPointsChanged += handler;
        subject.Value--;

        Assert.That(eventHandled);
    }

    [Test]
    public void TestPointsPastMaxIsClampedToMax()
    {
        var subject = new Points(2);
        subject.Value *= 2;
        
        Assert.That(subject.Value, Is.EqualTo(2));
    }
}