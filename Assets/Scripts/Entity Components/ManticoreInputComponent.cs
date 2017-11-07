using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManticoreInputComponent : EntityComponent {

    EntityStaminaComponent staminaComponent;

    // This will probably be lifted from a data class managed by the inventory
    // system later on, but we'll use serialized fields for the time being.
    [SerializeField]
    float blinkCost;

    ParryComponent parryComponent;

    void OnEnable()
    {
        staminaComponent = GetComponent<EntityStaminaComponent>();
        parryComponent = GetComponent<ParryComponent>();
    }

    protected override void Subscribe()
    {
        entityData.SetAttribute(EntityAttributes.CurrentDirection, Vector3.zero);
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);

		entityEmitter.SubscribeToEvent(EntityEvents.Stun, Disconnect);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, Disconnect);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, Reconnect);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, Disconnect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, Disconnect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, Reconnect);
    }

    #region Event listeners

    void OnUpdate()
    {
        TransmitPlayerAction();
    }

    void OnFixedUpdate()
    {
        SetDirectionalMovementFromKeys();
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

    #endregion

    #region Action functions

    void TransmitPlayerAction()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            float parryCost = parryComponent.ParryCost;
            if (staminaComponent.TryToExpendStamina(parryCost))
            {
                entityEmitter.EmitEvent(EntityEvents.Parry);
            }
        }
        else if (Input.GetButtonDown("Blink"))
        {
            if (staminaComponent.TryToExpendStamina(blinkCost))
            {
                entityEmitter.EmitEvent(EntityEvents.Blink);
            }
        }
        else if (Input.GetButtonDown("Nullify"))
        {
            entityEmitter.EmitEvent(EntityEvents.Nullify);
        }
    }

    #endregion

    #region Movement functions

    void SetDirectionalMovementFromKeys()
    {
        float horizontalKeyValue = Input.GetAxis("HorizontalKey");
        float verticalKeyValue = Input.GetAxis("VerticalKey");

        if (Mathf.Abs(horizontalKeyValue) < 0.1f && Mathf.Abs(verticalKeyValue) < 0.1f)
        {
            entityData.SetAttribute(EntityAttributes.CurrentDirection, Vector3.zero);
            entityEmitter.EmitEvent(EntityEvents.Stop);
        }
        else
        {
            Vector3 horizontalMovement = new Vector3(1f, 0f, 1f) * horizontalKeyValue;
            Vector3 verticalMovement = new Vector3(-1f, 0f, 1f) * verticalKeyValue;
            Vector3 direction = horizontalMovement + verticalMovement;
            entityData.SetAttribute(EntityAttributes.CurrentDirection, direction);
            entityEmitter.EmitEvent(EntityEvents.DirectionChanged);
        }
    }

    #endregion

}