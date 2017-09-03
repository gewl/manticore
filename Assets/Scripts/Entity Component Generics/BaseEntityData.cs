using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntityData : MonoBehaviour {

    protected Dictionary<HardEntityAttributes, object> HardAttributes;
    protected Dictionary<SoftEntityAttributes, object> SoftAttributes;

    public object GetHardAttribute(HardEntityAttributes attribute)
    {
        return HardAttributes[attribute];
    }

    public object GetSoftAttribute(SoftEntityAttributes attribute)
    {
        return SoftAttributes[attribute];
    }

    public void ChangeSoftAttribute(SoftEntityAttributes attribute, object newValue)
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
