using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityData : MonoBehaviour {

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
        HardAttributes = new Dictionary<HardEntityAttributes, object>();

        InitializeHardAttributes();
    }

    // Functions for taking string values from inspectors, casting them to correct value,
    // and populating just-initialized dicts. Run once on Awake.

    protected void InitializeHardAttributes()
    {
        for (int i = 0; i < hardAttributeList.Count; i++)
        {
            HardEntityAttributes attribute = hardAttributeList[i].HardAttribute;
            string stringValue = hardAttributeList[i].value;
            Type intendedType = Type.GetType(HardEntityAttributeTypes.GetType(attribute));

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
        return SoftAttributes[attribute];
    }

    // Setter for soft attribute that checks input value against expected type for that attribute.
    public void SetSoftAttribute(SoftEntityAttributes attribute, object newValue)
    {
        if (object.ReferenceEquals(newValue.GetType(), Type.GetType(SoftEntityAttributeTypes.GetType(attribute))))
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
