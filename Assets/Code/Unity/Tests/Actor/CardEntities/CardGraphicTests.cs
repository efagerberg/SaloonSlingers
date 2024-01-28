using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class CardGraphicTests
    {
        [Test]
        public void Kill_TriggersDeathEvent()
        {
            var subject = TestUtils.CreateComponent<CardGraphic>();
            var eventEmitted = false;
            void OnDeath(object sender, System.EventArgs e)
            {
                eventEmitted = true;
            }
            subject.Death += OnDeath;
            subject.Kill();

            Assert.That(eventEmitted);
        }

        [Test]
        [RequiresPlayMode]
        public void Card_ReturnsSetCard()
        {
            var go = new GameObject();
            go.AddComponent<MeshRenderer>();
            var subject = go.AddComponent<CardGraphic>();
            var card = new Card(Values.ACE, Suits.CLUBS);
            subject.Card = card;

            Assert.That(subject.Card, Is.EqualTo(card));
        }

        [Test]
        [RequiresPlayMode]
        public void CardSetter_SetsRendererMaterial_WhenRendererAttached()
        {
            var go = new GameObject();
            var _renderer = go.AddComponent<MeshRenderer>();
            var subject = go.AddComponent<CardGraphic>();
            var card = new Card(Values.ACE, Suits.CLUBS);
            subject.Card = card;

            Assert.That(subject.FaceMaterial, Is.EqualTo(_renderer.material));
        }

        [Test]
        [RequiresPlayMode]
        public void CardSetter_UpdatesGraphicsAndName()
        {
            var go = new GameObject();
            var _renderer = go.AddComponent<MeshRenderer>();
            var subject = go.AddComponent<CardGraphic>();
            var card = new Card(Values.ACE, Suits.CLUBS);
            var expectedTexture = Resources.Load<Texture>($"Textures/{card}");
            subject.Card = card;

            Assert.That(subject.name, Is.EqualTo(card.ToUnicode()));
            Assert.That(_renderer.material.mainTexture, Is.EqualTo(expectedTexture));
        }

        [Test]
        [RequiresPlayMode]
        public void ResetActor_ResetsFaceRendererMaterial()
        {
            var go = new GameObject();
            var _renderer = go.AddComponent<MeshRenderer>();
            var subject = go.AddComponent<CardGraphic>();
            var card = new Card(Values.ACE, Suits.CLUBS);
            subject.Card = card;
            subject.ResetActor();

            Assert.That(_renderer.material.mainTexture, Is.Null);
        }
    }
}