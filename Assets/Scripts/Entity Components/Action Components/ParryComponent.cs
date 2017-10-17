using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryComponent : EntityComponent {

    [SerializeField]
    float parryDamage = 50f;
    public float ParryDamage { get { return parryDamage; } }
    // Speed of swing.
    [SerializeField]
    float timeToCompleteParry;
    [SerializeField]
    float movementPenalty = 1f;

    [SerializeField]
    GameObject parryBox;
    public AnimationCurve swingCurve;

    // How quickly user has to chain inputs.
	[SerializeField]
	float timeToCombo;
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
        entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_Ready);
	}

    protected override void Unsubscribe()
    {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry_Ready);
	}

    #region EventListeners

    // Listener for Parry event received while in Ready state (has not yet parried).
    void OnParry_Ready()
    {
        Unsubscribe();
		StartCoroutine("ForwardParry"); 
    }

	void OnParry_DuringParry()
	{
        parryQueued = true;
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry_DuringParry);
	}

	void OnParry_AfterFirstParry()
	{
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry_AfterFirstParry);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
        StartCoroutine("BackwardParry");
	}

	void OnUpdate_AfterFirstParry()
    {
        if (currentComboTimer > 0f)
        {
            currentComboTimer -= Time.deltaTime;
        }
        else
        {
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry_AfterFirstParry);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
			UnlimitEntityAfterParry();
			Subscribe();
		}
    }

    #endregion

    IEnumerator ForwardParry()
    {
        LimitEntityInParry();
		parryBox.SetActive(true);
        parryCollider.enabled = true;
		yield return new WaitForSeconds(0.05f);

		float step = 0f;
        float lastStep = 0f;
		float rate = 1 / timeToCompleteParry;

        bool openedComboWindow = false;

        while (step < 1f)
        {
            step += Time.deltaTime * rate;

            float curvedStep = swingCurve.Evaluate(step);
            parryBox.transform.RotateAround(transform.position, Vector3.up * -1f, 150f * (curvedStep - lastStep));
            lastStep = curvedStep;

            if (step >= 0.8f && !openedComboWindow)
            {
                parryCollider.enabled = false;
                entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_DuringParry);
                openedComboWindow = true;
			}

            yield return null;
        }

		entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry_DuringParry);

		if (parryQueued)
        {
			parryQueued = false;
			StartCoroutine("BackwardParry");
            yield break;
        }
        currentComboTimer = timeToCombo;

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
		entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_AfterFirstParry);
		yield break;
	}

	IEnumerator BackwardParry()
	{
        parryCollider.enabled = true;

		float step = 0f;
		float smoothStep;
		float lastStep = 0f;

		yield return new WaitForSeconds(0.1f);

        float rate = 1 / (timeToCompleteParry * 2/3);

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

		entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_Ready);
		yield break;
	}

    void LimitEntityInParry()
    {
        float currentMovementSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
        float adjustedMovementSpeed = currentMovementSpeed * movementPenalty;
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, adjustedMovementSpeed);

		entityEmitter.EmitEvent(EntityEvents.Busy);
		entityEmitter.EmitEvent(EntityEvents.FreezeRotation);
	}

	void UnlimitEntityAfterParry()
	{
		float currentMovementSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
		float restoredMovementSpeed = currentMovementSpeed / movementPenalty;
		entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, restoredMovementSpeed);

		entityEmitter.EmitEvent(EntityEvents.Available);
		entityEmitter.EmitEvent(EntityEvents.ResumeRotation);
	}

    #region called by entity action handler (Parry child object)

    public void SuccessfulParryHandler()
    {
        entityEmitter.EmitEvent(EntityEvents.ParrySuccessful);
    }

    #endregion
}
