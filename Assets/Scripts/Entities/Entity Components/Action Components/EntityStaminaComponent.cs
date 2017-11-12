﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a weird one—going to allow for some coupling for the time being.
// As of now, only player entity will actually use this, and it's going to be
// hard-wired into the player's Input component.
// If it's going to be used in any future enemies, it'll be similarly wired into
// their AI's--there'll be no stamina expenditure/contracts via the emission system.
// However, keeping it wired into the emission system normalizes behaviors and allows
// for it to be looped into status effects, which seems worth having it break type
// with the typical component format.
public class EntityStaminaComponent : EntityComponent {

    [SerializeField]
    float maximumStamina = 100;
    [SerializeField]
    StaminaBar attachedStaminaBar;

    [SerializeField]
    float recoveryFreezeDuration = 1f;
    [SerializeField]
    float recoveryTickRate = 0.5f;
    [SerializeField]
    float recoveryTickAmount = 20f;

    float currentStamina;

    protected override void Awake()
    {
        base.Awake();
        currentStamina = maximumStamina;
    }

    protected override void Subscribe()
    {
    }

    protected override void Unsubscribe()
    {
    }

    void OnEnable()
    {
        if (attachedStaminaBar != false) 
        {
            attachedStaminaBar.UpdateTotalStamina(currentStamina);
        }
    }

    public bool TryToExpendStamina(float amountToUse)
    {
        if (currentStamina < amountToUse)
        {
            return false;
        }

        currentStamina -= amountToUse;

        if (attachedStaminaBar != false)
        {
            attachedStaminaBar.UpdateCurrentStamina(currentStamina);
        }

        CancelInvoke();
        InvokeRepeating("RecoverStamina", recoveryFreezeDuration, recoveryTickRate);

        return true;
    }

    public void ChangeStamina(float changeAmount)
    {
        currentStamina += changeAmount;

        if (currentStamina >= maximumStamina)
        {
            currentStamina = maximumStamina;
        }

        if (attachedStaminaBar != false)
        {
            attachedStaminaBar.UpdateCurrentStamina(currentStamina);
        }
    }

    #region callbacks

    void RecoverStamina()
    {
        currentStamina += recoveryTickAmount;

        if (currentStamina >= maximumStamina)
        {
            currentStamina = maximumStamina;
            CancelInvoke();
        }

        if (attachedStaminaBar != false)
        {
            attachedStaminaBar.UpdateCurrentStamina(currentStamina);
        }
    }

    #endregion
}