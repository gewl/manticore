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
    float timeToCompleteRiposte = 0.4f;

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

        riposteZone.SetActive(false);

        yield return new WaitForSeconds(riposteCooldown - riposteDuration);

        isOnCooldown = false;
    }

    public void BeginRiposte(Transform bulletFirer)
    {
        StartCoroutine(FireRiposteAction(bulletFirer));
    }

    IEnumerator FireRiposteAction(Transform bulletFirer)
    {
        inputComponent.ActionsLocked = true;
        inputComponent.MovementLocked = true;

        healthComponent.IsInvulnerable = true;
        Material originalSkin = entityRenderer.material;
        entityRenderer.material = blinkSkin;
        entityCollider.enabled = false;

        yield return new WaitForSeconds(hangTimeBeforeRiposteStarts);

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

            transform.position = Vector3.Lerp(initialPosition, targetPosition, percentageComplete);
            transform.rotation = Quaternion.Lerp(initialRotation, bulletFirer.rotation, percentageComplete);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        trailRenderer.enabled = false;
        inputComponent.ActionsLocked = false;
        inputComponent.MovementLocked = false;
        healthComponent.IsInvulnerable = false;
        entityCollider.enabled = true;
        entityRenderer.material = originalSkin;
    }
}
