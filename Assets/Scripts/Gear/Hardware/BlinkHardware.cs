using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkHardware : EntityComponent, IHardware
{
    // Serialized values
    [SerializeField]
    Material blinkMaterial;
    [SerializeField]
    float blinkRange;
	[SerializeField]
	float timeToCompleteBlink;
    public float TimeToCompleteBlink { get { return timeToCompleteBlink; } }
    float hangTimeBeforeBlinkStarts = 0.1f;
    public float HangTimeBeforeBlinkStarts { get { return hangTimeBeforeBlinkStarts; } }

    [SerializeField]
    LayerMask terrainLayerMask;

	MeshRenderer entityMeshRenderer;
    TrailRenderer trailRenderer;
    ManticoreInputComponent inputComponent;
    EntityGearManagement gear;

    // IHardware properties
    int baseStaminaCost = 40;
    public int BaseStaminaCost { get { return baseStaminaCost; } }
    public int UpdatedStaminaCost { get { return baseStaminaCost; } }

    // State management
    bool isOnCooldown = false;
    bool canBlink = true;
    public bool IsOnCooldown { get { return isOnCooldown && canBlink; } }

    protected override void OnEnable()
    {
        base.OnEnable();
        gear = GetComponent<EntityGearManagement>();
        inputComponent = GetComponent<ManticoreInputComponent>();
        if (blinkMaterial == null || System.Math.Abs(blinkRange) < 0.1f)
        {
            Debug.LogError("Serialized values unassigned in BlinkComponent on " + transform.name); 
        }
    }

    override protected void Subscribe()
    {
        entityMeshRenderer = GetComponent<MeshRenderer>();
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

    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
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
    #endregion

    IEnumerator FireBlink()
    {
        gear.ApplyPassiveHardwareToBlink(gameObject);
        isOnCooldown = true;
        // Entering blink state
        inputComponent.ActionsLocked = true;
        inputComponent.MovementLocked = true;

        Material originalSkin = entityMeshRenderer.material;
        entityMeshRenderer.material = blinkMaterial;
        trailRenderer.enabled = true;

		// Brief pause before blink proper begins
		yield return new WaitForSeconds(HangTimeBeforeBlinkStarts);

        // Get blink destination based on current movement.
        Vector3 currentDirection = (Vector3)entityData.GetAttribute(EntityAttributes.CurrentDirection);
        currentDirection.Normalize();
        Vector3 origin = transform.position;

        if (currentDirection == Vector3.zero)
        {
            currentDirection = transform.forward;
        }

        Vector3 destination = GetBlinkDestination(origin, currentDirection);

		float step = 0f;
        float rate = 1 / timeToCompleteBlink;

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
        isOnCooldown = false;

        inputComponent.ActionsLocked = false;
        inputComponent.MovementLocked = false;
		yield break;
    }

    Vector3 GetBlinkDestination(Vector3 origin, Vector3 currentDirection)
    {
        Vector3 testClearPathOrigin = origin;
        testClearPathOrigin.y -= entityData.EntityCollider.bounds.extents.y / 2f;
        Vector3 destination;

        // Check to see if blink can carry target to full range, or if 
        // something is in the way. Set destination accordingly.
        RaycastHit blinkTestHit = new RaycastHit();

        if (Physics.Raycast(testClearPathOrigin, currentDirection, out blinkTestHit, blinkRange, terrainLayerMask))
        {
            GameObject hitTerrain = blinkTestHit.collider.gameObject;

            if (hitTerrain.CompareTag("Ramp"))
            {
                destination = origin + (currentDirection * blinkRange);
                Vector3 rampTopBlinkPoint = destination;

                rampTopBlinkPoint.y = blinkTestHit.collider.bounds.max.y + 1f;

                Ray testRay = new Ray(rampTopBlinkPoint, -transform.up);
                RaycastHit rampHeightHit = new RaycastHit();

                // Projects a ray from above the ramp down, for blinking "up" the ramp.
                if (Physics.Raycast(testRay, out rampHeightHit, terrainLayerMask))
                {
                    destination.y = rampHeightHit.point.y + entityData.EntityCollider.bounds.extents.y;

                    Vector3 directionToDestination = (destination - transform.position).normalized;

                    RaycastHit rampPathHit = new RaycastHit();

                    // If possible "blink up ramp" destination is found, checks again to see if any terrain
                    // prohibits full length of blink.
                    if (Physics.Raycast(origin, directionToDestination, out rampPathHit, blinkRange, terrainLayerMask))
                    {
                        float distanceToHit = rampPathHit.distance - entityData.EntityCollider.bounds.size.z;
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
                float distanceToHit = blinkTestHit.distance - entityData.EntityCollider.bounds.size.z;
                destination = origin + (currentDirection * distanceToHit);
            }
        }
        else
        {
			destination = origin + (currentDirection * blinkRange);
		}

        return destination;
    }
}
