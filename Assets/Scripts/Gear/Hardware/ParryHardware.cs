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
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as ParryHardwareData;
    }

    int ParryMomentum
    {
        get
        {
            return MomentumManager.GetMomentumPointsByHardwareType(HardwareTypes.Parry);
        }
    }

    public float ParryDamage { get { return subtypeData.GetDamageDealt(ParryMomentum); } }
    public int StaminaCost
    {
        get
        {
            if (isInParry)
            {
                return subtypeData.GetStaminaCost(ParryMomentum) / 2;
            }
            else
            {
                return subtypeData.GetStaminaCost(ParryMomentum);
            }
        }
    }

    GameObject parryBox;
    const string PARRY_BOX_NAME = "ParryBox";

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
        parryBox = transform.Find(PARRY_BOX_NAME).gameObject;

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

        AnimationCurve swingCurve = GameManager.ParrySwingCurve;
		float step = 0f;
        float lastStep = 0f;
		float rate = 1 / subtypeData.GetTimeToCompleteParry(ParryMomentum);

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

        currentComboTimer = subtypeData.GetTimeToCombo(ParryMomentum);

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

        AnimationCurve swingCurve = GameManager.ParrySwingCurve;
		float step = 0f;
		float smoothStep;
		float lastStep = 0f;

		yield return new WaitForSeconds(0.1f);

        float rate = 1 / (subtypeData.GetTimeToCompleteParry(ParryMomentum) * 2/3);

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
        float currentMovementSpeed = (float)entityInformation.GetAttribute(EntityAttributes.CurrentMoveSpeed);
        float adjustedMovementSpeed = currentMovementSpeed * subtypeData.GetMovementModifier(ParryMomentum);
        entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, adjustedMovementSpeed);
		entityEmitter.EmitEvent(EntityEvents.Busy);
		entityEmitter.EmitEvent(EntityEvents.FreezeRotation);
	}

	void UnlimitEntityAfterParry()
	{
        isInParry = false;
		float currentMovementSpeed = (float)entityInformation.GetAttribute(EntityAttributes.CurrentMoveSpeed);
		float restoredMovementSpeed = currentMovementSpeed / subtypeData.GetMovementModifier(ParryMomentum);
		entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, restoredMovementSpeed);

		entityEmitter.EmitEvent(EntityEvents.Available);
		entityEmitter.EmitEvent(EntityEvents.ResumeRotation);
	}

}
