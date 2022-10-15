using System.Collections.Generic;
using UnityEngine;

namespace SaloonSlingers.Unity.Aerodynamics
{
    [RequireComponent(typeof(Rigidbody))]
    public class AeroPhysics : MonoBehaviour
    {
        const float PREDICTION_TIMESTEP_FRACTION = 0.5f;
        [SerializeField]
        List<AeroSurface> aerodynamicSurfaces = null;

        Rigidbody rb;
        BiVector3 currentForceAndTorque;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (rb.isKinematic == true) return;

            BiVector3 forceAndTorqueThisFrame =
                CalculateAerodynamicForces(rb.velocity, rb.angularVelocity, Vector3.zero, 1.2f, rb.worldCenterOfMass);

            Vector3 velocityPrediction = PredictVelocity(forceAndTorqueThisFrame.p + Physics.gravity * rb.mass);
            Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q);

            BiVector3 forceAndTorquePrediction =
                CalculateAerodynamicForces(velocityPrediction, angularVelocityPrediction, Vector3.zero, 1.2f, rb.worldCenterOfMass);

            currentForceAndTorque = (forceAndTorqueThisFrame + forceAndTorquePrediction) * 0.5f;
            rb.AddForce(currentForceAndTorque.p);
            rb.AddTorque(currentForceAndTorque.q);
        }

        private BiVector3 CalculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, float airDensity, Vector3 centerOfMass)
        {
            BiVector3 forceAndTorque = new();
            foreach (var surface in aerodynamicSurfaces)
            {
                Vector3 relativePosition = surface.transform.position - centerOfMass;
                forceAndTorque += surface.CalculateForces(-velocity + wind
                    - Vector3.Cross(angularVelocity,
                    relativePosition),
                    airDensity, relativePosition);
            }
            return forceAndTorque;
        }

        private Vector3 PredictVelocity(Vector3 force)
        {
            return rb.velocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * force / rb.mass;
        }

        private Vector3 PredictAngularVelocity(Vector3 torque)
        {
            Quaternion inertiaTensorWorldRotation = rb.rotation * rb.inertiaTensorRotation;
            Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
            Vector3 angularVelocityChangeInDiagonalSpace = Vector3.zero;
            if (rb.inertiaTensor.x > 0)
                angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / rb.inertiaTensor.x;
            if (rb.inertiaTensor.y > 0)
                angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / rb.inertiaTensor.y;
            if (rb.inertiaTensor.z > 0)
                angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / rb.inertiaTensor.z;

            return rb.angularVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION
                * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
        }
    }


}
