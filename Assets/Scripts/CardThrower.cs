
using UnityEngine;

/// <summary>
/// Throw cards
/// </summary>
public abstract class CardThrower : MonoBehaviour
{
    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    protected DeckComponent m_deck;

    [SerializeField]
    protected float m_throwSpeedMultiplier = 10f;

    [SerializeField]
    protected Vector3 m_grabbedObjectPosOffset;
    [SerializeField]
    protected Vector3 m_grabbedObjectRotOffset;

    // Used to check input on the update frame but apply physics on the fixed update frame
    protected bool m_isThrowing = false;
    protected bool m_isDrawing = false;

    protected GameObject m_grabbedObj;

    protected abstract void ThrowCard();

    protected virtual void DrawCard()
    {
        if (m_grabbedObj == null)
        {
            m_grabbedObj = m_deck.SpawnCard();
            m_grabbedObj.transform.SetParent(m_parentTransform);
            m_grabbedObj.transform.localPosition = m_grabbedObjectPosOffset;
            m_grabbedObj.transform.localRotation = Quaternion.Euler(m_grabbedObjectRotOffset);

            m_isDrawing = false;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (m_isThrowing) ThrowCard();
    }

    protected virtual void Update()
    {
        if (m_isDrawing) DrawCard();
    }

    protected virtual void Start()
    {
        if (m_deck == null)
        {
            var deck_go = GameObject.Find("Deck");
            m_deck = deck_go.GetComponent<DeckComponent>();
        }

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
    }
}

