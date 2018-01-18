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
 
public class CoroutineQueue
{
    MonoBehaviour m_Owner = null;
    Coroutine m_InternalCoroutine = null;
    Queue<IEnumerator> actions = new Queue<IEnumerator>();

    public CoroutineQueue(MonoBehaviour aCoroutineOwner)
    {
        m_Owner = aCoroutineOwner;
    }

    void StartLoop()
    {
        m_InternalCoroutine = m_Owner.StartCoroutine(Process());
    }

    public void StopLoop()
    {
        m_Owner.StopCoroutine(m_InternalCoroutine);
    }

    public void EnqueueAction(IEnumerator aAction)
    {
        actions.Enqueue(aAction);
        if (m_InternalCoroutine == null)
        {
            StartLoop();
        }
    }

    private IEnumerator Process()
    {
        while (true)
        {
            if (actions.Count > 0)
                yield return m_Owner.StartCoroutine(actions.Dequeue());
            else
                yield return null;
        }
    }
}
