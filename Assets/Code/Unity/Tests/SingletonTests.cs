using System.Linq;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.Unity.Tests
{
    public class TestSingleton
    {
        [Test]
        [RequiresPlayMode]
        public void CreatesNewInstance_WhenOneDoesNotExist()
        {
            TestUtils.CreateComponent<ASingleton>();

            Assert.That(GameObject.FindObjectsOfType<ASingleton>().Count(), Is.EqualTo(1));
        }
    }

    class ASingleton : Singleton<MonoBehaviour> { }
}