using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    [SerializeField]
    List<GameObject> wallList;

    float currentTriggerCount;
    bool areWallsCollapsed = false;

    public void RegisterEnterTrigger()
    {
        currentTriggerCount++;

        if (currentTriggerCount > 0 && !areWallsCollapsed)
        {
            StartCoroutine(CollapseWalls());
        }
    }

    public void RegisterExitTrigger()
    {
        currentTriggerCount--;

        if (currentTriggerCount <= 0 && areWallsCollapsed)
        {
            StartCoroutine(ExpandWalls());
        }
    }

    IEnumerator CollapseWalls()
    {
        areWallsCollapsed = true;

        float wallSlideTime = GameManager.WallSlideTime;
        AnimationCurve wallSlideCurve =GameManager.WallSlideCurve;
        foreach (GameObject wall in wallList)
        {
            StartCoroutine(SlideWallDownward(wall, wallSlideTime, wallSlideCurve));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlideWallDownward(GameObject wall, float slideTime, AnimationCurve slideCurve)
    {
        Vector3 originalScale = wall.transform.localScale;
        Vector3 finalScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, wall.transform.localScale.z * 0.1f);

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
        areWallsCollapsed = false;

        float wallSlideTime = GameManager.WallSlideTime;
        AnimationCurve wallSlideCurve =GameManager.WallSlideCurve;
        foreach (GameObject wall in wallList)
        {
            StartCoroutine(SlideWallUpward(wall, wallSlideTime, wallSlideCurve));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlideWallUpward(GameObject wall, float slideTime, AnimationCurve slideCurve)
    {
        Vector3 originalScale = wall.transform.localScale;
        Vector3 finalScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, wall.transform.localScale.y);

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
