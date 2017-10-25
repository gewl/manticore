using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    [SerializeField]
    List<GameObject> wallList;
    [SerializeField]
    GameObject roof;

    MeshRenderer roofRenderer;

    List<float> expandedWallScales;
    List<float> collapsedWallScales;

    float currentTriggerCount;

    void Awake()
    {
        roofRenderer = roof.GetComponent<MeshRenderer>();

        expandedWallScales = new List<float>();
        collapsedWallScales = new List<float>();
        for (int i = 0; i < wallList.Count; i++)
        {
            Transform wall = wallList[i].transform;
            expandedWallScales.Add(wall.localScale.z);
            collapsedWallScales.Add(wall.localScale.z * 0.2f);
        }
    }

    public void RegisterEnterTrigger()
    {
        currentTriggerCount++;

        if (currentTriggerCount == 1)
        {
            StartCoroutine(TransitionRoof(true));
            StartCoroutine(CollapseWalls());
        }
    }

    public void RegisterExitTrigger()
    {
        currentTriggerCount--;

        if (currentTriggerCount == 0)
        {
            StartCoroutine(TransitionRoof(false));
            StartCoroutine(ExpandWalls());
        }
    }

    IEnumerator TransitionRoof(bool isFading)
    {
        Color originalColor = roofRenderer.material.color;
        Color finalColor = originalColor;

        finalColor.a = isFading ? 0f : 1f;

        float transitionTime = GameManager.RoomTransitionTime;
        AnimationCurve transitionCurve = GameManager.RoomTransitionCurve;

        float timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / transitionTime;

            roofRenderer.material.color = Color.Lerp(originalColor, finalColor, transitionCurve.Evaluate(percentageComplete));
            yield return null;
        }
    }

    IEnumerator CollapseWalls()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallList.Count; i++)
        {
            StartCoroutine(SlideWallDownward(i, roomTransitionTime, roomTransitionCurve));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlideWallDownward(int wallIndex, float slideTime, AnimationCurve slideCurve)
    {
        GameObject wall = wallList[wallIndex];

        Vector3 originalScale = wall.transform.localScale;
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
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallList.Count; i++)
        {
            StartCoroutine(SlideWallUpward(i, roomTransitionTime, roomTransitionCurve));
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
