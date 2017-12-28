using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class stores data for the entity to which it's assigned, as well as implementing functionality for accessing/updating that data.
/// </summary>
[Serializable]
public class EntityInformation : MonoBehaviour 
{
    // Holds references to components that all entities will have. If they don't, something is wrong,
    // so the game _should_ break then.
    Rigidbody entityRigidbody;
    public Rigidbody EntityRigidbody { get { return entityRigidbody; } }
    Collider entityCollider;
    public Collider EntityCollider { get { return entityCollider; } }
    Transform entityTransform;
    public Transform EntityTransform { get { return entityTransform; } }

    // Dictionaries to hold attributes at runtime
    public Dictionary<EntityAttributes, object> Attributes;

    [SerializeField]
    public EntityData Data;

    // Init dictionaries, populate from lists
    private void Awake()
    {
        entityCollider = GetComponent<Collider>();
        entityRigidbody = GetComponent<Rigidbody>();
        entityTransform = GetComponent<Transform>();

        Attributes = new Dictionary<EntityAttributes, object>();
    }

    // Components call this during initialization to be certain that any SoftAttribute data values they need will be

    // Simple getters.
    public object GetAttribute(EntityAttributes attribute)
    {
        try
        {
            return Attributes[attribute];
        }
        catch (Exception)
        {
            Debug.Log("Soft attribute not found:");
            Debug.Log(attribute);
            return null;
        }
    }

    // Setter for soft attribute that checks input value against expected type for that attribute.
    public void SetAttribute(EntityAttributes attribute, object newValue)
    {
        if (!object.ReferenceEquals(newValue.GetType(), EntityAttributeTypes.GetType(attribute)))
        {
            Debug.Log("Tried to update SoftAttribute with invalid type.");
            Debug.Log("Attribute:");
            Debug.Log(attribute);
            Debug.Log("Value:");
            Debug.Log(newValue);
            Debug.Log("Expected type:");
            Debug.Log(EntityAttributeTypes.GetType(attribute));
            return;
        }

        Attributes[attribute] = newValue;
    }
}
