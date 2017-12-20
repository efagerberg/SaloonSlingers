using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour, IPlayerMotor
{
    public IPlayerController PlayerController { get; private set; }

    private Rigidbody m_rb;
    private OVRInput.Controller m_controller;

    // Use this for initialization
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        PlayerController = new PlayerController(GetComponent<IPlayerStatsManager>().PlayerStats);
        m_controller = OVRInput.GetActiveController();
    }

    private void Update()
    {
        if (PauseMenu.IsOn)
        {
            PlayerController.HandlePause();
            return;
        }

        //bool running = OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
        //Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        //PlayerController.HandleMovement(transform, touchPosition, running);
        PlayerController.HandleMovement(transform, Vector3.zero, false);
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void PerformMovement()
    {
        if (PlayerController.Velocity != Vector3.zero)
        {
            m_rb.MovePosition(m_rb.position + PlayerController.Velocity * Time.fixedDeltaTime);
        }
    }
}