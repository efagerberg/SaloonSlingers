using System;
using System.Collections.Generic;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.Unity.Tests
{
    public class CardHandStatusTypeTests
    {
        [Test]
        public void Getter_DefaultsToDamageMode()
        {
            var subject = CardHandStatusTypeTestHelper.BuildSubject();

            Assert.That(subject.Current, Is.EqualTo(StatusType.Damage));
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(2)]
        public void Setter_DoesNotSetCursed_WhenHandDoesNotHave1Card(int nCards)
        {
            var subject = CardHandStatusTypeTestHelper.BuildSubject();
            var hand = subject.GetComponent<CardHand>();
            for (int i = 0; i < nCards; i++)
            {
                hand.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            }
            subject.Current = StatusType.Curse;

            Assert.That(subject.Current, Is.EqualTo(StatusType.Damage));
        }

        [Test]
        public void Setter_SetsCursed_WhenHandOnlyHas1Card()
        {
            var subject = CardHandStatusTypeTestHelper.BuildSubject();
            var hand = subject.GetComponent<CardHand>();
            hand.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            subject.Current = StatusType.Curse;

            Assert.That(subject.Current, Is.EqualTo(StatusType.Curse));
        }

        [Test]
        public void Setter_AlwaysChangingToDamageMode()
        {
            var subject = CardHandStatusTypeTestHelper.BuildSubject();
            var hand = subject.GetComponent<CardHand>();
            hand.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            subject.Current = StatusType.Curse;
            subject.Current = StatusType.Damage;
    
            Assert.That(subject.Current, Is.EqualTo(StatusType.Damage));
        }

        [Test]
        public void ChangesModeBackToDamage_WhenDrawResultsInMoreThanOneCard()
        {
            var subject = CardHandStatusTypeTestHelper.BuildSubject();
            var hand = subject.GetComponent<CardHand>();
            hand.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);
            subject.Current = StatusType.Curse;
            hand.TryDrawCard(HandTestHelper.TestCardSpawner, HandTestHelper.TestPokerGame);

            Assert.That(subject.Current, Is.EqualTo(StatusType.Damage));
        }
    }

    public static class CardHandStatusTypeTestHelper
    {
        public static CardHandStatusType BuildSubject(int nCards = 10)
        {
            var subject = TestUtils.CreateComponent<CardHandStatusType>();
            var hand = subject.gameObject.AddComponent<CardHand>();
            subject.runInEditMode = true;
            hand.runInEditMode = true;
            hand.Assign(new Deck(nCards), new Dictionary<AttributeType, Core.Attribute> { });
            return subject;
        }
    }
}
