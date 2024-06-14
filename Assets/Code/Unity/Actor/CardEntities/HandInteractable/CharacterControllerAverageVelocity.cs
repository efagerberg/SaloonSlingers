using System;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerAverageVelocity : MonoBehaviour
    {
        public Vector3 AverageVelocity { get => velocityAverageCalculator.Calculate(); }

        [SerializeField]
        [Range(0, 32)]
        private int nVelocityFramesToTrack = 20;

        private VelocityAverageCalculator velocityAverageCalculator;
        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            velocityAverageCalculator = new(nVelocityFramesToTrack);
        }

        private void FixedUpdate()
        {
            // Use the consistent update to avoid variable timing
            velocityAverageCalculator.Record(characterController.velocity);
        }
    }
}