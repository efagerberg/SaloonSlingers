using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour, IPlayerMotor
{
    private IPlayerController m_playerController;
    public IPlayerController PlayerController { get { return m_playerController; } }

    private Rigidbody m_rb;

    private OVRInput.Controller m_controller;

    // Use this for initialization
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_playerController = new PlayerController(GetComponent<IPlayerStatsManager>().PlayerStats);
        m_controller = OVRInput.GetActiveController();
    }

    private void Update()
    {
        if (PauseMenu.IsOn)
        {
            m_playerController.HandlePause();
            return;
        }

        if (m_playerController == null) return;

        Vector2 touchPosition = Vector2.zero;
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
            touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        m_playerController.HandleMovement(transform, touchPosition, Input.GetKey(KeyCode.LeftShift));
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if (m_playerController.Velocity != Vector3.zero)
        {
            m_rb.MovePosition(m_rb.position + m_playerController.Velocity * Time.fixedDeltaTime);
        }
    }
}