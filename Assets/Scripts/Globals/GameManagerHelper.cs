using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class GameManagerHelper : SerializedMonoBehaviour {

    IEnumerator freezeCoroutine;

    [OdinSerialize]
    Dictionary<GlobalConstants.GameFreezeEvent, int> gameFreezeTimers;

    public void FreezeFrame(GlobalConstants.GameFreezeEvent freezeEvent)
    {
        int freezeFrameCount = gameFreezeTimers[freezeEvent];
        freezeCoroutine = CoroutineUtilities.PauseForFrames(freezeFrameCount);

        StartCoroutine(freezeCoroutine);
    }

    //IEnumerator Freeze(int freezeFrameCount)
    //{
    //    int frameCounter = 0;
    //    Time.timeScale = 0f;
    //    GameManager.MuteAllEmitters();
    //    while (frameCounter < freezeFrameCount)
    //    {
    //        frameCounter++;
    //        yield return new WaitForFixedUpdate();
    //    }
    //    Time.timeScale = 1f;
    //    GameManager.UnmuteAllEmitters();
    //}

}
