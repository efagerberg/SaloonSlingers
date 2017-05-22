using UnityEngine;
using UnityEngine.UI;

public class CardComponent : MonoBehaviour
{
    [SerializeField]
    private Card card;
    [SerializeField]
    private Renderer m_renderer;

    private void Update()
    {
        name = card.ToString();
        UpdateGraphic();
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
}
