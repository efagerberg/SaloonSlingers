using System.Collections;
using System.Linq;

using NUnit.Framework;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class EnemyTests
    {
        [UnityTest]
        public IEnumerator InitializesEnemyDeckAndAttributes_OnStart()
        {
            var subject = TestUtils.CreateComponent<Enemy>();
            var attributes = subject.gameObject.AddComponent<Attributes>();
            attributes.Registry[AttributeType.Health] = new Attribute(2);
            subject.runInEditMode = true;
            yield return null;

            Assert.That(subject.AttributeRegistry, Is.EqualTo(attributes.Registry));
            Assert.That(subject.Deck.Count, Is.GreaterThan(0));
        }

        [UnityTest]
        public IEnumerator ResetAttributes_ResetsAttributesAndDeck()
        {
            var subject = TestUtils.CreateComponent<Enemy>();
            var attributes = subject.gameObject.AddComponent<Attributes>();
            attributes.Registry[AttributeType.Health] = new Attribute(2);
            subject.runInEditMode = true;
            yield return null;
            var deckBefore = subject.Deck;
            attributes.Registry[AttributeType.Health].Decrease(1);
            subject.ResetAttributes();

            Assert.That(deckBefore, Is.Not.EqualTo(subject.Deck));
            Assert.That(subject.AttributeRegistry.Values.Select(a => a.Value), Is.EqualTo(subject.AttributeRegistry.Values.Select(x => x.InitialValue)));
        }
    }
}
