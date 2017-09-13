using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : EntityComponent {

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);

        entityEmitter.SubscribeToEvent(EntityEvents.Hurt, Disconnect);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, Disconnect);
        entityEmitter.SubscribeToEvent(EntityEvents.Recovered, Reconnect);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, Disconnect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, Disconnect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Recovered, Reconnect);
    }

    void OnUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            Vector3 characterToHitpoint = (hitPoint - transform.position).normalized;

            entityData.SetSoftAttribute(SoftEntityAttributes.CurrentTargetPosition, characterToHitpoint);
            entityEmitter.EmitEvent(EntityEvents.TargetPositionUpdated);
		}
    }

    void OnFixedUpdate()
    {

    }

    void Disconnect()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }
    
    void Reconnect()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }
}
