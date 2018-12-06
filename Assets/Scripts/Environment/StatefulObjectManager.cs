using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class StatefulObjectManager : SerializedMonoBehaviour {
    [SerializeField]
    List<StatefulObject> statefulObjectsInScene;

    public List<string> GetAllStatefulSceneTags()
    {
        return statefulObjectsInScene
            .Select(o => o.GetStateTag())
            .ToList();
    }
}
