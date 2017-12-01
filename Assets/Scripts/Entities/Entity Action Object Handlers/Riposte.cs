using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riposte : MonoBehaviour {

    Material blinkSkin;

    RiposteHardware riposteHardware;

    private void OnEnable()
    {
        blinkSkin = GetComponent<Renderer>().material;
        riposteHardware = GetComponentInParent<RiposteHardware>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Collider>().enabled = false;
        other.GetComponent<BasicBullet>().enabled = false;

        riposteHardware.StartAbsorbingBullet(other.gameObject);
    }

}
