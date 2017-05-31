using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Throw cards
/// </summary>
public class CardThrower : MonoBehaviour
{

    // Should be OVRInput.Controller.LTrackedRemote or OVRInput.Controller.RTrackedRemote.
    [SerializeField]
    protected OVRInput.Controller m_controller;

    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    private DeckComponent m_deck;

    protected GameObject m_grabbedObj = null;
    Vector3 m_grabbedObjectPosOff;
    Quaternion m_grabbedObjectRotOff;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;

    void Start()
    {
        if (m_parentTransform == null)
        {
            if (gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }

        if (m_deck == null)
        {
            var deck_go = GameObject.Find("Deck");
            m_deck = deck_go.GetComponent<DeckComponent>();
        }
    }

    void FixedUpdate()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller) && m_grabbedObj == null)
            DrawCard();
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_controller) && m_grabbedObj != null)
            Throw();
    }

    protected void Throw()
    {
        if (m_grabbedObj != null)
        {
            OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
            OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
            localPose = localPose * offsetPose;

            OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
            Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
            Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

            Rigidbody rb = m_grabbedObj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.velocity = linearVelocity;
            rb.angularVelocity = angularVelocity;

            Destroy(m_grabbedObj, 2f);
            m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;
        }
    }

    protected void DrawCard()
    {
        m_grabbedObj = m_deck.GetComponent<DeckComponent>().SpawnCard();
        m_grabbedObj.transform.SetParent(m_parentTransform);
        Rigidbody rb = m_grabbedObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        m_grabbedObj.transform.localPosition = Vector3.zero;
        m_grabbedObj.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
    }
}
