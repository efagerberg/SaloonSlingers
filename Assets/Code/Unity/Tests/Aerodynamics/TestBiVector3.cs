using NUnit.Framework;

using SaloonSlingers.Unity.Aerodynamics;

using UnityEngine;

namespace SaloonSlingers.Unity.Tests.Aerodynamics
{
    public class TestBiVector3
    {
        [TestCaseSource(nameof(AddTestCases))]
        public void TestAdd(BiVector3 a, BiVector3 b, BiVector3 expected)
        {
            Assert.AreEqual(a + b, expected);
        }

        private static readonly object[][] AddTestCases = {
            new object[] {
                new BiVector3(Vector3.one, Vector3.zero),
                new BiVector3(Vector3.zero, Vector3.one),
                new BiVector3(Vector3.one, Vector3.one)
            },
            new object[] {
                new BiVector3(Vector3.one * 3, Vector3.one),
                new BiVector3(Vector3.one * -2, Vector3.one),
                new BiVector3(Vector3.one, Vector3.one * 2)
            },
            new object[]
            {
                new BiVector3(new Vector3(1, -1, 0.5f), new Vector3(-3.2f, 500f, 12.1f)),
                new BiVector3(new Vector3(-9, 1, 12.2f), new Vector3(27, 100f, -100f)),
                new BiVector3(new Vector3(-8, 0, 12.7f), new Vector3(23.8f, 600f, -87.9f)),
            }
        };

        [TestCaseSource(nameof(MultiplyCases))]
        public void TestMultiplyByBiVector3(float f, BiVector3 a, BiVector3 expected)
        {
            Assert.AreEqual(f * a, expected);
        }

        private static readonly object[][] MultiplyCases = {
            new object[] {
                0f,
                new BiVector3(Vector3.one, Vector3.zero),
                new BiVector3(Vector3.zero, Vector3.zero)
            },
            new object[] {
                3f,
                new BiVector3(Vector3.one * 3, Vector3.one),
                new BiVector3(Vector3.one * 9, Vector3.one * 3)
            },
        };

        [TestCaseSource(nameof(MultiplyCases))]
        public void TestMultiplyByFloat(float f, BiVector3 a, BiVector3 expected)
        {
            Assert.AreEqual(a * f, expected);
        }
    }
}

