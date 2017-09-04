using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityData : MonoBehaviour {

    Rigidbody entityRigidbody;
    public Rigidbody EntityRigidbody { get { return entityRigidbody; } }
    Collider entityCollider;
    public Collider EntityCollider { get { return entityCollider; } }

    // Dictionaries to hold attributes at runtime
    protected Dictionary<HardEntityAttributes, object> HardAttributes;
    protected Dictionary<SoftEntityAttributes, object> SoftAttributes;

    // Serializable lists holding information to be moved to dictionaries;
    // edited in inspector
    [SerializeField]
    protected List<HardAttributeEntry> hardAttributeList;

    // Init dictionaries, populate from lists
    private void Awake()
    {
        entityCollider = GetComponent<Collider>();
        entityRigidbody = GetComponent<Rigidbody>();
        HardAttributes = new Dictionary<HardEntityAttributes, object>();
        SoftAttributes = new Dictionary<SoftEntityAttributes, object>();

        InitializeHardAttributes();
    }

    // Components call this during initialization to be certain that any SoftAttribute data values they need will be
    // in the entity's data, populated from HardAttributes.

    public void Expect(SoftEntityAttributes attribute)
    {
        switch (attribute)
        {
            case SoftEntityAttributes.CurrentHealth:
                SoftAttributes[SoftEntityAttributes.CurrentHealth] = HardAttributes[HardEntityAttributes.StartingHealth];
                break;
            case SoftEntityAttributes.CurrentMoveSpeed:
                SoftAttributes[SoftEntityAttributes.CurrentMoveSpeed] = HardAttributes[HardEntityAttributes.BaseMoveSpeed];
                break;
            case SoftEntityAttributes.IsFriendly:
                SoftAttributes[SoftEntityAttributes.IsFriendly] = HardAttributes[HardEntityAttributes.StartsFriendly];
                break;
            case SoftEntityAttributes.CurrentRotationSpeed:
                SoftAttributes[SoftEntityAttributes.CurrentRotationSpeed] = HardAttributes[HardEntityAttributes.BaseRotationSpeed];
                break;
            default:
                break;
        }
    }

    // Functions for taking string values from inspectors, casting them to correct value,
    // and populating just-initialized dicts. Run once on Awake.

    protected void InitializeHardAttributes()
    {
        for (int i = 0; i < hardAttributeList.Count; i++)
        {
            HardEntityAttributes attribute = hardAttributeList[i].HardAttribute;
            string stringValue = hardAttributeList[i].value;
            Type intendedType = HardEntityAttributeTypes.GetType(attribute);

            HardAttributes[attribute] = Convert.ChangeType(stringValue, intendedType);
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
        if (object.ReferenceEquals(newValue.GetType(), SoftEntityAttributeTypes.GetType(attribute)))
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
