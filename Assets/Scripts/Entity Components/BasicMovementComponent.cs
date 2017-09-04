using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BasicMovementComponent : EntityComponent {

    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);

        base.entityData.Expect(SoftEntityAttributes.CurrentMoveSpeed);
    }

    void OnUpdate()
    {
        base.entityData.EntityRigidbody.velocity = new Vector3(0f, 0f, 1f) * (float)base.entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
    }
}
