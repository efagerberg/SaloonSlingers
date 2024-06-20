using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.Unity.Tests
{
    public class DeckInteractibilityOnHoverExitTests
    {
        [Test]
        public void ReturnsNull()
        {
            var subject = new DeckInteractabilityDetector(
                new Dictionary<AttributeType, Attribute>(),
                new Deck(1),
                new Mock<ICardGame>().Object
            );

            var result = subject.OnHoverExit();
            Assert.That(result, Is.Null);
        }
    }

    public class DeckInteractabilityOnHoverEnterTests
    {
        [Test]
        public void WhenProjectileNull_ReturnsBasedOnEmptyHand()
        {
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 0);

            var subject = new DeckInteractabilityDetector(
                new Dictionary<AttributeType, Attribute>(),
                new Deck(2),
                gameMock.Object
            );
            var result = subject.OnHoverEnter(null);

            Assert.That(result, Is.True);
        }

        [Test]
        public void WhenProjectileNotNull_IndicateBasedOnProjectileHand()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var projectile = TestUtils.CreateComponent<CardHand>();
            var result = subject.OnHoverEnter(projectile);

            Assert.That(result, Is.False);
        }
    }

    public class DeckInteractabilityOnDrawnTests
    {
        [Test]
        public void WhenProjectileNull_ReturnsNull()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var result = subject.OnDrawn(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenNotHovering_ReturnsNull()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var projectile = TestUtils.CreateComponent<CardHand>();
            subject.OnHoverExit();
            var result = subject.OnDrawn(projectile);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenHovering_IndicateBasedOnProjectileHand()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 0);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var projectile = TestUtils.CreateComponent<CardHand>();
            subject.OnHoverEnter(projectile);
            var result = subject.OnDrawn(projectile);

            Assert.That(result, Is.True);
        }
    }

    public class DeckInteractabilityOnThrown
    {
        [Test]
        public void WhenProjectilNull_ReturnsNull()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var result = subject.OnThrown(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenNotHovering_ReturnsNull()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var projectile = TestUtils.CreateComponent<CardHand>();
            subject.OnHoverExit();
            var result = subject.OnThrown(projectile);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenHovering_IndicateBasedOnProjectileHand()
        {
            var deck = new Deck(1);
            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 0);

            var subject = new DeckInteractabilityDetector(
                 new Dictionary<AttributeType, Attribute>(),
                 deck,
                gameMock.Object
             );
            var projectile = TestUtils.CreateComponent<CardHand>();
            subject.OnHoverEnter(projectile);
            var result = subject.OnThrown(projectile);

            Assert.That(result, Is.True);
        }
    }
}
