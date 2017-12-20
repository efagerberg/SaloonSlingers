using UnityEngine;

public enum ThrowMode { Straight, Curve }

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

    // Temporary velocity until OVRPlugin includes Gear VR Controller velocity
    private Vector3 m_controllerVelocity = Vector3.zero;
    private Vector3 m_lastPosition = Vector3.zero;

    // Temporary velocity until OVRPlugin includes Gear VR Controller angular velocity
    private Vector3 m_controllerAngularVelocity = Vector3.zero;
    private Vector3 m_lastRotation = Vector3.zero;

    // Used to check input on the update frame but apply physics on the fixed update frame
    private bool m_isThrowing = false;
    private bool m_isDrawing = false;

    [SerializeField]
    private ThrowMode m_throwMode = ThrowMode.Straight;

    private void Start()
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

    private void Update()
    {
        m_isDrawing = (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_controller) && m_grabbedObj == null);
        m_isThrowing = (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_controller) && m_grabbedObj != null);
    }


    private void FixedUpdate()
    {
        // Calculate controller velocity
        Vector3 _deltaDist = transform.position - m_lastPosition;
        m_controllerVelocity = (_deltaDist / Time.fixedDeltaTime);
        m_lastPosition = transform.position;

        // Calculate controller angular velocity
        Vector3 _deltaRotDist = transform.rotation.eulerAngles - m_lastRotation;
        m_controllerAngularVelocity = (_deltaRotDist / Time.fixedDeltaTime);
        m_lastRotation = transform.rotation.eulerAngles;

        // Check inputs
        if (m_isDrawing)
            DrawCard();
        else if (m_isThrowing)
            ThrowCard();
    }

    protected void ThrowCard()
    {
        if (m_grabbedObj != null)
        {
            OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller),
                                              orientation = OVRInput.GetLocalControllerRotation(m_controller) };
            OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
            localPose = localPose * offsetPose;

            OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
            // Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
            // Temporary velocity until OVRPlugin includes Gear VR Controller velocity
            Vector3 linearVelocity = trackingSpace.orientation * transform.forward * m_controllerVelocity.magnitude;
            // Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);
            // Temporary angular velocity until OVRPlugin includes Gear VR Controller angular velocity
            Vector3 angularVelocity = trackingSpace.orientation * m_controllerAngularVelocity;
 

            m_grabbedObj.GetComponent<CardComponent>().Throw(linearVelocity, angularVelocity);
            m_grabbedObj = null;

            m_isThrowing = false;
        }
    }

    protected void DrawCard()
    {
        m_grabbedObj = m_deck.SpawnCard();
        m_grabbedObj.transform.SetParent(m_parentTransform);
        m_grabbedObj.transform.localPosition = new Vector3(0, 0.05f, 0f);
        m_grabbedObj.transform.localRotation = Quaternion.Euler(0f, 90f, 45f);

        m_isDrawing = false;
    }
}
