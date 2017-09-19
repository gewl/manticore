using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryComponent : EntityComponent {

    [SerializeField]
    GameObject parryBox;
    public AnimationCurve swingCurve;
    // Speed of swing.
    [SerializeField]
    float timeToCompleteParry;

    // How quickly user has to chain inputs.
	[SerializeField]
	float timeToCombo;
    // Current expiration timer on combo chain.
    float currentComboTimer;
    bool parryQueued = false;

	Vector3 parryReadyPosition;
    Quaternion parryReadyRotation;

	enum ParryState {
        Ready,
        InFirstParry,
        AfterFirstParry,
        InSecondParry,
        AfterSecondParry
    }

    ParryState currentState;

    override protected void Awake()
    {
        base.Awake();
		parryReadyPosition = parryBox.transform.localPosition;
		parryReadyRotation = parryBox.transform.localRotation;
	}

    // Generally use to prime parry box (and controller) for "Ready" state.
    override protected void Subscribe()
    {
		parryBox.transform.localPosition = parryReadyPosition;
		parryBox.transform.localRotation = parryReadyRotation;
		parryBox.SetActive(false);
		currentState = ParryState.Ready;
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
			entityEmitter.EmitEvent(EntityEvents.ResumeRotation);
			Subscribe();
		}
    }

    #endregion

    IEnumerator ForwardParry()
    {
        parryBox.SetActive(true);
        entityEmitter.EmitEvent(EntityEvents.FreezeRotation);
		yield return new WaitForSeconds(0.05f);

		float step = 0f;
        float lastStep = 0f;
		float rate = 1 / timeToCompleteParry;
		currentState = ParryState.InFirstParry;

        bool openedComboWindow = false;

        while (step < 1f)
        {
            step += Time.deltaTime * rate;

            float smoothStep = swingCurve.Evaluate(step);
            parryBox.transform.RotateAround(transform.position, Vector3.up * -1f, 150f * (smoothStep - lastStep));
            lastStep = smoothStep;

            if (step >= 0.9f && !openedComboWindow)
            {
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
        currentState = ParryState.AfterFirstParry;

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
		entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_AfterFirstParry);
		yield break;
	}

	IEnumerator BackwardParry()
	{
		entityEmitter.EmitEvent(EntityEvents.FreezeRotation);

		float step = 0f;
		float smoothStep;
		float lastStep = 0f;
		currentState = ParryState.InSecondParry;

		yield return new WaitForSeconds(0.05f);

        float rate = 1 / (timeToCompleteParry * 2/3);

		while (step < 1f)
		{
			step += Time.deltaTime * rate;

			smoothStep = swingCurve.Evaluate(step);
			parryBox.transform.RotateAround(transform.position, Vector3.up, 150f * (smoothStep - lastStep));
			lastStep = smoothStep;
			yield return null;
		}

		currentState = ParryState.Ready;

        parryBox.transform.localPosition = parryReadyPosition;
        parryBox.transform.localRotation = parryReadyRotation;
        parryBox.SetActive(false);
		entityEmitter.EmitEvent(EntityEvents.ResumeRotation);

		entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_Ready);
		yield break;
	}
}
