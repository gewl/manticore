using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggroComponent : EntityComponent {

    List<Transform> entitiesToAggro;

    [SerializeField]
    float minAggroDelay = 0.2f;
    [SerializeField]
    float maxAggroDelay = 0.4f;

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

            float aggroDelay = Random.Range(minAggroDelay, maxAggroDelay);
            yield return new WaitForSeconds(maxAggroDelay);
        }
        // If one dies before all are aggroed, it wigs. Just leaving them immune until all notified.
        foreach (Transform entity in entitiesToAggro)
        {
            entity.GetComponent<StationaryEntityHealthComponent>().IsInvulnerable = false;
        }
    }
}

