using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHardware : EntityComponent, IHardware {

    HardwareTypes type = HardwareTypes.Parry;
    public HardwareTypes Type { get { return type; } }

    public bool IsInUse { get { return false; } }

    HardwareUseTypes hardwareUseType = HardwareUseTypes.Instant;
    public HardwareUseTypes HardwareUseType { get { return hardwareUseType; } }

    ParryHardwareData subtypeData;

    //[SerializeField]
    //float parryDamage = 50f;
    // TODO: Add momentum
    public float ParryDamage { get { return subtypeData.GetDamageDealt(0); } }
    // Speed of swing.
    //[SerializeField]
    //float timeToCompleteParry;
    //[SerializeField]
    //int parryStaminaCost = 30;
    //public int BaseStaminaCost { get { return parryStaminaCost; } }
    // TODO: Add momentum
    public int StaminaCost
    {
        get
        {
            if (isInParry)
            {
                return subtypeData.GetStaminaCost(0) / 2;
            }
            else
            {
                return subtypeData.GetStaminaCost(0);
            }
        }
    }

    [SerializeField]
    GameObject parryBox;
    public AnimationCurve swingCurve;

    bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float percentOfCooldownRemaining = 1.0f;
    public CooldownDelegate CooldownUpdater { get; set; }

    bool inComboWindow = false;
    bool isInParry = false;

    // Current expiration timer on combo chain.
    float currentComboTimer;
    bool parryQueued = false;
    BoxCollider parryCollider;

	Vector3 parryReadyPosition;
    Quaternion parryReadyRotation;

    override protected void Awake()
    {
        base.Awake();
		parryReadyPosition = parryBox.transform.localPosition;
		parryReadyRotation = parryBox.transform.localRotation;

        parryCollider = parryBox.GetComponent<BoxCollider>();
	}

    // Generally use to prime parry box (and controller) for "Ready" state.
    override protected void Subscribe()
    {
		parryBox.transform.localPosition = parryReadyPosition;
		parryBox.transform.localRotation = parryReadyRotation;
		parryBox.SetActive(false);
	}

    protected override void Unsubscribe()
    {
	}

    #region IHardware methods

    public void UseActiveHardware()
    {
        if (inComboWindow)
        {
            inComboWindow = false;
            if (isInParry)
            {
                parryQueued = true;
            }
            else
            {
                StartCoroutine("BackwardParry");
            }
            isOnCooldown = true;
        }
        else
        {
            StartCoroutine("ForwardParry");
        }
    }

    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        Debug.LogError("Trying to apply Parry passively.");
    }

    // Listener for Parry event received while in Ready state (has not yet parried).
	void OnUpdate_AfterFirstParry()
    {
        if (currentComboTimer > 0f)
        {
            currentComboTimer -= Time.deltaTime;
        }
        else
        {
            inComboWindow = false;
            isOnCooldown = false;
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
            UnlimitEntityAfterParry();
			Subscribe();
		}
    }

    #endregion

    IEnumerator ForwardParry()
    {
        isInParry = true;
        isOnCooldown = true;
        LimitEntityInParry();
		parryBox.SetActive(true);
        parryCollider.enabled = true;
		yield return new WaitForSeconds(0.05f);

		float step = 0f;
        float lastStep = 0f;
        //TODO: Add momentum
		float rate = 1 / subtypeData.GetTimeToCompleteParry(0);

        while (step < 1f)
        {
            step += Time.deltaTime * rate;

            float curvedStep = swingCurve.Evaluate(step);
            parryBox.transform.RotateAround(transform.position, Vector3.up * -1f, 150f * (curvedStep - lastStep));
            lastStep = curvedStep;

            if (step >= 0.8f && !inComboWindow)
            {
                isOnCooldown = false;
                parryCollider.enabled = false;
                inComboWindow = true;
			}

            yield return null;
        }

		if (parryQueued)
        {
			parryQueued = false;
			StartCoroutine("BackwardParry");
            yield break;
        }

        // TODO: Momentum
        currentComboTimer = subtypeData.GetTimeToCombo(0);

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
        isOnCooldown = false;
        isInParry = false;
		yield break;
	}

	IEnumerator BackwardParry()
	{
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
        isInParry = true;
        parryCollider.enabled = true;

		float step = 0f;
		float smoothStep;
		float lastStep = 0f;

		yield return new WaitForSeconds(0.1f);

        // TODO: Add momentum
        float rate = 1 / (subtypeData.GetTimeToCompleteParry(0) * 2/3);

		while (step < 1f)
		{
			step += Time.deltaTime * rate;

			smoothStep = swingCurve.Evaluate(step);
			parryBox.transform.RotateAround(transform.position, Vector3.up, 150f * (smoothStep - lastStep));
			lastStep = smoothStep;
			yield return null;

            if (step >= 0.8f)
            {
                parryCollider.enabled = false;
            }
        }
		entityEmitter.EmitEvent(EntityEvents.Available);

        parryBox.transform.localPosition = parryReadyPosition;
        parryBox.transform.localRotation = parryReadyRotation;
        parryBox.SetActive(false);

        UnlimitEntityAfterParry();

        isOnCooldown = false;
		yield break;
	}

    void LimitEntityInParry()
    {
        isInParry = true;
        float currentMovementSpeed = (float)entityData.GetAttribute(EntityAttributes.CurrentMoveSpeed);
        // TODO: Momentum
        float adjustedMovementSpeed = currentMovementSpeed * subtypeData.GetMovementModifier(0);
        entityData.SetAttribute(EntityAttributes.CurrentMoveSpeed, adjustedMovementSpeed);
		entityEmitter.EmitEvent(EntityEvents.Busy);
		entityEmitter.EmitEvent(EntityEvents.FreezeRotation);
	}

	void UnlimitEntityAfterParry()
	{
        isInParry = false;
		float currentMovementSpeed = (float)entityData.GetAttribute(EntityAttributes.CurrentMoveSpeed);
        // TODO: momentum
		float restoredMovementSpeed = currentMovementSpeed / subtypeData.GetMovementModifier(0);
		entityData.SetAttribute(EntityAttributes.CurrentMoveSpeed, restoredMovementSpeed);

		entityEmitter.EmitEvent(EntityEvents.Available);
		entityEmitter.EmitEvent(EntityEvents.ResumeRotation);
	}

}
