using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DataAssociations : SerializedMonoBehaviour {

    static DataAssociations instance;

    private void Awake()
    {
        instance = this;
    }

    [OdinSerialize]
    Dictionary<HardwareTypes, Sprite> hardwareTypeToBubImageMap;
    public static Sprite GetHardwareTypeBubImage(HardwareTypes hardwareType)
    {
        return instance.hardwareTypeToBubImageMap[hardwareType];
    }

    [OdinSerialize]
    Dictionary<GlobalConstants.EntityTypes, int> entityTypeToMomentumValueMap;
    public static int GetMomentumValueForEntityType(GlobalConstants.EntityTypes entityType)
    {
        return instance.entityTypeToMomentumValueMap[entityType];
    }
}
