using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugThrower : MonoBehaviour
{
    [SerializeField]
    private DeckComponent m_deck;
    [SerializeField]
    private float m_throw_speed = 2f;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isThrowing = true;
        }
    }

    private void FixedUpdate()
    {
        if (isThrowing)
        {
            DrawCard();
            ThrowCard();
            isThrowing = false;
        }
    }

    protected void DrawCard()
    {
        m_grabbedObj = m_deck.SpawnCard();
        m_grabbedObj.transform.SetParent(transform);
        Rigidbody rb = m_grabbedObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        m_grabbedObj.transform.localPosition = Vector3.zero;
        m_grabbedObj.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
    }

    protected void ThrowCard()
    {
        if (m_grabbedObj != null)
        {
            Vector3 linearVelocity = transform.forward * m_throw_speed;
            Vector3 angularVelocity = (transform.right + transform.up) * m_throw_speed * 50f;

            Rigidbody rb = m_grabbedObj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(linearVelocity, ForceMode.Impulse);
            rb.AddTorque(angularVelocity);

            Destroy(m_grabbedObj, 2f);
            m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;
        }
    }
}
