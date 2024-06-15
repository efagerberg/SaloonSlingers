using System;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    // Initially was meant specifically for CharacterController but CharacterController.velocity
    // is not reliable it seems
    public class VelocityTracker : MonoBehaviour
    {
        public Vector3 AverageVelocity { get => velocityAverageCalculator.Calculate(); }

        [SerializeField]
        [Range(0, 32)]
        private int nVelocityFramesToTrack = 20;

        private VelocityAverageCalculator velocityAverageCalculator;
        private Vector3 lastPosition;
        private Vector3 currentPosition;

        private void Awake()
        {
            currentPosition = lastPosition = transform.position;
            velocityAverageCalculator = new(nVelocityFramesToTrack);
        }

        private void Update()
        {
            currentPosition = transform.position;
            velocityAverageCalculator.Record((currentPosition - lastPosition) / Time.deltaTime);
            lastPosition = currentPosition;
        }
    }
}
