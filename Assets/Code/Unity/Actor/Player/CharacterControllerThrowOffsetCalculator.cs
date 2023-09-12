using System.Linq;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// When throwing objects as the player it can become important to ignore the character's velocity which gets added to the rigidbody being thrown.
    /// This component calculates the throw offset based on the character controller average velocity.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerThrowOffsetCalculator : MonoBehaviour
    {
        [SerializeField]
        private int nFramesToTrack = 32;


        private CharacterController characterController;
        private Vector3[] characterControllerVelocityFrames;
        private int currentCharacterControllerVelocityIndex = 0;

        /// <summary>
        /// Do not add player velocity to throw to make things more
        /// predictable for the player.
        /// </summary>
        public Vector3 Calculate(XRGrabInteractable grabInteractable)
        {
            var velocitySum = characterControllerVelocityFrames.Aggregate(Vector3.zero, (acc, v) => v == null ? acc : acc += v);
            var averageCCVelocity = velocitySum / characterControllerVelocityFrames.Length;
            var offset = -averageCCVelocity * grabInteractable.throwVelocityScale;
            return offset;
        }

        public void RecordVelocity()
        {
            currentCharacterControllerVelocityIndex = (currentCharacterControllerVelocityIndex + 1) % nFramesToTrack;
            characterControllerVelocityFrames[currentCharacterControllerVelocityIndex] = characterController.velocity;
        }

        private void Awake()
        {
            characterControllerVelocityFrames = new Vector3[nFramesToTrack];
            characterController = GetComponent<CharacterController>();
        }
    }
}
