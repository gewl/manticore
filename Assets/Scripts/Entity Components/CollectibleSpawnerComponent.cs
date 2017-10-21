using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawnerComponent : EntityComponent {

    [SerializeField]
    GameObject collectible;
    [SerializeField]
    float launchSpeed;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
    }

    void OnDead()
    {
        Vector3 playerDirection = (GameManager.GetPlayerPosition() - transform.position).normalized / 2f;
        playerDirection.y = 1f;

        GameObject collectibleInstance = GameObject.Instantiate(collectible, transform.position, Quaternion.identity);
        collectibleInstance.GetComponent<Rigidbody>().AddForce(playerDirection * launchSpeed, ForceMode.Impulse);
    }
}
