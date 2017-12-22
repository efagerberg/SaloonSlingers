using UnityEngine;

public class CardComponent : MonoBehaviour
{
    [SerializeField]
    private Card m_card;
    [SerializeField]
    private Renderer m_renderer;
    [SerializeField]
    private float m_maxSpinSpeed = 50;
    [SerializeField]
    private bool m_shouldTracePath = true;
    [SerializeField]
    private GameObject m_destroyEffectPrefab;
    [SerializeField]
    private float m_effectLifetime = 2f;
    [SerializeField]
    private float m_curveFactor = 0.01f;

    private Rigidbody m_rb;


    private void Update()
    {
        name = m_card.ToString();
        UpdateGraphic();
    }

    private void FixedUpdate()
    {
        AddCurveForce();
    }

    private void AddCurveForce()
    {
        Debug.Log(m_rb.angularVelocity.normalized);
        var curveForce = new Vector3(m_rb.angularVelocity.normalized.y,
                                     m_rb.angularVelocity.normalized.x,
                                     0) * m_curveFactor;

        m_rb.AddForce(curveForce);
    }

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public Card GetCard()
    {
        return m_card;
    }

    public void SetCard(Card inCard)
    {
        m_card = inCard;
    }

    public void UpdateGraphic()
    {
        if (m_card.Equals(null)) return;
        m_renderer.material.mainTexture = Resources.Load<Texture>(m_card.GetTexturePath());
    }

    public void Throw(Vector3 linearVelocity)
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.maxAngularVelocity = m_maxSpinSpeed;
        m_rb.isKinematic = false;
        m_rb.AddForce(linearVelocity);

        // Spin card in direction of velocity
        m_rb.AddRelativeTorque(linearVelocity);

        Destroy(gameObject, 2f);
        transform.parent = null;

        if (m_shouldTracePath)
            AddTracer();
    }

    private void AddTracer()
    {
        TrailRenderer tr = gameObject.AddComponent<TrailRenderer>();
        tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tr.time = 0.2f;
        var keyframes = new Keyframe[] {
            new Keyframe(0f, 0.1f),
            new Keyframe(0.2f, 0.05f),
            new Keyframe(0.8f, 0.003f),
            new Keyframe(1f, 0f)
        };
        tr.widthCurve = new AnimationCurve(keyframes);
        tr.material = Resources.Load<Material>("Materials/TracerMaterial");
    }

    public void Burn()
    {
        if (m_destroyEffectPrefab)
        {
            var _go = Instantiate(m_destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(_go, m_effectLifetime);
        }
    }
}
