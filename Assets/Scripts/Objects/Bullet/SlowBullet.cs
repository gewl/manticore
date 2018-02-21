using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBullet : BulletController
{
    int playerLayer;
    int entityLayer;

    const string PLAYER_LAYER = "Player";
    const string ENTITY_LAYER = "Entity";

    public ScriptableObject slowModifier;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer(PLAYER_LAYER);
        entityLayer = LayerMask.NameToLayer(ENTITY_LAYER);
    }

    protected override void Impact(Vector3 point, Vector3 normal, GameObject collisionObject, int collisionObjectLayer)
    {
        if (collisionObjectLayer == playerLayer || collisionObjectLayer == entityLayer)
        {

            EntityModifierHandler modifierHandler = collisionObject.GetComponent<EntityModifierHandler>();

            if (modifierHandler != null)
            {
                Modifier slowModifierInstance = UnityEngine.Object.Instantiate(slowModifier) as Modifier;
                modifierHandler.RegisterModifier(slowModifierInstance);
            }
        }

        base.Impact(point, normal, collisionObject, collisionObjectLayer);
    }
}

