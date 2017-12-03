﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    ParryHardware parryHardware;
    EntityGearManagement gear;

    ManticoreAudioComponent audioComponent;

    void Awake()
    {
        parryHardware = GetComponentInParent<ParryHardware>();
        gear = GetComponentInParent<EntityGearManagement>();

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
        gear.ApplyParryPassiveHardwareToBullet(bulletObject);

        if (audioComponent != null)
        {
            audioComponent.PlayGearSound(HardwareTypes.Parry);
        }
    }
}
