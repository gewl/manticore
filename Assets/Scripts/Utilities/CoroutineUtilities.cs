using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtilities 
{
    static int frameFreezeCount = 0;

    public static IEnumerator PauseForFrames(int frames)
    {
        if (frames < frameFreezeCount)
        {
            yield break;
        }

        frameFreezeCount = frames;
        Time.timeScale = 0f;
        while (true)
        {
            int framesEndPoint = Time.frameCount + frames;
            while (Time.frameCount < framesEndPoint)
            {
                yield return null;
            }
            Time.timeScale = 1f;
            frameFreezeCount = 0;
            yield break;
        }
    }
}
