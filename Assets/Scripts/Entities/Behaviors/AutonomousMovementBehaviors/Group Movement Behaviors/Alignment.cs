using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a weird one: For the time being, it's going to manipulate the rotation
// of the object on which it's acting, but is not going to return any actual steering force.
// It might be better-suited to being moved out of the AutonomousMovementBehavior system and made
// into a standalone component, or one that another rotational-movement component manages.
public class Alignment : AutonomousMovementBehavior, ITriggerBasedMovementBehavior {

    const string entityTriggerName = "Entity Trigger";
    const int entityTriggerLayer = 22;

    SphereCollider entityTrigger;
    
    #region Tagged neighbors management
    HashSet<Transform> _taggedNeighbors;
    HashSet<Transform> TaggedNeighbors
    {
        get
        {
            if (_taggedNeighbors == null)
            {
                _taggedNeighbors = new HashSet<Transform>();
            }
            return _taggedNeighbors;
        }
    }

    public void RegisterCollider(Collider collider)
    {
        TaggedNeighbors.Add(collider.transform);
    }

    public void DeregisterCollider(Collider collider)
    {
        TaggedNeighbors.Remove(collider.transform);
    }
    #endregion

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (entityTrigger == null)
        {
            entityTrigger = InitializeEntityTrigger(movementComponent.gameObject, movementComponent.SeparationRadius);
        }

        Quaternion averageHeading = Quaternion.identity;
        Vector4 cumulative = Vector4.zero;
        Quaternion entityHeading = movementComponent.transform.rotation;
        int headingCount = 0;

        HashSet<Transform>.Enumerator neighborEnumerator = TaggedNeighbors.GetEnumerator();

        while (neighborEnumerator.MoveNext())
        {
            Transform neighbor = neighborEnumerator.Current;
            Quaternion neighborHeading = neighbor.rotation;
            headingCount++;

            averageHeading = AverageQuaternion(ref cumulative, neighborHeading, entityHeading, headingCount);
        }

        movementComponent.transform.rotation = averageHeading;

        return Vector3.zero;
    }

    SphereCollider InitializeEntityTrigger(GameObject parentObject, float triggerRadius)
    {
        GameObject entityTriggerContainer = new GameObject(entityTriggerName)
        {
            layer = LayerMask.NameToLayer("EntityTrigger")
        };
        MovementBehaviorTriggerController triggerController = entityTriggerContainer.AddComponent<MovementBehaviorTriggerController>();
        triggerController.RegisterControllingBehavior(this);

        entityTriggerContainer.transform.SetParent(parentObject.transform);
        entityTriggerContainer.transform.localPosition = Vector3.zero;

        SphereCollider instantiatedTrigger = entityTriggerContainer.AddComponent<SphereCollider>();
        instantiatedTrigger.isTrigger = true;
        instantiatedTrigger.radius = triggerRadius;

        return instantiatedTrigger;
    }

    Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
    {
        float w = 0.0f;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
        //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
        if (!AreQuaternionsClose(newRotation, firstRotation))
        {

            newRotation = InverseSignQuaternion(newRotation);
        }

        //Average the values
        float addDet = 1f / (float)addAmount;
        cumulative.w += newRotation.w;
        w = cumulative.w * addDet;
        cumulative.x += newRotation.x;
        x = cumulative.x * addDet;
        cumulative.y += newRotation.y;
        y = cumulative.y * addDet;
        cumulative.z += newRotation.z;
        z = cumulative.z * addDet;

        //note: if speed is an issue, you can skip the normalization step
        return NormalizeQuaternion(x, y, z, w);
    }

    Quaternion NormalizeQuaternion(float x, float y, float z, float w)
    {

        float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
        w *= lengthD;
        x *= lengthD;
        y *= lengthD;
        z *= lengthD;

        return new Quaternion(x, y, z, w);
    }

    //Changes the sign of the quaternion components. This is not the same as the inverse.
    Quaternion InverseSignQuaternion(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
    {

        float dot = Quaternion.Dot(q1, q2);

        if (dot < 0.0f)
        {

            return false;
        }

        else
        {

            return true;
        }
    }

}
