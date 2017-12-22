using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFireComponent : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        var cardComp = col.gameObject.GetComponent<CardComponent>();
        if (cardComp == null) return;
        cardComp.Burn();
        Destroy(col.gameObject);
    }

}
