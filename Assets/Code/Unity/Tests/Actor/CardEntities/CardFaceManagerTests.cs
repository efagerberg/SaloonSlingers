using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests
{
    public class CardFaceManagerTests
    {

        [TestCase("AH")]
        [TestCase("TD")]
        public void SetTexture_SetsExpectedTexture_BasedOnCard(string cardString)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var renderer = TestUtils.CreateComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            var subject = new CardFaceManager(renderer);
            var card = new Core.Card(cardString);
            subject.SetTexture(card);

            Assert.That(renderer.sharedMaterial.mainTexture.name, Is.EqualTo(card.ToString()));
        }

        [Test]
        public void SetColor_SetsColor_OfManagedMaterial()
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var renderer = TestUtils.CreateComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            var subject = new CardFaceManager(renderer);
            subject.SetColor(Color.red);

            Assert.That(renderer.sharedMaterial.color, Is.EqualTo(Color.red));
        }
    }
}
