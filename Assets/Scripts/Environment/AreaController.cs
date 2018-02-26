using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2;

public class AreaController : MonoBehaviour {

    private const string FLOOR_PREFIX = "Floor";

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

    Dictionary<GameObject, int> interiorEntityTracker;
    List<GameObject> entitiesToRemove;

    float currentPlayerTriggerCount;
    bool isAreaFaded = false;

    int currentlyActiveFloor = -1;
    int lastFloor = -1;
    [SerializeField]
    int numberOfFloors = -1;

    List<GameObject> floorGroups;

    void Awake()
    {
        if (roof != null)
        {
            roofRenderer = roof.GetComponent<MeshRenderer>();
        }

        wallRenderers = new List<MeshRenderer>();
        interiorEntityTracker = new Dictionary<GameObject, int>();
        entitiesToRemove = new List<GameObject>();
        floorGroups = new List<GameObject>();

        for (int i = 0; i < fadableWallList.Count; i++)
        {
            wallRenderers.Add(fadableWallList[i].GetComponent<MeshRenderer>());
        }

        // TODO: Remove
        if (numberOfFloors < 0)
        {
            Debug.LogError("Set number of floors!");
        }

        if (numberOfFloors > 1)
        {
            for (int i = 1; i <= numberOfFloors; i++)
            {
                floorGroups.Add(transform.Find(FLOOR_PREFIX + i).gameObject);
            }
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
        interiorEntityTracker[entity]--;

        if (interiorEntityTracker[entity] <= 0)
        {
            interiorEntityTracker.Remove(entity);
        }
    }

    public void RegisterPlayerEnter(int floor)
    {
        currentPlayerTriggerCount++;

        lastFloor = currentlyActiveFloor;
        currentlyActiveFloor = floor;

        if (currentPlayerTriggerCount == 1)
        {
            ToggleAreaActive(true);
        }

        UpdateFloorVisibility();
    }

    public void RegisterPlayerExit(int floor)
    {
        currentPlayerTriggerCount--;

        if (currentPlayerTriggerCount == 0)
        {
            ToggleAreaActive(false);
            currentlyActiveFloor = -1;
            lastFloor = -1;
        }
        else if (currentlyActiveFloor == floor)
        {
            currentlyActiveFloor = lastFloor;
            lastFloor = floor;
        }

        UpdateFloorVisibility();
    }
    #endregion

    #region Area manipulation 
    void UpdateFloorVisibility()
    {
        if (numberOfFloors < 2)
        {
            return; 
        }

        if (currentlyActiveFloor == -1)
        {
            for (int i = 0; i < floorGroups.Count; i++)
            {
                floorGroups[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 1; i <= currentlyActiveFloor; i++)
            {
                int floorIndex = i - 1;
                floorGroups[floorIndex].SetActive(true);
            }
            for (int i = currentlyActiveFloor + 1; i <= numberOfFloors; i++)
            {
                int floorIndex = i - 1;
                floorGroups[floorIndex].SetActive(false);
            }
        }
    }

    void HideWalls()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallRenderers.Count; i++)
        {
            StartCoroutine(FadeObject(wallRenderers[i], true));
        }
    }

    void ShowWalls()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < wallRenderers.Count; i++)
        {
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

}
