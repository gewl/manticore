using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    ParryComponent parryComponent;

    void Awake()
    {
        parryComponent = GetComponentInParent<ParryComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BasicBullet bullet = other.GetComponent<BasicBullet>();
        if (bullet == null)
        {
            return;
        }
        float parryDamage = parryComponent.ParryDamage;
        bullet.Parry(transform, GameManager.GetMousePositionInWorldSpace(), parryDamage);

        parryComponent.SuccessfulParryHandler();
    }
}
