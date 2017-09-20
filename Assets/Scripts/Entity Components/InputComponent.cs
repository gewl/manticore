﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : EntityComponent {

    protected override void Subscribe()
    {
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentDirection, Vector3.zero);
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
            entityEmitter.EmitEvent(EntityEvents.Parry);
        }
        else if (Input.GetButtonDown("Blink"))
        {
            entityEmitter.EmitEvent(EntityEvents.Blink);
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
            entityData.SetSoftAttribute(SoftEntityAttributes.CurrentDirection, Vector3.zero);
            entityEmitter.EmitEvent(EntityEvents.Stop);
        }
        else
        {
            Vector3 direction = new Vector3(horizontalKeyValue, 0, verticalKeyValue);
            entityData.SetSoftAttribute(SoftEntityAttributes.CurrentDirection, direction);
            entityEmitter.EmitEvent(EntityEvents.DirectionChanged);
        }
    }

    #endregion
}