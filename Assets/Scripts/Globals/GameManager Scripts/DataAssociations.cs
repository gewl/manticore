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
    Dictionary<HardwareType, Sprite> hardwareTypeToBubImageMap;
    public static Sprite GetHardwareTypeBubImage(HardwareType hardwareType)
    {
        return instance.hardwareTypeToBubImageMap[hardwareType];
    }

}
