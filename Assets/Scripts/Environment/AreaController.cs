using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2;

public class AreaController : MonoBehaviour {

    [Header("Area Sections")]
    [SerializeField]
    List<GameObject> fadableWallList;
    [SerializeField]
    GameObject roof;
    [SerializeField]
    GameObject interior;
    [SerializeField]
    List<GameObject> sectionsToFadeWhenAreaFaded;
    [SerializeField]
    List<GameObject> sectionsToHideWhenAreaFaded;

    [Header("Config")]
    [SerializeField]
    float playerInAreaFadeValue = 0f;
    [SerializeField]
    float areaFadedFadeValue = 0.3f;

    [Header("References to External Objects")]
    [SerializeField]
    List<AreaController> areasToFadeWhenActive;

    MeshRenderer roofRenderer;
    List<MeshRenderer> sectionsToFadeRenderers;
    List<MeshRenderer> wallRenderers;

    List<float> expandedWallScales;
    List<float> collapsedWallScales;

    Dictionary<GameObject, int> interiorEntityTracker;
    List<GameObject> entitiesToRemove;

    float currentPlayerTriggerCount;
    bool isAreaFaded = false;

    void Awake()
    {
        if (roof != null)
        {
            roofRenderer = roof.GetComponent<MeshRenderer>();
        }

        expandedWallScales = new List<float>();
        wallRenderers = new List<MeshRenderer>();
        collapsedWallScales = new List<float>();
        interiorEntityTracker = new Dictionary<GameObject, int>();
        entitiesToRemove = new List<GameObject>();
        for (int i = 0; i < fadableWallList.Count; i++)
        {
            wallRenderers.Add(fadableWallList[i].GetComponent<MeshRenderer>());
        }
    }

    void InitializeSectionsToFadeRenderers()
    {
        sectionsToFadeRenderers = new List<MeshRenderer>();

        for (int i = 0; i < sectionsToFadeWhenAreaFaded.Count; i++)
        {
            MeshRenderer sectionRenderer = sectionsToFadeWhenAreaFaded[i].GetComponent<MeshRenderer>();

            sectionsToFadeRenderers.Add(sectionRenderer);
        }
    }

    void ToggleAreaActive(bool isActive)
    {
        if (roof != null)
        {
            StartCoroutine(FadeObject(roofRenderer, isActive, true));
        }
        if (isActive)
        {
            HideWalls();

            for (int i = 0; i < areasToFadeWhenActive.Count; i++)
            {
                areasToFadeWhenActive[i].ToggleAreaFaded(true);
            }
        }
        else
        {
            ShowWalls();

            for (int i = 0; i < areasToFadeWhenActive.Count; i++)
            {
                areasToFadeWhenActive[i].ToggleAreaFaded(false);
            }
        }

        foreach (GameObject entity in interiorEntityTracker.Keys)
        {
            if (entity != null)
            {
                entity.SetActive(isActive);
            }
            else
            {
                entitiesToRemove.Add(entity);
            }
        }

        int numberOfEntitiesToRemove = entitiesToRemove.Count;
        if (numberOfEntitiesToRemove > 0)
        {
            for (int i = 0; i < numberOfEntitiesToRemove; i++)
            {
                interiorEntityTracker.Remove(entitiesToRemove[i]);
            }
        }
        entitiesToRemove.Clear();
    }

    void ToggleAreaFaded(bool isFading)
    {
        isAreaFaded = isFading;
        if (sectionsToFadeRenderers == null)
        {
            InitializeSectionsToFadeRenderers();
        }

        for (int i = 0; i < sectionsToFadeWhenAreaFaded.Count; i++)
        {
            StartCoroutine(FadeObject(sectionsToFadeRenderers[i], isFading));
        }

        for (int i = 0; i < sectionsToHideWhenAreaFaded.Count; i++)
        {
            sectionsToHideWhenAreaFaded[i].SetActive(!isFading);
        }
    }

    #region Entity entrance/exit handling
    public void RegisterEntityEnter(GameObject entity)
    {
        if (!interiorEntityTracker.ContainsKey(entity))
        {
            interiorEntityTracker[entity] = 0;
        }
        interiorEntityTracker[entity]++;
    }

    public void RegisterEntityExit(GameObject entity)
    {
        Debug.Log("registering exiting entity");
        interiorEntityTracker[entity]--;

        if (interiorEntityTracker[entity] <= 0)
        {
            interiorEntityTracker.Remove(entity);
        }
    }

    public void RegisterPlayerEnter()
    {
        currentPlayerTriggerCount++;

        if (currentPlayerTriggerCount == 1)
        {
            ToggleAreaActive(true);
        }
    }

    public void RegisterPlayerExit()
    {
        currentPlayerTriggerCount--;

        if (currentPlayerTriggerCount == 0)
        {
            ToggleAreaActive(false);
        }
    }
    #endregion

    #region Area manipulation 
    void HideWalls()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallRenderers.Count; i++)
        {
            //StartCoroutine(SlideWallDownward(i, roomTransitionTime, roomTransitionCurve));
            StartCoroutine(FadeObject(wallRenderers[i], true));
        }
    }

    void ShowWalls()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallRenderers.Count; i++)
        {
            //StartCoroutine(SlideWallUpward(i, roomTransitionTime, roomTransitionCurve));
            StartCoroutine(FadeObject(wallRenderers[i], false));
        }
    }
    #endregion

    IEnumerator FadeObject(MeshRenderer renderer, bool isFading, bool fadeCompletely = false)
    {
        Color originalColor = renderer.material.color;
        Color finalColor = originalColor;

        if (!isFading)
        {
            finalColor.a = 1f;
        }
        else if (isAreaFaded)
        {
            finalColor.a = areaFadedFadeValue;
        }
        else
        {
            if (fadeCompletely)
            {
                finalColor.a = 0f;
            }
            else
            {
                finalColor.a = playerInAreaFadeValue;
            }
        }

        float transitionTime = GameManager.RoomTransitionTime;
        AnimationCurve transitionCurve = GameManager.RoomTransitionCurve;

        float timeElapsed = 0.0f;

        while (timeElapsed < transitionTime)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / transitionTime;

            renderer.material.color = Color.Lerp(originalColor, finalColor, transitionCurve.Evaluate(percentageComplete));
            yield return null;
        }
    }

    IEnumerator SlideWallDownward(int wallIndex, float slideTime, AnimationCurve slideCurve)
    {
        Debug.Log("sliding wall downward");
        GameObject wall = fadableWallList[wallIndex];

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

    IEnumerator SlideWallUpward(int wallIndex, float slideTime, AnimationCurve slideCurve)
    {
        GameObject wall = fadableWallList[wallIndex];

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
