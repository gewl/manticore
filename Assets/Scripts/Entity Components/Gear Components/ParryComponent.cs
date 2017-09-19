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

	Vector3 parryReadyPosition;
    Quaternion parryReadyRotation;

    Vector3 afterParryPosition;
    Quaternion afterParryRotation;

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

		afterParryPosition = new Vector3(-parryReadyPosition.x, 0f, parryReadyPosition.z);
		afterParryRotation = Quaternion.Euler(0f, -parryReadyRotation.eulerAngles.y, 0f);
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

	void OnParry_AfterFirstParry()
	{
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
			Subscribe();
		}
    }

    #endregion

    IEnumerator ForwardParry()
    {
        parryBox.SetActive(true);

        float step = 0f;
        float smoothStep;
        float lastStep = 0f;
		currentState = ParryState.InFirstParry;

        yield return new WaitForSeconds(0.05f);

        float rate = 1 / timeToCompleteParry;

        while (step < 1f)
        {
            step += Time.deltaTime * rate;

            smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step);
            parryBox.transform.RotateAround(transform.position, Vector3.up * -1f, 150f * (smoothStep - lastStep));
            lastStep = smoothStep;
            yield return null;
        }

        //while (timeInParry < timeToCompleteParry)
        //{
        //    timeInParry += Time.smoothDeltaTime;
        //    float percentageComplete = timeInParry / timeToCompleteParry;

        //    parryBox.transform.RotateAround(transform.position, Vector3.up * -1f, 150f * percentageComplete);
        //    //parryBox.transform.localPosition = Vector3.Lerp(parryReadyPosition, afterParryPosition, swingCurve.Evaluate(percentageComplete));
        //    //parryBox.transform.localRotation = Quaternion.Lerp(parryReadyRotation, afterParryRotation, swingCurve.Evaluate(percentageComplete));
        //    yield return null;
        //}

        //parryBox.transform.localPosition = afterParryPosition;
        //parryBox.transform.localRotation = afterParryRotation;

        currentComboTimer = timeToCombo;
        currentState = ParryState.AfterFirstParry;

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate_AfterFirstParry);
		entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry_AfterFirstParry);
		yield break;
	}
}
