using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    [SerializeField]
    List<GameObject> wallList;

    List<float> expandedWallScales;
    List<float> collapsedWallScales;

    float currentTriggerCount;

    void Awake()
    {
        expandedWallScales = new List<float>();
        collapsedWallScales = new List<float>();
        for (int i = 0; i < wallList.Count; i++)
        {
            Transform wall = wallList[i].transform;
            expandedWallScales.Add(wall.localScale.z);
            collapsedWallScales.Add(wall.localScale.z * 0.1f);
        }
    }

    public void RegisterEnterTrigger()
    {
        currentTriggerCount++;

        if (currentTriggerCount == 1)
        {
            StartCoroutine(CollapseWalls());
        }
    }

    public void RegisterExitTrigger()
    {
        currentTriggerCount--;

        if (currentTriggerCount == 0)
        {
            StartCoroutine(ExpandWalls());
        }
    }

    IEnumerator CollapseWalls()
    {
        float wallSlideTime = GameManager.WallSlideTime;
        AnimationCurve wallSlideCurve = GameManager.WallSlideCurve;
        for (int i = 0; i < wallList.Count; i++)
        {
            StartCoroutine(SlideWallDownward(i, wallSlideTime, wallSlideCurve));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlideWallDownward(int wallIndex, float slideTime, AnimationCurve slideCurve)
    {
        GameObject wall = wallList[wallIndex];

        Vector3 originalScale = wall.transform.localScale;
        // TODO: This bases Z scaling off of another dimension (Y, arbitrarily) to fix problem of scaling being repeatedly applied.
        // There has to be a better way to do this, as it's currently sliding walls multiple times, but additional slides are hidden.
        Vector3 finalScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, collapsedWallScales[wallIndex]);

        float timeElapsed = 0.0f;

        while (timeElapsed < slideTime)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / slideTime;

            wall.transform.localScale = Vector3.Lerp(originalScale, finalScale, slideCurve.Evaluate(percentageComplete));
            yield return null;
        }
    }

    IEnumerator ExpandWalls()
    {
        float wallSlideTime = GameManager.WallSlideTime;
        AnimationCurve wallSlideCurve =GameManager.WallSlideCurve;
        for (int i = 0; i < wallList.Count; i++)
        {
            StartCoroutine(SlideWallUpward(i, wallSlideTime, wallSlideCurve));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlideWallUpward(int wallIndex, float slideTime, AnimationCurve slideCurve)
    {
        GameObject wall = wallList[wallIndex];

        Vector3 originalScale = wall.transform.localScale;
        Vector3 finalScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, expandedWallScales[wallIndex]);

        float timeElapsed = 0.0f;

        while (timeElapsed < slideTime)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / slideTime;

            wall.transform.localScale = Vector3.Lerp(originalScale, finalScale, slideCurve.Evaluate(percentageComplete));
            yield return null;
        }
    }
}
