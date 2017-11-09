using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithVelocity : EntityComponent {

    Rigidbody entityRigidbody;
    [SerializeField]
    float damping = 1f;

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
        if (entityRigidbody.velocity.sqrMagnitude >= 0.1f)
        {
            Quaternion nextRotation = Quaternion.LookRotation(entityRigidbody.velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, nextRotation, Time.deltaTime * damping);
        }
    }
}
