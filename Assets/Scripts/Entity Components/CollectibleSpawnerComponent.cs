using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawnerComponent : EntityComponent {

    [SerializeField]
    GlobalConstants.Collectibles collectible;
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
        if (GameManager.TryToRegisterCollectible(collectible))
        {
            Vector3 playerDirection = (GameManager.GetPlayerPosition() - transform.position).normalized / 2f;
            playerDirection.y = 1f;

            GameObject collectiblePrefab = GameManager.RetrieveCollectiblePrefab(collectible);
            GameObject collectibleInstance = GameObject.Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
            Rigidbody collectibleRigidbody = collectibleInstance.GetComponent<Rigidbody>();

            collectibleRigidbody.AddForce(playerDirection * launchSpeed, ForceMode.Impulse);
            collectibleRigidbody.AddTorque(Vector3.forward, ForceMode.Impulse);
        }
    }
}
