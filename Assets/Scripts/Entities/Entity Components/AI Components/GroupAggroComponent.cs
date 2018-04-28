using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggroComponent : EntityComponent {

    List<Transform> entitiesToAggro;

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
        entitiesToAggro = GetComponent<GroupReferenceComponent>().GetGroup();
        StartCoroutine(AlertGroupMembers());
    }

    IEnumerator AlertGroupMembers()
    {
        foreach (Transform entity in entitiesToAggro)
        {
            Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
            entity.GetComponent<EntityManagement>().SetAttribute(EntityAttributes.CurrentTarget, currentTarget);
            EntityEmitter emitter = entity.GetComponent<EntityEmitter>();
            emitter.EmitEvent(EntityEvents.Aggro);
            emitter.EmitEvent(EntityEvents.TargetUpdated);

            yield return new WaitForSeconds(aggroDelay);
        }
    }
}

