using System;
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

    public delegate void StaminaUpdated(float newStamina);
    [HideInInspector]
    public StaminaUpdated TotalStaminaUpdated;
    [HideInInspector]
    public StaminaUpdated CurrentStaminaUpdated;

    [SerializeField]
    float baseMaximumStamina = 200;
    [SerializeField]
    float equipCost_activeHardware = 40;
    [SerializeField]
    float equipCost_passiveHardware = 10;

    [SerializeField]
    float recoveryFreezeDuration = 1f;
    [SerializeField]
    float recoveryTickRate = 0.5f;
    [SerializeField]
    float recoveryTickAmount = 20f;

    float adjustedMaximumStamina;
    public float AdjustedMaximumStamina { get { return adjustedMaximumStamina; } }
    float currentStamina;

    protected override void Awake()
    {
        base.Awake();
        currentStamina = baseMaximumStamina;
    }

    private void Start()
    {
        CalculateMaximumStamina(InventoryController.Inventory);
        if (TotalStaminaUpdated != null)
        {
            TotalStaminaUpdated(adjustedMaximumStamina);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InventoryController.OnInventoryUpdated += CalculateMaximumStamina;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InventoryController.OnInventoryUpdated -= CalculateMaximumStamina;
    }

    #region Inventory change listeners

    void CalculateMaximumStamina(InventoryData inventory)
    {
        adjustedMaximumStamina = baseMaximumStamina;

        for (int i = 2; i < inventory.EquippedActiveHardware.Length; i++)
        {
            HardwareType hardwareType = inventory.EquippedActiveHardware[i];
            if (hardwareType != HardwareType.None)
            {
                adjustedMaximumStamina -= equipCost_activeHardware;
            }
        }

        for (int i = 0; i < inventory.EquippedPassiveHardware.Length; i++)
        {
            HardwareType hardwareType = inventory.EquippedPassiveHardware[i];
            if (hardwareType != HardwareType.None)
            {
                adjustedMaximumStamina -= equipCost_passiveHardware;
            }
        }

        if (TotalStaminaUpdated != null)
        {
            TotalStaminaUpdated(adjustedMaximumStamina);
        }

        currentStamina = adjustedMaximumStamina;

        if (CurrentStaminaUpdated != null)
        {
            CurrentStaminaUpdated(currentStamina);
        }
    }

    #endregion

    protected override void Subscribe()
    {
    }

    protected override void Unsubscribe()
    {
    }

    public bool TryToExpendStamina(float amountToUse)
    {
        if (currentStamina < amountToUse)
        {
            return false;
        }

        currentStamina -= amountToUse;

        if (CurrentStaminaUpdated != null)
        {
            CurrentStaminaUpdated(currentStamina);
        }

        CancelInvoke();
        InvokeRepeating("RecoverStamina", recoveryFreezeDuration, recoveryTickRate);

        return true;
    }

    public void ChangeStamina(float changeAmount)
    {
        currentStamina += changeAmount;

        if (currentStamina >= adjustedMaximumStamina)
        {
            currentStamina = adjustedMaximumStamina;
        }

        if (CurrentStaminaUpdated != null)
        {
            CurrentStaminaUpdated(currentStamina);
        }
    }

    #region callbacks

    void RecoverStamina()
    {
        currentStamina += recoveryTickAmount;

        if (currentStamina >= adjustedMaximumStamina)
        {
            currentStamina = adjustedMaximumStamina;
            CancelInvoke();
        }

        if (CurrentStaminaUpdated != null)
        {
            CurrentStaminaUpdated(currentStamina);
        }
    }

    #endregion
}
