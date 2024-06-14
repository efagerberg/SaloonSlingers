using System;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// When throwing objects as the player it can become important to ignore the character's velocity which gets added to the rigidbody being thrown.
    /// This component calculates the throw offset based on the character controller average velocity.
    /// </summary>
    public class VelocityAverageCalculator
    {
        private readonly int n;
        private int currentIndex = 0;
        private readonly Vector3[] records;

        public VelocityAverageCalculator(int n = 32)
        {
            this.n = n;
            records = new Vector3[n];
            Array.Fill(records, Vector3.zero);
        }

        public Vector3 Calculate()
        {
            var velocitySum = records.Aggregate(Vector3.zero, (acc, v) => acc += v);
            return velocitySum / n;
        }

        public void Record(Vector3 velocity)
        {
            currentIndex = (currentIndex + 1) % n;
            records[currentIndex] = velocity;
        }
    }
}
