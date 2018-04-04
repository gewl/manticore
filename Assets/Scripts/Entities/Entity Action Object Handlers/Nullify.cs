using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nullify : MonoBehaviour {

    public bool IsFracturing = false;
    float handicap = 0.5f;

    LayerMask terrainLayer;

    const string TERRAIN_LAYER_ID = "Terrain";

    private void Awake()
    {
        terrainLayer = LayerMask.NameToLayer(TERRAIN_LAYER_ID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == terrainLayer)
        {
            return;
        }

        BulletController bullet = other.GetComponent<BulletController>();
        if (bullet == null)
        {
            return;
        }

        if (IsFracturing)
        {
            if (bullet.CompareTag(BulletController.ENEMY_BULLET))
            {
                bullet.Parry(transform.parent, bullet.strength * handicap, handicap);
            }
        }
        else 
        {
            bullet.Dissolve();
        }
        
    }

}
