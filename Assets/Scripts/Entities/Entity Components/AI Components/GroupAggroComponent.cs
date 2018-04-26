using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggroComponent : EntityComponent {

    [SerializeField]
    List<EntityEmitter> entitiesToAggro;

    [SerializeField]
    float aggroDelay = 0.1f;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
    }

    void OnAggro()
    {
        StartCoroutine(AlertGroupMembers());
    }

    IEnumerator AlertGroupMembers()
    {
        foreach (EntityEmitter emitter in entitiesToAggro)
        {
            Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
            emitter.GetComponent<EntityManagement>().SetAttribute(EntityAttributes.CurrentTarget, currentTarget);
            emitter.EmitEvent(EntityEvents.Aggro);
            yield return new WaitForSeconds(aggroDelay);
        }
    }
}

