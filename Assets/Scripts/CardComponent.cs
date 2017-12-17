using UnityEngine;
using UnityEngine.UI;

public class CardComponent : MonoBehaviour
{
    [SerializeField]
    private Card card;
    [SerializeField]
    private Renderer m_renderer;

    private Rigidbody m_rb;

    private void Update()
    {
        name = card.ToString();
        UpdateGraphic();
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
        m_rb.isKinematic = false;
        m_rb.velocity = linearVelocity;
        m_rb.angularVelocity = angularVelocity;

        Destroy(gameObject, 2f);
        transform.parent = null;
    }
}
