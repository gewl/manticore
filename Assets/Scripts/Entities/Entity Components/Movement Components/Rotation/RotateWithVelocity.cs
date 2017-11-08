using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithVelocity : EntityComponent {

    Rigidbody entityRigidbody;

    protected override void Awake()
    {
        base.Awake();
        entityRigidbody = GetComponent<Rigidbody>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnFixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(entityRigidbody.velocity, Vector3.up);
    }
}
