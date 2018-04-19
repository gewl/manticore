using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkHardware : EntityComponent, IHardware
{
    HardwareType _type = HardwareType.Blink;
    public HardwareType Type { get { return _type; } }

    HardwareUseType hardwareUseType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return hardwareUseType; } }

    BlinkHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as BlinkHardwareData;
    }

    [SerializeField]
    Modifier blinkStunModifier;
    [SerializeField]
    GameObject blinkReturnIndicator;
    GameObject instantiatedBlinkReturnIndicator;

    int BlinkMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(_type); } }
    float BlinkRange { get { return subtypeData.GetBlinkRange(BlinkMomentum); } }
	public float TimeToCompleteBlink { get { return subtypeData.GetTimeToCompleteBlink(BlinkMomentum); } }
    public int StaminaCost { get { return subtypeData.GetStaminaCost(BlinkMomentum); } }

    public bool IsInUse { get { return false; } }

    public bool DoesBlinkStun = false;
    bool isInReturnState = false;
    bool isReturnStateQueued = false;
    Vector3 returnPoint;

    int entityLayermask;

    // Serialized values
    [SerializeField]
    Material blinkMaterial;
    float hangTimeBeforeBlinkStarts = 0.1f;
    public float HangTimeBeforeBlinkStarts { get { return hangTimeBeforeBlinkStarts; } }

    [SerializeField]
    LayerMask terrainLayerMask;

    [SerializeField]
	SkinnedMeshRenderer entityMeshRenderer;
    TrailRenderer trailRenderer;
    ManticoreInputComponent inputComponent;
    EntityGearManagement gear;

    // State management
    bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown && canBlink; } }

    float BlinkCooldown { get { return subtypeData.GetCooldown(BlinkMomentum); } }
    float percentOfCooldownRemaining = 1.0f;
    public CooldownDelegate CooldownPercentUpdater { get; set; }
    public CooldownDelegate CooldownDurationUpdater { get; set; }

    bool canBlink = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        entityLayermask = 1 << LayerMask.NameToLayer("Entity");

        gear = GetComponent<EntityGearManagement>();
        inputComponent = GetComponent<ManticoreInputComponent>();
        if (blinkMaterial == null) 
        {
            Debug.LogError("Serialized values unassigned in BlinkComponent on " + transform.name); 
        }
    }

    override protected void Subscribe()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        entityEmitter.SubscribeToEvent(EntityEvents.Available, Unlock);
        entityEmitter.SubscribeToEvent(EntityEvents.Busy, Lock);
    }

    override protected void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Available, Unlock);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Busy, Lock);
    }

    #region IHardware methods
    public void UseActiveHardware()
    {
        StartCoroutine("FireBlink");
    }

    public void ApplyPassiveHardware(HardwareType activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        Debug.LogError("Attempting to apply Blink hardware passively");
    }

    void Lock()
    {
        canBlink = false;
    }

    void Unlock()
    {
        canBlink = true;
    }

    IEnumerator GoOnCooldown()
    {
        if (isReturnStateQueued)
        {
            yield break;
        }
        float timeOffCooldown = Time.time + BlinkCooldown;
        isOnCooldown = true;

        while (Time.time < timeOffCooldown)
        {
            float cooldownRemaining = timeOffCooldown - Time.time;
            percentOfCooldownRemaining = 1 - cooldownRemaining / BlinkCooldown;

            if (CooldownDurationUpdater != null)
            {
                CooldownDurationUpdater(cooldownRemaining);
            }

            if (CooldownPercentUpdater != null)
            {
                CooldownPercentUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownDurationUpdater != null)
        {
            CooldownDurationUpdater(percentOfCooldownRemaining);
        }
        if (CooldownPercentUpdater != null)
        {
            CooldownPercentUpdater(percentOfCooldownRemaining);
        }
        isOnCooldown = false;
    }
    #endregion

    IEnumerator FireBlink()
    {
        if (!isInReturnState)
        {
            gear.ApplyPassiveHardware(typeof(BlinkHardware), gameObject);
        }
        StartCoroutine(GoOnCooldown());
        // Entering blink state
        inputComponent.LockActions(true);
        inputComponent.LockMovement(true);

        Material originalSkin = entityMeshRenderer.material;
        entityMeshRenderer.material = blinkMaterial;
        trailRenderer.enabled = true;

		// Brief pause before blink proper begins
		yield return new WaitForSeconds(HangTimeBeforeBlinkStarts);

        // Get blink destination based on current movement.
        Vector3 currentDirection = (Vector3)entityInformation.GetAttribute(EntityAttributes.CurrentDirection);
        currentDirection.Normalize();
        Vector3 origin = transform.position;

        if (currentDirection == Vector3.zero)
        {
            currentDirection = transform.forward;
        }

        Vector3 destination;
        if (isInReturnState)
        {
            destination = returnPoint;
            isInReturnState = false;
            Destroy(instantiatedBlinkReturnIndicator);
        }
        else
        {
            destination = GetBlinkDestination(origin, currentDirection);
        }

		float step = 0f;
        float rate = 1 / TimeToCompleteBlink;

        while (step < 1f)
        {
            step += Time.deltaTime * rate;
            float curvedStep = GameManager.BlinkCompletionCurve.Evaluate(step);

			transform.position = Vector3.Lerp(origin, destination, curvedStep);
            yield return new WaitForEndOfFrame();
        }

        // Exiting blink state
        entityMeshRenderer.material = originalSkin;
        trailRenderer.enabled = false;

        inputComponent.LockActions(false);
        inputComponent.LockMovement(false);

        if (DoesBlinkStun)
        {
            DoesBlinkStun = false;

            ApplyBlinkStun(origin, destination);
        }

        if (isReturnStateQueued)
        {
            isInReturnState = true;
            isReturnStateQueued = false;
        }
		yield break;
    }

    void ApplyBlinkStun(Vector3 blinkOrigin, Vector3 blinkDestination)
    {
        Vector3 toDestination = blinkDestination - blinkOrigin;
        RaycastHit[] hits = Physics.RaycastAll(blinkOrigin, toDestination, toDestination.magnitude, entityLayermask);

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject entity = hits[i].collider.gameObject;
            EntityModifierHandler entityModifierHandler = entity.GetComponent<EntityModifierHandler>();

            if (entityModifierHandler != null)
            {
                entityModifierHandler.RegisterModifier(blinkStunModifier);
            }
        }
    }

    public void PutInReturnState()
    {
        isReturnStateQueued = true;
        returnPoint = transform.position;

        Quaternion particleRotation = Quaternion.Euler(-90f, 0f, 0f);
        instantiatedBlinkReturnIndicator = Instantiate(blinkReturnIndicator, transform.position, particleRotation);
        ParticleSystem[] indicatorParticles = instantiatedBlinkReturnIndicator.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < indicatorParticles.Length; i++)
        {
            ParticleSystem.MainModule main = indicatorParticles[i].main;
            main.startLifetime = BlinkCooldown;
        }

        StopCoroutine(GoOnCooldown());
        isOnCooldown = false;

        StartCoroutine(ReturnStateCooldown());
    }

    IEnumerator ReturnStateCooldown()
    {
        yield return new WaitForSeconds(BlinkCooldown);

        isReturnStateQueued = false;
        isInReturnState = false;
    }

    Vector3 GetBlinkDestination(Vector3 origin, Vector3 currentDirection)
    {
        Vector3 testClearPathOrigin = origin;
        testClearPathOrigin.y -= entityInformation.EntityCollider.bounds.extents.y / 2f;
        Vector3 destination;

        // Check to see if blink can carry target to full range, or if 
        // something is in the way. Set destination accordingly.
        RaycastHit blinkTestHit = new RaycastHit();

        if (Physics.Raycast(testClearPathOrigin, currentDirection, out blinkTestHit, BlinkRange, terrainLayerMask))
        {
            GameObject hitTerrain = blinkTestHit.collider.gameObject;

            if (hitTerrain.CompareTag("Ramp"))
            {
                destination = origin + (currentDirection * BlinkRange);
                Vector3 rampTopBlinkPoint = destination;

                rampTopBlinkPoint.y = blinkTestHit.collider.bounds.max.y + 1f;

                Ray testRay = new Ray(rampTopBlinkPoint, -transform.up);
                RaycastHit rampHeightHit = new RaycastHit();

                // Projects a ray from above the ramp down, for blinking "up" the ramp.
                if (Physics.Raycast(testRay, out rampHeightHit, terrainLayerMask))
                {
                    destination.y = rampHeightHit.point.y + entityInformation.EntityCollider.bounds.extents.y;

                    Vector3 directionToDestination = (destination - transform.position).normalized;

                    RaycastHit rampPathHit = new RaycastHit();

                    // If possible "blink up ramp" destination is found, checks again to see if any terrain
                    // prohibits full length of blink.
                    if (Physics.Raycast(origin, directionToDestination, out rampPathHit, BlinkRange, terrainLayerMask))
                    {
                        float distanceToHit = rampPathHit.distance - entityInformation.EntityCollider.bounds.size.z;
                        destination = origin + (directionToDestination * distanceToHit);
                    }
                }
                else
                // If it doesn't find the top of the ramp, cancel blink for safety.
                {
                    Debug.Log("Top of ramp not found; canceling blink.");
                    destination = transform.position;
                }
            }
            else
            {
                float distanceToHit = blinkTestHit.distance - entityInformation.EntityCollider.bounds.size.z;
                destination = origin + (currentDirection * distanceToHit);
            }
        }
        else
        {
			destination = origin + (currentDirection * BlinkRange);
		}

        return destination;
    }
}
