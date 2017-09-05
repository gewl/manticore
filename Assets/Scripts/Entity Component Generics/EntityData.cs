using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityData : MonoBehaviour 
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
    protected Dictionary<HardEntityAttributes, object> HardAttributes;
    public Dictionary<SoftEntityAttributes, object> SoftAttributes;

    // Serializable lists holding information to be moved to dictionaries;
    // edited in inspector
    [SerializeField]
    protected List<HardAttributeEntry> hardAttributeList;

    // Init dictionaries, populate from lists
    private void Awake()
    {
        entityCollider = GetComponent<Collider>();
        entityRigidbody = GetComponent<Rigidbody>();
        entityTransform = GetComponent<Transform>();

        HardAttributes = new Dictionary<HardEntityAttributes, object>();
        SoftAttributes = new Dictionary<SoftEntityAttributes, object>();

        InitializeHardAttributes();
    }

    // Components call this during initialization to be certain that any SoftAttribute data values they need will be
    // in the entity's data, populated from HardAttributes.

    public void Expect(SoftEntityAttributes attribute)
    {
        HardEntityAttributes matchingHardAttribute;
            
        switch (attribute)
        {
            case SoftEntityAttributes.CurrentHealth:
                SoftAttributes[SoftEntityAttributes.CurrentHealth] = HardAttributes[HardEntityAttributes.StartingHealth];
                matchingHardAttribute = HardEntityAttributes.StartingHealth;
                break;
            case SoftEntityAttributes.CurrentMoveSpeed:
                SoftAttributes[SoftEntityAttributes.CurrentMoveSpeed] = HardAttributes[HardEntityAttributes.BaseMoveSpeed];
                matchingHardAttribute = HardEntityAttributes.BaseMoveSpeed;
                break;
            case SoftEntityAttributes.IsFriendly:
                SoftAttributes[SoftEntityAttributes.IsFriendly] = HardAttributes[HardEntityAttributes.StartsFriendly];
                matchingHardAttribute = HardEntityAttributes.StartsFriendly;
                break;
            case SoftEntityAttributes.CurrentRotationSpeed:
                matchingHardAttribute = HardEntityAttributes.BaseRotationSpeed;
                break;
            default:
                Debug.LogError(attribute);
                throw new Exception("No case for expected soft attribute (identified above.)");
        }

        SoftAttributes[attribute] = DegenericizeHardAttribute(matchingHardAttribute, HardAttributes[matchingHardAttribute]);
    }

    private object DegenericizeHardAttribute(HardEntityAttributes attribute, object attributeValue)
    {
        Type intendedType = HardEntityAttributeTypes.GetType(attribute);
        return Convert.ChangeType(attribute, intendedType);
    }

    // Functions for taking string values from inspectors, casting them to correct value,
    // and populating just-initialized dicts. Run once on Awake.

    protected void InitializeHardAttributes()
    {
        for (int i = 0; i < hardAttributeList.Count; i++)
        {
            HardEntityAttributes attribute = hardAttributeList[i].HardAttribute;
            string stringValue = hardAttributeList[i].value;

            HardAttributes[attribute] = DegenericizeHardAttribute(attribute, stringValue);
        }
    }

    // Simple getters.
    public object GetHardAttribute(HardEntityAttributes attribute)
    {
        return HardAttributes[attribute];
    }

    public object GetSoftAttribute(SoftEntityAttributes attribute)
    {
        try
        {
            Type attributeType = SoftEntityAttributeTypes.GetType(attribute);
            return SoftAttributes[attribute];
        }
        catch (Exception)
        {
            Debug.Log("Soft attribute not found:");
            Debug.Log(attribute);
            return null;
        }
    }

    // Setter for soft attribute that checks input value against expected type for that attribute.
    public void SetSoftAttribute(SoftEntityAttributes attribute, object newValue)
    {
        if (!object.ReferenceEquals(newValue.GetType(), SoftEntityAttributeTypes.GetType(attribute)))
        {
            Debug.Log("Tried to update SoftAttribute with invalid type.");
            Debug.Log("Attribute:");
            Debug.Log(attribute);
            Debug.Log("Value:");
            Debug.Log(newValue);
            Debug.Log("Expected type:");
            Debug.Log(SoftEntityAttributeTypes.GetType(attribute));
            return;
        }

        SoftAttributes[attribute] = newValue;
    }
}
