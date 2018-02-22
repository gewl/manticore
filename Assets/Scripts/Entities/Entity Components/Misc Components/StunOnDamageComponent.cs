using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunOnDamageComponent : EntityComponent {

    [SerializeField]
    Modifier stunModifier;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Hurt, OnHurt);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, OnHurt);
    }

    void OnHurt()
    {
        Modifier stunModifierInstance = Object.Instantiate(stunModifier) as Modifier;
        entityModifierHandler.RegisterModifier(stunModifierInstance);
    }
}
