using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riposte : MonoBehaviour {

    RiposteHardware riposteHardware;
    int enemyBulletLayer, entityLayer;

    private void OnEnable()
    {
        riposteHardware = GetComponentInParent<RiposteHardware>();

        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
        entityLayer = LayerMask.NameToLayer("Entity");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyBulletLayer)
        {
            other.GetComponent<Collider>().enabled = false;
            other.GetComponent<BulletController>().enabled = false;

            riposteHardware.StartAbsorbingBullet(other.gameObject);
        }
        else if (other.gameObject.layer == entityLayer)
        {
            if (riposteHardware.IsDashing)
            {
                riposteHardware.BeginRiposte(other.transform);
            }
        }
    }

}
