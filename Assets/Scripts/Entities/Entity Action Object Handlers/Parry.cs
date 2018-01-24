﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    ParryHardware parryHardware;
    EntityGearManagement gear;
    EntityEmitter entityEmitter;

    ManticoreAudioComponent audioComponent;

    void Awake()
    {
        parryHardware = GetComponentInParent<ParryHardware>();
        gear = GetComponentInParent<EntityGearManagement>();
        entityEmitter = GetComponentInParent<EntityEmitter>();

        audioComponent = GetComponentInParent<ManticoreAudioComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject bulletObject = other.gameObject;
        BasicBullet bullet = other.GetComponent<BasicBullet>();
        if (bullet == null)
        {
            return;
        }
        float parryDamage = parryHardware.ParryDamage;

        Vector3 aimPosition = GameManager.GetMousePositionOnPlayerPlane();
        bullet.Parry(transform, aimPosition, parryDamage);
        gear.ApplyPassiveHardware(typeof(ParryHardware), bullet.gameObject);

        entityEmitter.EmitEvent(EntityEvents.Parry);
    }
}
