using System.Collections;
using UnityEngine;

public class ManticoreInputComponent : EntityComponent {

    EntityStaminaComponent staminaComponent;
    EntityGearManagement gear;

    private bool actionsLocked = false;
    private bool movementLocked = false;

    float staminaTickRate = 0.5f;

    public void LockActions(bool areLocked)
    {
        actionsLocked = areLocked;
    }

    public void LockMovement(bool isLocked)
    {
        actionsLocked = isLocked;
        if (isLocked)
        {
            entityEmitter.EmitEvent(EntityEvents.FreezeRotation);
        }
        else
        {
            entityEmitter.EmitEvent(EntityEvents.ResumeRotation);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        staminaComponent = GetComponent<EntityStaminaComponent>();
        gear = GetComponent<EntityGearManagement>();
    }

    protected override void Subscribe()
    {
        entityInformation.SetAttribute(EntityAttributes.CurrentDirection, Vector3.zero);
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
        if (actionsLocked)
        {
            return;
        }

        IHardware gear_slot2 = gear.EquippedGear_Slot2;
        IHardware gear_slot3 = gear.EquippedGear_Slot3;

        if (gear.EquippedGear_Slot2 != null && gear.EquippedGear_Slot2.IsInUse && Input.GetButtonUp("UseGear_Slot2"))
        {
            StopGearUse(gear_slot2);
        }
        else if (gear.EquippedGear_Slot3 != null && gear.EquippedGear_Slot3.IsInUse && Input.GetButtonUp("UseGear_Slot3"))
        {
            StopGearUse(gear_slot3);
        }

        // Check if using (for Instant) or beginning (for Charged/Channeled) gear use
        if (Input.GetButtonDown("Parry"))
        {
            IHardware parryGear = gear.ParryGear;
            UseGear(parryGear);
        }
        else if (Input.GetButtonDown("Blink"))
        {
            IHardware blinkGear = gear.BlinkGear;
            UseGear(blinkGear);
        }
        else if (Input.GetButtonDown("UseGear_Slot2") && !IsGearInUse())
        {
            if (gear_slot2 == null)
            {
                return;
            }
            UseGear(gear_slot2);
            if (gear_slot2.HardwareUseType != HardwareUseTypes.Instant)
            {
                StartCoroutine("PeriodicalStaminaTick_Slot2");
            }
        }
        else if (Input.GetButtonDown("UseGear_Slot3") && !IsGearInUse())
        {
            if (gear_slot3 == null)
            {
                return;
            }
            UseGear(gear_slot3);
            if (gear_slot3.HardwareUseType != HardwareUseTypes.Instant)
            {
                StartCoroutine("PeriodicalStaminaTick_Slot3");
            }
        }
        else if (Input.GetButtonDown("UseRenewable"))
        {
            if (gear.EquippedRenewable == null)
            {
                return;
            }
            gear.EquippedRenewable.UseRenewable();
        }
    }

    void UseGear(IHardware gear)
    {
        int staminaCost = gear.StaminaCost;
        if (!gear.IsOnCooldown && staminaComponent.TryToExpendStamina(staminaCost))
        {
            gear.UseActiveHardware();
        }
    }

    void StopGearUse(IHardware gear)
    {
        gear.UseActiveHardware();
    }

    #endregion

    #region Channeling gear functions

    bool IsGearInUse()
    {
        return gear.EquippedGear_Slot2 != null && gear.EquippedGear_Slot2.IsInUse || gear.EquippedGear_Slot3 != null && gear.EquippedGear_Slot3.IsInUse;
    }

    IEnumerator PeriodicalStaminaTick_Slot2()
    {
        int staminaCost = gear.EquippedGear_Slot2.StaminaCost;
        IHardware gear_slot2 = gear.EquippedGear_Slot2;

        while (gear_slot2.IsInUse)
        {
            if (!staminaComponent.TryToExpendStamina(staminaCost))
            {
                StopGearUse(gear_slot2);
            }

            yield return new WaitForSeconds(staminaTickRate);
        }
    }

    IEnumerator PeriodicalStaminaTick_Slot3()
    {
        int staminaCost = gear.EquippedGear_Slot3.StaminaCost;
        IHardware gear_slot3 = gear.EquippedGear_Slot3;

        while (gear_slot3.IsInUse)
        {
            if (!staminaComponent.TryToExpendStamina(staminaCost))
            {
                StopGearUse(gear_slot3);
            }

            yield return new WaitForSeconds(staminaTickRate);
        }
    }

    #endregion

    #region Movement functions

    void SetDirectionalMovementFromKeys()
    {
        if (movementLocked)
        {
            return;
        }
        float horizontalKeyValue = Input.GetAxis("HorizontalKey");
        float verticalKeyValue = Input.GetAxis("VerticalKey");

        if (Mathf.Abs(horizontalKeyValue) < 0.1f && Mathf.Abs(verticalKeyValue) < 0.1f)
        {
            entityInformation.SetAttribute(EntityAttributes.CurrentDirection, Vector3.zero);
            entityEmitter.EmitEvent(EntityEvents.Stop);
        }
        else
        {
            Vector3 horizontalMovement = new Vector3(1f, 0f, 1f) * horizontalKeyValue;
            Vector3 verticalMovement = new Vector3(-1f, 0f, 1f) * verticalKeyValue;
            Vector3 direction = horizontalMovement + verticalMovement;
            entityInformation.SetAttribute(EntityAttributes.CurrentDirection, direction);
            entityEmitter.EmitEvent(EntityEvents.DirectionChanged);
            entityEmitter.EmitEvent(EntityEvents.Move);
        }
    }

    #endregion

}