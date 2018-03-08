using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2;

public class AreaController : MonoBehaviour {

    private const string FLOOR_PREFIX = "Floor";

    [Header("Area Sections")]
    [SerializeField]
    List<GameObject> sectionsToFadeWhenAreaActive;
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

    List<MeshRenderer> sectionsToFadeRenderers;
    List<MeshRenderer> fadableSectionRenderers;

    // This tracks number of times an entity has been registered on a given floor, to handle
    // the possibility of it setting off multiple triggers on one floor
    // without being added to the list twice, etc.
    Dictionary<int, Dictionary<GameObject, int>> floorsToEntitiesMap; 
    List<GameObject> entitiesToRemove;

    float currentPlayerTriggerCount;
    bool isAreaFaded = false;

    int currentlyActiveFloor = -1;
    int lastActiveFloor = -1;
    [SerializeField]
    int numberOfFloors = -1;

    List<GameObject> floorGroups;

    void Awake()
    {
        fadableSectionRenderers = new List<MeshRenderer>();
        entitiesToRemove = new List<GameObject>();
        floorGroups = new List<GameObject>();

        floorsToEntitiesMap = new Dictionary<int, Dictionary<GameObject, int>>();

        for (int i = 0; i < sectionsToFadeWhenAreaActive.Count; i++)
        {
            fadableSectionRenderers.Add(sectionsToFadeWhenAreaActive[i].GetComponent<MeshRenderer>());
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
        if (isActive)
        {
            FadeSections();

            for (int i = 0; i < areasToFadeWhenActive.Count; i++)
            {
                areasToFadeWhenActive[i].ToggleAreaFaded(true);
            }
        }
        else
        {
            ShowSections();

            for (int i = 0; i < areasToFadeWhenActive.Count; i++)
            {
                areasToFadeWhenActive[i].ToggleAreaFaded(false);
            }
        }
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
    public void RegisterEntityEnter(GameObject entity, int floor = 1)
    {
        if (!floorsToEntitiesMap.ContainsKey(floor))
        {
            floorsToEntitiesMap[floor] = new Dictionary<GameObject, int>();
        }

        Dictionary<GameObject, int> entitiesOnFloorTracker = floorsToEntitiesMap[floor];

        if (!entitiesOnFloorTracker.ContainsKey(entity))
        {
            entitiesOnFloorTracker[entity] = 0;
        }
        entitiesOnFloorTracker[entity]++;

        if (floor == currentlyActiveFloor || currentlyActiveFloor == -1)
        {
            entity.GetComponent<EntityManagement>().SetEntityVisibility(true);
        }
    }

    public void RegisterEntityExit(GameObject entity, int floor = 1)
    {
        if (!floorsToEntitiesMap.ContainsKey(floor))
        {
            floorsToEntitiesMap[floor] = new Dictionary<GameObject, int>();
        }

        Dictionary<GameObject, int> entitiesOnFloorTracker = floorsToEntitiesMap[floor];
        entitiesOnFloorTracker[entity]--;

        if (entitiesOnFloorTracker[entity] <= 0)
        {
            entitiesOnFloorTracker.Remove(entity);

            bool isEntityOnHigherFloor = false;

            for (int i = floor; i <= numberOfFloors; i++)
            {
                if (floorsToEntitiesMap[i].ContainsKey(entity))
                {
                    isEntityOnHigherFloor = true;
                    break;
                }
            }

            if (floor == currentlyActiveFloor && isEntityOnHigherFloor)
            {
                entity.GetComponent<EntityManagement>().SetEntityVisibility(false);
            }
        }
    }

    public void RegisterPlayerEnter(int floor)
    {
        currentPlayerTriggerCount++;

        lastActiveFloor = currentlyActiveFloor;
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
            lastActiveFloor = -1;
        }
        else if (currentlyActiveFloor == floor)
        {
            currentlyActiveFloor = lastActiveFloor;
            lastActiveFloor = floor;
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
            RevealAllFloorsUpToCurrentFloor(currentlyActiveFloor);
        }

        SetVisibilityForEntitiesByFloor(currentlyActiveFloor);
    }

    void RevealAllFloorsUpToCurrentFloor(int currentFloor)
    {
        for (int i = 1; i <= currentFloor; i++)
        {
            int floorIndex = i - 1;
            floorGroups[floorIndex].SetActive(true);
        }
        for (int i = currentFloor + 1; i <= numberOfFloors; i++)
        {
            int floorIndex = i - 1;
            floorGroups[floorIndex].SetActive(false);
        }
    }

    void SetVisibilityForEntitiesByFloor(int currentFloor)
    {
        foreach (int floor in floorsToEntitiesMap.Keys)
        {
            Dictionary<GameObject, int> entitiesOnFloorTracker = floorsToEntitiesMap[floor];

            bool areEntitiesVisible = floor <= currentFloor || currentFloor == -1; 

            foreach (GameObject entity in entitiesOnFloorTracker.Keys)
            {
                entity.GetComponent<EntityManagement>().SetEntityVisibility(areEntitiesVisible);
            }
        }
    }

    void FadeSections()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < fadableSectionRenderers.Count; i++)
        {
            StartCoroutine(FadeObject(fadableSectionRenderers[i], true));
        }
    }

    void ShowSections()
    {
        float roomTransitionTime = GameManager.RoomTransitionTime;
        AnimationCurve roomTransitionCurve = GameManager.RoomTransitionCurve;
        for (int i = 0; i < fadableSectionRenderers.Count; i++)
        {
            StartCoroutine(FadeObject(fadableSectionRenderers[i], false));
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
