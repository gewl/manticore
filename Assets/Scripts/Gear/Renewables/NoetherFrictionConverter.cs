using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoetherFrictionConverter : EntityComponent, IRenewable {

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry);
    }

    public void UseRenewable()
    {
        Debug.Log("use!");
    }

    void OnParry()
    {

    }
}
