using UnityEngine;

public class OGearVRCardThrower : CardThrower
{
    // Should be OVRInput.Controller.LTrackedRemote or OVRInput.Controller.RTrackedRemote.
    [SerializeField]
    protected OVRInput.Controller m_controller;

    [SerializeField]
    protected Vector3 m_anchorOffsetPos;
    [SerializeField]
    protected Vector3 m_anchorOffsetRot;

    // Temporary velocity until OVRPlugin includes Gear VR Controller velocity
    private Vector3 m_controllerVelocity = Vector3.zero;
    private Vector3 m_lastPosition = Vector3.zero;

    protected void CalculateControllerVelocity()
    {
        // Calculate controller velocity
        Vector3 _deltaDist = transform.position - m_lastPosition;
        m_controllerVelocity = (_deltaDist / Time.fixedDeltaTime);
        m_lastPosition = transform.position;
    }

    protected override void Update()
    {
        m_isDrawing = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller);
        m_isThrowing = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_controller);
        base.Update();
    }


    protected override void FixedUpdate()
    {
        CalculateControllerVelocity();
        base.FixedUpdate();
    }

    protected override void ThrowCard()
    {
        if (m_grabbedObj != null)
        {
            OVRPose trackingSpace = GetTrackingSpace();

            // Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
            // Temporary velocity until OVRPlugin includes Gear VR Controller velocity
            Vector3 linearVelocity = trackingSpace.orientation * transform.forward * m_controllerVelocity.magnitude * m_throwSpeedMultiplier;
            m_grabbedObj.GetComponent<CardComponent>().Throw(linearVelocity);
            m_grabbedObj = null;
            m_isThrowing = false;
        }
    }

    private OVRPose GetTrackingSpace()
    {
        OVRPose localPose = new OVRPose
        {
            position = OVRInput.GetLocalControllerPosition(m_controller),
            orientation = OVRInput.GetLocalControllerRotation(m_controller)
        };
        OVRPose offsetPose = new OVRPose {
            position = m_anchorOffsetPos,
            orientation = Quaternion.Euler(m_anchorOffsetRot)
        };
        localPose = localPose * offsetPose;

        OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
        return trackingSpace;
    }
}
