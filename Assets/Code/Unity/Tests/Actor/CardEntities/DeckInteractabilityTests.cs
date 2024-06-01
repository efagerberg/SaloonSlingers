using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Tests
{
    class StubbedIndicator : IIndicator
    {
        public bool Hidden = false;
        public bool Allowed = false;
        public Transform transform { get; private set; }

        public StubbedIndicator()
        {
            transform = new GameObject("IndicatorTransform").transform;
        }

        public void Hide()
        {
            Hidden = true;
        }

        public void Indicate(bool allowed)
        {
            Hidden = false;
            Allowed = allowed;
        }
    }

    class StubbedCardGraphic : MonoBehaviour, ICardGraphic
    {
        public Card Card { get; set; }
        public Color Color { get; set; }

        public void Kill() { }
    }

    public class DeckInteractibilityOnHoverExitTests
    {
        [Test]
        public void HidesIndicator()
        {
            var indicator = new StubbedIndicator();
            indicator.Indicate(true);
            var deckGraphicMock = new Mock<IDeckGraphic>();
            deckGraphicMock.Setup(instance => instance.Peek()).Returns(new GameObject().transform);
            var subject = new DeckInteractability(
                indicator,
                new Mock<ISpawner<GameObject>>().Object,
                new Dictionary<AttributeType, Core.Attribute>(),
                deckGraphicMock.Object,
                new Mock<ICardGame>().Object
            );

            subject.OnHoverExit();
            Assert.That(indicator.Hidden);
        }
    }

    public class DeckInteractabilityOnHoverEnterTests
    {
        [TestCaseSource(nameof(SansProjectileInteractors))]
        public void WhenNoProjectileInvolved_IndicatesBasedOnEmptyHand(IXRSelectInteractor interactor)
        {
            var handInteractableSpawner = new Mock<ISpawner<GameObject>>();
            handInteractableSpawner.Setup(instance => instance.Spawn())
                                   .Returns(new GameObject("HandInteractable"));

            var indicator = new StubbedIndicator();
            Mock<IDeckGraphic> deckGraphicMock = CreateDeckGraphic();

            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 0);
            gameMock.Setup(instance => instance.Draw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => deckGraphicMock.Object.Deck.Draw());

            var subject = new DeckInteractability(
                indicator,
                handInteractableSpawner.Object,
                new Dictionary<AttributeType, Attribute>(),
                deckGraphicMock.Object,
                gameMock.Object
            );
            subject.OnHoverEnter(interactor);

            Assert.That(indicator.Hidden, Is.False);
            Assert.That(indicator.Allowed, Is.True);
            Assert.That(indicator.transform.position, Is.EqualTo(deckGraphicMock.Object.Peek().transform.position));
        }

        private static IEnumerable<IXRSelectInteractor> SansProjectileInteractors()
        {
            yield return null;

            var nonSelectingMock = new Mock<IXRSelectInteractor>();
            nonSelectingMock.Setup(instance => instance.isSelectActive).Returns(false);
            yield return nonSelectingMock.Object;


            var selectingMock = new Mock<IXRSelectInteractor>();
            nonSelectingMock.Setup(instance => instance.isSelectActive).Returns(true);
            nonSelectingMock.Setup(instance => instance.interactablesSelected).Returns(new List<IXRSelectInteractable>());
            yield return selectingMock.Object;
        }

        [Test]
        public void WhenSelectingAndHasInteractable_IndicateBasedOnProjectileHand()
        {
            var handInteractableSpawner = new Mock<ISpawner<GameObject>>();
            handInteractableSpawner.Setup(instance => instance.Spawn()).Returns(new GameObject("HandInteractable"));

            var indicator = new StubbedIndicator();
            Mock<IDeckGraphic> deckGraphicMock = CreateDeckGraphic();

            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count == 1);
            gameMock.Setup(instance => instance.Draw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => deckGraphicMock.Object.Deck.Draw());

            var (interactorMock, projectile) = createInteractorMock(
                deckGraphicMock.Object, gameMock.Object,
                selectingWithInteractable: true
            );

            var subject = new DeckInteractability(
                 indicator,
                 handInteractableSpawner.Object,
                 new Dictionary<AttributeType, Attribute>(),
                 deckGraphicMock.Object,
                gameMock.Object
             );
            subject.OnHoverEnter(interactorMock.Object);

            Assert.That(indicator.Hidden, Is.False);
            Assert.That(indicator.Allowed, Is.True);
            Assert.That(indicator.transform.position, Is.EqualTo(deckGraphicMock.Object.Peek().transform.position));
        }

        [Test]
        public void WhenSelectingAndDrawingWhilelHovering_UpdatesIndicator()
        {
            var handInteractableSpawner = new Mock<ISpawner<GameObject>>();
            handInteractableSpawner.Setup(instance => instance.Spawn()).Returns(new GameObject("HandInteractable"));

            var indicator = new StubbedIndicator();
            Mock<IDeckGraphic> deckGraphicMock = CreateDeckGraphic();

            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count <= 1);
            gameMock.Setup(instance => instance.Draw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => deckGraphicMock.Object.Deck.Draw());

            var (interactorMock, projectile) = createInteractorMock(
                deckGraphicMock.Object, gameMock.Object,
                selectingWithInteractable: true
            );

            var subject = new DeckInteractability(
                indicator,
                handInteractableSpawner.Object,
                new Dictionary<AttributeType, Attribute>(),
                deckGraphicMock.Object,
                gameMock.Object
            );
            subject.OnHoverEnter(interactorMock.Object);
            projectile.TryDrawCard(deckGraphicMock.Object.Spawn, gameMock.Object);

            Assert.That(indicator.Hidden, Is.False);
            Assert.That(indicator.Allowed, Is.False);
        }

        [Test]
        public void WhenSelectingAndDrawingWhilelNotHovering_DoesNotIndicate()
        {
            var handInteractableSpawner = new Mock<ISpawner<GameObject>>();
            handInteractableSpawner.Setup(instance => instance.Spawn()).Returns(new GameObject("HandInteractable"));

            var indicator = new StubbedIndicator();
            Mock<IDeckGraphic> deckGraphicMock = CreateDeckGraphic();

            var gameMock = new Mock<ICardGame>();
            gameMock.Setup(instance => instance.CanDraw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => ctx.Hand.Count <= 1);
            gameMock.Setup(instance => instance.Draw(It.IsAny<DrawContext>()))
                    .Returns((DrawContext ctx) => deckGraphicMock.Object.Deck.Draw());

            var (interactorMock, projectile) = createInteractorMock(
                deckGraphicMock.Object, gameMock.Object,
                selectingWithInteractable: true
            );

            var subject = new DeckInteractability(
                indicator,
                handInteractableSpawner.Object,
                new Dictionary<AttributeType, Attribute>(),
                deckGraphicMock.Object,
                gameMock.Object
            );
            subject.OnHoverEnter(interactorMock.Object);
            subject.OnHoverExit();
            projectile.TryDrawCard(deckGraphicMock.Object.Spawn, gameMock.Object);

            Assert.That(indicator.Hidden, Is.True);
        }

        private static (Mock<IXRSelectInteractor> interactorMock, HandProjectile projectile) createInteractorMock(
            IDeckGraphic deckGraphic,
            ICardGame game,
            int nCards = 0,
            bool selectingWithInteractable = false,
            bool selectingWithoutInteractable = false
        )
        {
            var interactableMock = new Mock<IXRSelectInteractable>();
            var interactableGO = new GameObject("Interactable");
            var projectile = interactableGO.AddComponent<HandProjectile>();
            projectile.TryDrawCard(deckGraphic.Spawn, game);
            var interactorMock = new Mock<IXRSelectInteractor>();
            interactableMock.Setup(instance => instance.transform).Returns(interactableGO.transform);
            interactorMock.Setup(instance => instance.isSelectActive).Returns(selectingWithInteractable || selectingWithoutInteractable);

            var interactables = new List<IXRSelectInteractable>();
            if (selectingWithInteractable)
                interactables.Add(interactableMock.Object);
            interactorMock.Setup(instance => instance.interactablesSelected)
                          .Returns(interactables);

            while (nCards > 0)
            {
                projectile.TryDrawCard(deckGraphic.Spawn, game);
                nCards--;
            }
            return (interactorMock, projectile);
        }

        private static Mock<IDeckGraphic> CreateDeckGraphic(int deckSize = 3)
        {
            var deckGraphicMock = new Mock<IDeckGraphic>();
            deckGraphicMock.Setup(instance => instance.Deck).Returns(new Deck(deckSize));
            var topCard = new GameObject("TopCard");
            topCard.transform.position = new Vector3(1, 3, 2);
            deckGraphicMock.Setup(instance => instance.Peek()).Returns(topCard.transform);
            static GameObject spawnCard()
            {
                var card = new GameObject("Card");
                var x = card.AddComponent<StubbedCardGraphic>();
                return card;
            }
            deckGraphicMock.Setup(instance => instance.Spawn()).Returns(spawnCard);
            return deckGraphicMock;
        }
    }
}