using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2;

public class AreaController : MonoBehaviour {

    private const string FLOOR_PREFIX = "Floor";

    Material areaMaterial;

    [Header("Area Sections")]
    [SerializeField]
    GameObject interior;

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

    Vector4 baseXZPlanePosition;
    const string xzPlaneTag = "_Plane1Position";
    [SerializeField]
    float revealedXZPlaneHeight;
    [SerializeField]
    float hiddenXZPlaneHeight;

    // This tracks number of times an entity has been registered on a given floor, to handle
    // the possibility of it setting off multiple triggers on one floor
    // without being added to the list twice, etc.
    Dictionary<int, Dictionary<GameObject, int>> floorsToEntitiesMap; 

    float currentPlayerTriggerCount;
    bool isAreaFaded = false;

    int currentlyActiveFloor = -1;
    int lastActiveFloor = -1;
    [SerializeField]
    int numberOfFloors = -1;

    List<GameObject> floorGroups;

    void Awake()
    {
        areaMaterial = GetComponent<Renderer>().material;
        baseXZPlanePosition = areaMaterial.GetVector(xzPlaneTag);

        fadableSectionRenderers = new List<MeshRenderer>();
        floorGroups = new List<GameObject>();

        floorsToEntitiesMap = new Dictionary<int, Dictionary<GameObject, int>>();

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

    void InitializeClippingPlanes()
    {
        areaMaterial.SetVector("_Plane1Normal", Vector4.zero);
        areaMaterial.SetVector("_Plane1Position", Vector4.zero);

        areaMaterial.SetVector("_Plane2Normal", Vector4.zero);
        areaMaterial.SetVector("_Plane2Position", Vector4.zero);

        areaMaterial.SetVector("_Plane3Normal", Vector4.zero);
        areaMaterial.SetVector("_Plane3Position", Vector4.zero);
    }

    void ToggleAreaActive(bool isActive)
    {
        if (isActive)
        {
            StartCoroutine(ToggleStructureVisibility(true));
        }
        else { 
            StartCoroutine(ToggleStructureVisibility(false));
        }
    }

    IEnumerator ToggleStructureVisibility(bool settingVisible)
    {
        float transitionTime = GameManager.RoomTransitionTime;

        float timeElapsed = 0.0f;

        Vector4 initialPosition = baseXZPlanePosition;
        Vector4 destinationPosition = baseXZPlanePosition;

        if (settingVisible)
        {
            initialPosition.y = hiddenXZPlaneHeight;
            destinationPosition.y = revealedXZPlaneHeight;
        } 
        else
        {
            initialPosition.y = revealedXZPlaneHeight;
            destinationPosition.y = hiddenXZPlaneHeight;
        }

        while (timeElapsed < transitionTime) 
        {
            float percentageComplete = timeElapsed / transitionTime;
            float curvedPercentageCompletion = GameManager.RoomTransitionCurve.Evaluate(percentageComplete);

            Vector4 newPlanePosition = Vector4.Lerp(initialPosition, destinationPosition, curvedPercentageCompletion);
            areaMaterial.SetVector(xzPlaneTag, newPlanePosition);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        areaMaterial.SetVector(xzPlaneTag, destinationPosition);
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
    #endregion

}
