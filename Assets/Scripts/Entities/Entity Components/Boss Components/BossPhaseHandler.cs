using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossPhaseHandler : SerializedMonoBehaviour {

    [SerializeField]
    List<MonoBehaviour> firstPhaseComponentsToDisable;
    [SerializeField]
    List<MonoBehaviour> secondPhaseComponentsToEnable;

    public void EndFirstPhase()
    {
        foreach (MonoBehaviour component in firstPhaseComponentsToDisable)
        {
            component.enabled = false;
        }
    }

}
