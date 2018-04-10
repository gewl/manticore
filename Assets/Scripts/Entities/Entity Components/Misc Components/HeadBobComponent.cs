using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobComponent : EntityComponent {

    [SerializeField]
    Transform head;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    private void OnUpdate()
    {
        float headAdjustment = Mathf.PingPong(Time.time, 1f);

        head.transform.localPosition = new Vector3(0f, 0f, headAdjustment - 0.5f);
    }
}
