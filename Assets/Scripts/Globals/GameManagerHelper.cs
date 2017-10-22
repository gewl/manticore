using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class GameManagerHelper : SerializedMonoBehaviour {

    static IEnumerator freezeCoroutine;
    public static bool[] collectibleTracker;

    [OdinSerialize]
    Dictionary<GlobalConstants.GameFreezeEvent, int> gameFreezeTimers;
    [OdinSerialize]
    Dictionary<GlobalConstants.Collectibles, GameObject> collectiblePrefabs;

    void Awake()
    {
        collectibleTracker = new bool[Enum.GetNames(typeof(GlobalConstants.Collectibles)).Length];
    }

    public void FreezeFrame(GlobalConstants.GameFreezeEvent freezeEvent)
    {
        int freezeFrameCount = gameFreezeTimers[freezeEvent];
        freezeCoroutine = CoroutineUtilities.PauseForFrames(freezeFrameCount);

        StartCoroutine(freezeCoroutine);
    }

    public bool TryToRegisterCollectible(GlobalConstants.Collectibles collectible)
    {
        int collectibleInt = (int)collectible;

        if (collectibleTracker[collectibleInt])
        {
            return false;
        }
        else
        {
            collectibleTracker[collectibleInt] = true;
            return true;
        }
    }

    public void DeregisterCollectible(GlobalConstants.Collectibles collectible)
    {
        int collectibleInt = (int)collectible;
        collectibleTracker[collectibleInt] = false;
    }
}
