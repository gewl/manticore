using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtilities 
{
    public static IEnumerator PauseForFrames(int frames)
    {
        Time.timeScale = 0f;
        while (true)
        {
            int framesEndPoint = Time.frameCount + frames;
            while (Time.frameCount < framesEndPoint)
            {
                yield return null;
            }
            Time.timeScale = 1f;
        }
    }
}
