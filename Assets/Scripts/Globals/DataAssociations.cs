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
    Dictionary<HardwareTypes, Sprite> bubImageDict;
    public static Sprite GetHardwareTypeBubImage(HardwareTypes hardwareType)
    {
        return instance.bubImageDict[hardwareType];
    }
}
