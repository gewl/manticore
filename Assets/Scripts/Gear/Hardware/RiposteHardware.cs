using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiposteHardware : MonoBehaviour, IHardware {

    int baseStaminaCost = 80;
    public int BaseStaminaCost { get { return baseStaminaCost; } }
    public int UpdatedStaminaCost { get { return baseStaminaCost; } }

    bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }

    float riposteDuration = 1f;
    float riposteCooldown = 6f;
    float hangTimeBeforeRiposteStarts = 0.1f;
    float timeToCompleteRiposte = 0.8f;

    GameObject riposteZone;
    const string RIPOSTE_ZONE = "RiposteZone";

    MobileEntityHealthComponent healthComponent;

    Renderer entityRenderer;
    ManticoreInputComponent inputComponent;
    Material blinkSkin;
    Collider entityCollider;
    TrailRenderer trailRenderer;

    private void OnEnable()
    {
        entityRenderer = GetComponent<Renderer>();
        entityCollider = GetComponent<Collider>();
        trailRenderer = GetComponent<TrailRenderer>();
        inputComponent = GetComponent<ManticoreInputComponent>();
        healthComponent = GetComponent<MobileEntityHealthComponent>();

        riposteZone = transform.Find(RIPOSTE_ZONE).gameObject;
        blinkSkin = riposteZone.GetComponent<Renderer>().material;
    }

    public void UseActiveHardware()
    {
        StartCoroutine(EnterRiposteState());
    }

    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
    {

    }

    IEnumerator EnterRiposteState()
    {
        isOnCooldown = true;
        riposteZone.SetActive(true);
        healthComponent.IsInvulnerable = true;

        yield return new WaitForSeconds(riposteDuration);

        healthComponent.IsInvulnerable = false;
        riposteZone.SetActive(false);

        yield return new WaitForSeconds(riposteCooldown - riposteDuration);

        isOnCooldown = false;
    }

    public IEnumerator FireRiposteAction(Transform bulletFirer)
    {
        Material originalSkin = entityRenderer.material;
        entityRenderer.material = blinkSkin;
        entityCollider.enabled = false;

        yield return new WaitForSeconds(hangTimeBeforeRiposteStarts);

        inputComponent.ActionsLocked = true;
        inputComponent.MovementLocked = true;
        trailRenderer.enabled = true;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float distanceBehindTarget = bulletFirer.GetComponent<Collider>().bounds.extents.z + entityCollider.bounds.size.z;
        Vector3 destinationPositionLocalToTarget = new Vector3(0f, 0f, -distanceBehindTarget);

        float timeElapsed = 0.0f;
        AnimationCurve blinkCurve = GameManager.BlinkCompletionCurve;

        while (timeElapsed < timeToCompleteRiposte)
        {
            float percentageComplete = timeElapsed / timeToCompleteRiposte;

            Vector3 targetPosition = bulletFirer.TransformPoint(destinationPositionLocalToTarget);
            float curveEvaluation = blinkCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, curveEvaluation);
            transform.rotation = Quaternion.Lerp(initialRotation, bulletFirer.rotation, curveEvaluation);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        trailRenderer.enabled = false;
        inputComponent.ActionsLocked = false;
        inputComponent.MovementLocked = false;
        entityCollider.enabled = true;
        entityRenderer.material = originalSkin;
    }
}
