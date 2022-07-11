﻿using UnityEngine;

namespace SaloonSlingers.Unity.Aerodynamics
{
    public class AeroSurface : MonoBehaviour
    {
        [SerializeField] AeroSurfaceConfig config = null;

        public BiVector3 CalculateForces(Vector3 worldAirVelocity, float airDensity, Vector3 relativePosition)
        {
            BiVector3 forceAndTorque = new();
            if (!gameObject.activeInHierarchy || config == null) return forceAndTorque;

            // Accounting for aspect ratio effect on lift coefficient.
            float correctedLiftSlope = config.liftSlope * config.aspectRatio /
               (config.aspectRatio + 2 * (config.aspectRatio + 4) / (config.aspectRatio + 2));

            float zeroLiftAoA = config.zeroLiftAoA * Mathf.Deg2Rad;

            // Calculating air velocity relative to the surface's coordinate system.
            // Z component of the velocity is discarded. 
            Vector3 airVelocity = transform.InverseTransformDirection(worldAirVelocity);
            airVelocity = new Vector3(airVelocity.x, airVelocity.y);
            Vector3 dragDirection = transform.TransformDirection(airVelocity.normalized);
            Vector3 liftDirection = Vector3.Cross(dragDirection, transform.forward);

            float area = config.chord * config.span;
            float dynamicPressure = 0.5f * airDensity * airVelocity.sqrMagnitude;
            float angleOfAttack = Mathf.Atan2(airVelocity.y, -airVelocity.x);

            Vector3 aerodynamicCoefficients = CalculateCoefficients(angleOfAttack,
                                                                    correctedLiftSlope,
                                                                    zeroLiftAoA,
                                                                    zeroLiftAoA,
                                                                    zeroLiftAoA);

            Vector3 lift = aerodynamicCoefficients.x * area * dynamicPressure * liftDirection;
            Vector3 drag = aerodynamicCoefficients.y * area * dynamicPressure * dragDirection;
            Vector3 torque = aerodynamicCoefficients.z * area * config.chord * dynamicPressure * -transform.forward;

            forceAndTorque.p += lift + drag;
            forceAndTorque.q += Vector3.Cross(relativePosition, forceAndTorque.p);
            forceAndTorque.q += torque;

            return forceAndTorque;
        }

        private Vector3 CalculateCoefficients(float angleOfAttack,
                                              float correctedLiftSlope,
                                              float zeroLiftAoA,
                                              float stallAngleHigh,
                                              float stallAngleLow)
        {
            Vector3 aerodynamicCoefficients;

            // Low angles of attack mode and stall mode curves are stitched together by a line segment. 
            float paddingAngleHigh = Mathf.Deg2Rad * Mathf.Lerp(15, 5, 1 / 2);
            float paddingAngleLow = Mathf.Deg2Rad * Mathf.Lerp(15, 5, - 1 / 2);
            float paddedStallAngleHigh = stallAngleHigh + paddingAngleHigh;
            float paddedStallAngleLow = stallAngleLow - paddingAngleLow;

            if (angleOfAttack < stallAngleHigh && angleOfAttack > stallAngleLow)
            {
                // Low angle of attack mode.
                aerodynamicCoefficients = CalculateCoefficientsAtLowAoA(angleOfAttack, correctedLiftSlope, zeroLiftAoA);
            }
            else
            {
                if (angleOfAttack > paddedStallAngleHigh || angleOfAttack < paddedStallAngleLow)
                {
                    // Stall mode.
                    aerodynamicCoefficients = CalculateCoefficientsAtStall(
                        angleOfAttack, correctedLiftSlope, zeroLiftAoA, stallAngleHigh, stallAngleLow);
                }
                else
                {
                    // Linear stitching in-between stall and low angles of attack modes.
                    Vector3 aerodynamicCoefficientsLow;
                    Vector3 aerodynamicCoefficientsStall;
                    float lerpParam;

                    if (angleOfAttack > stallAngleHigh)
                    {
                        aerodynamicCoefficientsLow = CalculateCoefficientsAtLowAoA(stallAngleHigh, correctedLiftSlope, zeroLiftAoA);
                        aerodynamicCoefficientsStall = CalculateCoefficientsAtStall(
                            paddedStallAngleHigh, correctedLiftSlope, zeroLiftAoA, stallAngleHigh, stallAngleLow);
                        lerpParam = (angleOfAttack - stallAngleHigh) / (paddedStallAngleHigh - stallAngleHigh);
                    }
                    else
                    {
                        aerodynamicCoefficientsLow = CalculateCoefficientsAtLowAoA(stallAngleLow, correctedLiftSlope, zeroLiftAoA);
                        aerodynamicCoefficientsStall = CalculateCoefficientsAtStall(
                            paddedStallAngleLow, correctedLiftSlope, zeroLiftAoA, stallAngleHigh, stallAngleLow);
                        lerpParam = (angleOfAttack - stallAngleLow) / (paddedStallAngleLow - stallAngleLow);
                    }
                    aerodynamicCoefficients = Vector3.Lerp(aerodynamicCoefficientsLow, aerodynamicCoefficientsStall, lerpParam);
                }
            }
            return aerodynamicCoefficients;
        }

        private Vector3 CalculateCoefficientsAtLowAoA(float angleOfAttack,
                                                      float correctedLiftSlope,
                                                      float zeroLiftAoA)
        {
            float liftCoefficient = correctedLiftSlope * (angleOfAttack - zeroLiftAoA);
            float inducedAngle = liftCoefficient / (Mathf.PI * config.aspectRatio);
            float effectiveAngle = angleOfAttack - zeroLiftAoA - inducedAngle;

            float tangentialCoefficient = config.skinFriction * Mathf.Cos(effectiveAngle);

            float normalCoefficient = (liftCoefficient +
                Mathf.Sin(effectiveAngle) * tangentialCoefficient) / Mathf.Cos(effectiveAngle);
            float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAngle) + tangentialCoefficient * Mathf.Cos(effectiveAngle);
            float torqueCoefficient = -normalCoefficient * TorqCoefficientProportion(effectiveAngle);

            return new Vector3(liftCoefficient, dragCoefficient, torqueCoefficient);
        }

        private Vector3 CalculateCoefficientsAtStall(float angleOfAttack,
                                                     float correctedLiftSlope,
                                                     float zeroLiftAoA,
                                                     float stallAngleHigh,
                                                     float stallAngleLow)
        {
            float liftCoefficientLowAoA;
            if (angleOfAttack > stallAngleHigh)
            {
                liftCoefficientLowAoA = correctedLiftSlope * (stallAngleHigh - zeroLiftAoA);
            }
            else
            {
                liftCoefficientLowAoA = correctedLiftSlope * (stallAngleLow - zeroLiftAoA);
            }
            float inducedAngle = liftCoefficientLowAoA / (Mathf.PI * config.aspectRatio);

            float lerpParam;
            if (angleOfAttack > stallAngleHigh)
            {
                lerpParam = (Mathf.PI / 2 - Mathf.Clamp(angleOfAttack, -Mathf.PI / 2, Mathf.PI / 2))
                    / (Mathf.PI / 2 - stallAngleHigh);
            }
            else
            {
                lerpParam = (-Mathf.PI / 2 - Mathf.Clamp(angleOfAttack, -Mathf.PI / 2, Mathf.PI / 2))
                    / (-Mathf.PI / 2 - stallAngleLow);
            }
            inducedAngle = Mathf.Lerp(0, inducedAngle, lerpParam);
            float effectiveAngle = angleOfAttack - zeroLiftAoA - inducedAngle;

            float tangentialCoefficient = 0.5f * config.skinFriction * Mathf.Cos(effectiveAngle);

            float liftCoefficient =  Mathf.Cos(effectiveAngle) - tangentialCoefficient * Mathf.Sin(effectiveAngle);
            float dragCoefficient =  Mathf.Sin(effectiveAngle) + tangentialCoefficient * Mathf.Cos(effectiveAngle);
            float torqueCoefficient = -TorqCoefficientProportion(effectiveAngle);

            return new Vector3(liftCoefficient, dragCoefficient, torqueCoefficient);
        }

        private float TorqCoefficientProportion(float effectiveAngle)
        {
            return 0.25f - 0.175f * (1 - 2 * Mathf.Abs(effectiveAngle) / Mathf.PI);
        }

        private float LiftCoefficientMax()
        {
            return Mathf.Clamp01(1 - 0.5f * -0.1f / 0.3f);
        }
    }
}
