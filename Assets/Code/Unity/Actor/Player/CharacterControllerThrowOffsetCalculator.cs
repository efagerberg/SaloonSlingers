using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// When throwing objects as the player it can become important to ignore the character's velocity which gets added to the rigidbody being thrown.
    /// This component calculates the throw offset based on the character controller average velocity.
    /// </summary>
    public class CharacterControllerThrowOffsetCalculator
    {
        private readonly int nFramesToTrack;
        private int currentCharacterControllerVelocityIndex = 0;
        private readonly IList<Vector3> characterControllerVelocityFrames;

        public CharacterControllerThrowOffsetCalculator(int nFramesToTrack = 32)
        {
            this.nFramesToTrack = nFramesToTrack;
            characterControllerVelocityFrames = new List<Vector3>();
        }

        /// <summary>
        /// Do not add player velocity to throw to make things more
        /// predictable for the player.
        /// </summary>
        public Vector3 Calculate(float throwVelocityScale)
        {
            int nFramesTracked = characterControllerVelocityFrames.Count;
            if (nFramesTracked == 0) return Vector3.zero;

            var velocitySum = characterControllerVelocityFrames.Aggregate(Vector3.zero, (acc, v) => v == null ? acc : acc += v);
            var averageCCVelocity = velocitySum / nFramesTracked;
            var offset = -averageCCVelocity * throwVelocityScale;
            return offset;
        }

        public void RecordVelocity(Vector3 velocity)
        {
            currentCharacterControllerVelocityIndex = (currentCharacterControllerVelocityIndex + 1) % nFramesToTrack;
            if (currentCharacterControllerVelocityIndex > characterControllerVelocityFrames.Count - 1)
                characterControllerVelocityFrames.Add(velocity);
            else
                characterControllerVelocityFrames[currentCharacterControllerVelocityIndex] = velocity;
        }
    }
}
