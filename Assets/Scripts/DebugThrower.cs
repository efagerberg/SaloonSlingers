using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugThrower : MonoBehaviour
{
    [SerializeField]
    private DeckComponent m_deck;
    [SerializeField]
    private float m_throwSpeed = 10f;
    [SerializeField]
    private float m_spinSpeed = 10f;
    private GameObject m_grabbedObj;

    private bool isThrowing = false;

    private void Start()
    {
        if (m_deck == null)
        {
            var deck_go = GameObject.Find("Deck");
            m_deck = deck_go.GetComponent<DeckComponent>();
        }
    }

    private void Update()
    {
        isThrowing = Input.GetKeyDown(KeyCode.Space);
        if (isThrowing)
        {
            DrawCard();
            ThrowCard();
            isThrowing = false;
        }
    }

    private void FixedUpdate()
    {
    }

    protected void DrawCard()
    {
        m_grabbedObj = m_deck.SpawnCard();
        m_grabbedObj.transform.SetParent(transform);
        m_grabbedObj.transform.localPosition = new Vector3(0.2f, -0.1f, 0f);
        m_grabbedObj.transform.localRotation = Quaternion.Euler(0f, 90f, 45f);
    }

    protected void ThrowCard()
    {
        if (m_grabbedObj != null)
        {
            Vector3 linearVelocity = transform.forward * m_throwSpeed;
            Vector3 angularVelocity = (transform.right) * m_spinSpeed;
            m_grabbedObj.GetComponent<CardComponent>().Throw(linearVelocity, angularVelocity);
            m_grabbedObj = null;
        }
    }
}
