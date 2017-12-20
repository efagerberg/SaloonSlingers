using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [SerializeField]
    private Card card;
    [SerializeField]
    private Renderer m_renderer;
    [SerializeField]
    private float m_maxSpinSpeed = 50;
    [SerializeField]
    private bool tracePath = true;

    private Rigidbody m_rb;

    private void Update()
    {
        name = card.ToString();
        UpdateGraphic();
    }

    private void FixedUpdate()
    {
        var decayVector = new Vector3(-Mathf.Round(m_rb.angularVelocity.normalized.y) * 0.05f,
                                      Mathf.Round(m_rb.angularVelocity.normalized.x) * 0.01f,
                                      0);
        m_rb.velocity -= decayVector;
    }

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public Card GetCard()
    {
        return card;
    }

    public void SetCard(Card inCard)
    {
        card = inCard;
    }

    public void UpdateGraphic()
    {
        if (card.Equals(null))
        {
            return;
        }
        m_renderer.material.mainTexture = Resources.Load<Texture>(card.GetTexturePath());
    }

    public void Throw(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.maxAngularVelocity = m_maxSpinSpeed;
        m_rb.isKinematic = false;
        m_rb.velocity = linearVelocity;
        m_rb.angularVelocity = angularVelocity;

        if (tracePath)
        {
            // Add tracer
            TrailRenderer tr = gameObject.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.widthMultiplier = 0.01f;
            tr.material = Resources.Load<Material>("Materials/TracerMaterial");
        }

        Destroy(gameObject, 2f);
        transform.parent = null;
    }
}
