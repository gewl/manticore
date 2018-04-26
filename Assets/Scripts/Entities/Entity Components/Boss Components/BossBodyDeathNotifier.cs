using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBodyDeathNotifier : EntityComponent {

    [SerializeField]
    Transform bossHead;

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
        bossHead.GetComponent<BossHeadSegmentSorter>().OnSegmentDeath();
    }
}
