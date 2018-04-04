using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yank : MonoBehaviour {

    Rigidbody yankRigidbody;
    YankHardware yankHardware;

    public float TravelTime;

    const string TERRAIN_LAYER_ID = "Terrain";
    LayerMask terrainLayer;

    public void PassReferenceToHardware(YankHardware _yankHardware)
    {
        yankHardware = _yankHardware;
    }

    void Awake()
    {
        terrainLayer = LayerMask.NameToLayer(TERRAIN_LAYER_ID);
        yankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 lookDirection = transform.position - GameManager.GetPlayerPosition();
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        BulletController bulletController = other.GetComponent<BulletController>();

        if (bulletController != null)
        {
            bulletController.Attach(transform);
        }
    }
}
