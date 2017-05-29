using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(OVRGearVrController))]
public class TestGrabber : MonoBehaviour
{
    private Transform grabbedTransform;
    private OVRInput.Controller m_controller;

    private void Start()
    {
        m_controller = GetComponent<OVRGearVrController>().m_controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) && grabbedTransform == null)
        {
            Grab(other.transform);
        }
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) && grabbedTransform != null)
            Release(grabbedTransform);
    }

    public void Grab(Transform t)
    {
        Debug.Log("Grabbing");

        t.SetParent(transform, false);

        // position it in the view
        t.localPosition = new Vector3(0, 0, 0);

        // disable physics
        t.GetComponent<Rigidbody>().isKinematic = true;
        grabbedTransform = t;
    }

    public void Release(Transform t)
    {
        Debug.Log("Releasing");

        // set the parent to the world
        t.transform.SetParent(null, true);

        // get the rigidbody physics component
        Rigidbody rigidbody = t.GetComponent<Rigidbody>();

        // reset velocity
        rigidbody.velocity = Vector3.zero;

        // enable physics
        rigidbody.isKinematic = false;

        grabbedTransform = null;
    }
}
