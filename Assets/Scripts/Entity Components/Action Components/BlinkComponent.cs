using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkComponent : EntityComponent
{
    [SerializeField]
    Material blinkMaterial;
    [SerializeField]
    float blinkRange;
	[SerializeField]
	float timeToCompleteBlink;
    [SerializeField]
	AnimationCurve blinkCompletionCurve;

    [SerializeField]
    LayerMask terrainLayerMask;

	MeshRenderer entityMeshRenderer;
    TrailRenderer trailRenderer;

    bool canBlink = true;

#if UNITY_EDITOR
    void OnEnable()
    {
        if (blinkMaterial == null || System.Math.Abs(blinkRange) < 0.1f)
        {
            Debug.LogError("Serialized values unassigned in BlinkComponent on " + transform.name); 
        }
    }
#endif

    override protected void Subscribe()
    {
        entityMeshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

		entityEmitter.SubscribeToEvent(EntityEvents.Blink, OnBlink);
        entityEmitter.SubscribeToEvent(EntityEvents.Available, Unlock);
        entityEmitter.SubscribeToEvent(EntityEvents.Busy, Lock);
    }

    override protected void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Blink, OnBlink);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Available, Unlock);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Busy, Lock);
    }

#region Event listeners
    void OnBlink()
    {
        if (canBlink)
        {
            StartCoroutine("FireBlink");
        }

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
        // Entering blink state
        entityEmitter.EmitEvent(EntityEvents.Stun);

        Material originalSkin = entityMeshRenderer.material;
        entityMeshRenderer.material = blinkMaterial;
        trailRenderer.enabled = true;

		// Brief pause before blink proper begins
		yield return new WaitForSeconds(0.1f);

        // Get blink destination based on current movement.
        Vector3 currentDirection = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentDirection);
        currentDirection.Normalize();
        Vector3 origin = transform.position;

        if (currentDirection == Vector3.zero)
        {
            currentDirection = transform.forward;
        }

        Vector3 destination;

        Vector3 testClearPathOrigin = origin;
        testClearPathOrigin.y -= entityData.EntityCollider.bounds.extents.y;
        // Check to see if blink can carry target to full range, or if 
        // something is in the way. Set destination accordingly.
        RaycastHit blinkTestHit = new RaycastHit();
        if (Physics.Raycast(testClearPathOrigin, currentDirection, out blinkTestHit, blinkRange, terrainLayerMask))
        {
            GameObject hitTerrain = blinkTestHit.collider.gameObject;
            // TODO: If forward cast intersects with a ramp, move player above the ramp so that they'll be put on it or on level that ramp leads up to (depending on
            // how far Blink carries them). This can lead to a moment of "dropping" onto the ramp when the Blink is complete, so it could be made a little smoother.
            if (hitTerrain.CompareTag("Ramp"))
            {
                destination = origin + (currentDirection * blinkRange);
                float rampHeight = blinkTestHit.collider.bounds.size.y;
                destination.y = destination.y + rampHeight + 0.01f;
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

		float step = 0f;
        float rate = 1 / timeToCompleteBlink;

        while (step < 1f)
        {
            step += Time.deltaTime * rate;
            float curvedStep = blinkCompletionCurve.Evaluate(step);

			transform.position = Vector3.Lerp(origin, destination, curvedStep);
            yield return new WaitForEndOfFrame();
        }

        // Exiting blink state
        entityMeshRenderer.material = originalSkin;
        trailRenderer.enabled = false;
		entityEmitter.EmitEvent(EntityEvents.Unstun);
		yield break;
    }
}
