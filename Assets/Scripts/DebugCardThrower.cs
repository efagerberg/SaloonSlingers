using UnityEngine;

public enum DebugThrowMode { DownStraight, UpStraight, RightSide, LeftSide}

public class DebugCardThrower : CardThrower
{
    [SerializeField]
    private DebugThrowMode m_mode = DebugThrowMode.DownStraight;
    private Vector3 linearVelocity;

    protected override void Update()
    {
        m_isDrawing = Input.GetKeyDown(KeyCode.Space);
        base.Update();
    }
    
    protected override void DrawCard()
    {
        CalculateVelocityFromThrowMode();
        base.DrawCard();
        m_isThrowing = true;
    }
     

    protected override void ThrowCard()
    {
        if (m_grabbedObj != null)
        {
            m_grabbedObj.GetComponent<CardComponent>().Throw(linearVelocity);
            m_grabbedObj = null;
            m_isThrowing = false;
        }
    }

    private void CalculateVelocityFromThrowMode()
    {
        if (m_mode == DebugThrowMode.DownStraight)
        {
            m_grabbedObjectRotOffset = new Vector3(0, 90, 0);
            linearVelocity = new Vector3(0f, -0.01f, 1f) * m_throwSpeedMultiplier;
        }
        else if (m_mode == DebugThrowMode.UpStraight)
        {
            m_grabbedObjectRotOffset = new Vector3(0, 90, 0);
            linearVelocity = new Vector3(0f, 0.01f, 1f) * m_throwSpeedMultiplier;
        }
        else if (m_mode == DebugThrowMode.RightSide)
        {
            m_grabbedObjectRotOffset = new Vector3(90, 0, 0);
            linearVelocity = new Vector3(0.01f, 0f, 1f) * m_throwSpeedMultiplier;
        }
        else if (m_mode == DebugThrowMode.LeftSide)
        {
            m_grabbedObjectRotOffset = new Vector3(90, 0, 0);
            linearVelocity = new Vector3(-0.01f, 0f, 1f) * m_throwSpeedMultiplier;
        }
        else
        {
            Debug.LogError("No action left for ${m_mode}");
            return;
        }
    }
}
