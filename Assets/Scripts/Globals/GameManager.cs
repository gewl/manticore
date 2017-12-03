using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class GameManager : SerializedMonoBehaviour {

    static GameManager instance;

    // State
    static bool isPaused = false;
    public static bool[] collectibleTracker;

    static bool isPlayerLowHealth = false;
    public static bool IsPlayerLowHealth
    {
        get
        {
            return isPlayerLowHealth;
        }
    }

    // Player references
    static GameObject playerObject;
    static Transform playerTransform;
    static MobileEntityHealthComponent playerHealthManager;
    Plane playerPlane;

    [SerializeField]
    float entityFallSpeed = 30f;
    static public float GetEntityFallSpeed { get { return instance.entityFallSpeed; } }
    [OdinSerialize]
    Dictionary<GlobalConstants.GameFreezeEvent, int> gameFreezeTimers;
    [OdinSerialize]
    Dictionary<GlobalConstants.Collectibles, GameObject> collectiblePrefabs;

    void Awake()
    {
        instance = this;
        collectibleTracker = new bool[Enum.GetNames(typeof(GlobalConstants.Collectibles)).Length];
    }

    void Update()
    {
        playerPlane = new Plane(Vector3.up, GetPlayerPosition());
    }

    #region lazyload references
    static Transform _terrain;
    static Transform terrain
    {
        get
        {
            if (_terrain == null)
            {
                _terrain = GameObject.Find("Terrain").transform;
            }

            return _terrain;
        }
    }

    static Camera _mainCamera;
    static Camera mainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            return _mainCamera;
        }
    }
    
    static GameObject _hud;
    public static GameObject HUD
    {
        get
        {
            if (_hud == null)
            {
                _hud = GameObject.FindGameObjectWithTag("HUD");
            }

            return _hud;
        }
    }

    static List<EntityEmitter> _activeEmittersInScene;
    static List<EntityEmitter> activeEmittersInScene
    {
        get
        {
            if (_activeEmittersInScene == null)
            {
                _activeEmittersInScene = new List<EntityEmitter>();
            }

            return _activeEmittersInScene;
        }
    }


    static CameraController _cameraController;
    static CameraController cameraController
    {
        get
        {
            if (_cameraController == null)
            {
                _cameraController = mainCamera.GetComponent<CameraController>();
            }

            return _cameraController;
        }
    }

    private static GameObject _player;
    private static GameObject player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }

            return _player;
        }
    }
    #endregion

    #region Collectible management

    public static bool TryToRegisterCollectible(GlobalConstants.Collectibles collectible)
    {
        int collectibleInt = (int)collectible;

        if (collectibleTracker[collectibleInt])
        {
            return false;
        }
        else
        {
            collectibleTracker[collectibleInt] = true;
            return true;
        }
    }

    public static void DeregisterCollectible(GlobalConstants.Collectibles collectible)
    {
        int collectibleInt = (int)collectible;
        collectibleTracker[collectibleInt] = false;
    }

    public static GameObject RetrieveCollectiblePrefab(GlobalConstants.Collectibles collectible)
    {
        return instance.collectiblePrefabs[collectible];
    }

    #endregion

    #region gamestate handlers

    public static void HandleFreezeEvent(GlobalConstants.GameFreezeEvent freezeEvent)
    {
        int freezeFrameCount = instance.gameFreezeTimers[freezeEvent];
        IEnumerator freezeCoroutine = CoroutineUtilities.PauseForFrames(freezeFrameCount);

        instance.StartCoroutine(freezeCoroutine);
    }

    public static void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            HUD.SetActive(false);
            MuteAllEmitters();
            Time.timeScale = 0f;
            cameraController.ApplyPauseFilter();
        }
        else
        {
            HUD.SetActive(true);
            UnmuteAllEmitters();
            Time.timeScale = 1f;
            cameraController.RevertToOriginalProfile();
        }
    }

    #endregion

    #region gamestate data flags

    public static void SetIsPlayerLowHealth(bool _isPlayerLowHealth)
    {
        isPlayerLowHealth = _isPlayerLowHealth;
    }

    #endregion

    #region en-masse entity manipulation

    public static void RegisterEmitter(EntityEmitter emitter)
    {
        activeEmittersInScene.Add(emitter);
    }

    public static void DeregisterEmitter(EntityEmitter emitter)
    {
        activeEmittersInScene.Remove(emitter);
    }

    public static void MuteAllEmitters()
    {
        for (int i = 0; i < activeEmittersInScene.Count; i++)
        {
            activeEmittersInScene[i].isMuted = true;
        }
    }

    public static void UnmuteAllEmitters()
    {
        for (int i = 0; i < activeEmittersInScene.Count; i++)
        {
            activeEmittersInScene[i].isMuted = false;
        }
    }

    #endregion

    #region entity data retrieval
    public static GameObject GetPlayerObject()
    {
        return player;
    }

    public static Transform GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = player.transform;
        }
        return playerTransform;
    }

    public static Vector3 GetPlayerPosition()
    {
        if (playerTransform == null)
        {
            playerTransform = player.transform;
        }
        return playerTransform.position;
    }

    public static float GetPlayerInitialHealth()
    {
        if (playerHealthManager == null)
        {
            playerHealthManager = player.GetComponent<MobileEntityHealthComponent>();
        }

        return playerHealthManager.InitialHealth();
    }

    public static float GetPlayerCurrentHealth()
    {
        if (playerHealthManager == null)
        {
            playerHealthManager = player.GetComponent<MobileEntityHealthComponent>();
        }

        return playerHealthManager.CurrentHealth();
    }

    public static Transform GetHidingSpot(Transform agent, Transform target)
    {
        Vector3 agentPosition = agent.position;
        Vector3 targetPosition = target.position;

        Vector3 positionNearTarget = ((targetPosition * 2f) + agentPosition) / 3f;

        float distanceFromObstacleToTarget = float.MaxValue;
        Transform nearestObstacle = agent;

        for (int i = 0; i < terrain.childCount; i++)
        {
            Transform obstacle = terrain.GetChild(i);
            float sqrDistanceToPosition = (obstacle.position - positionNearTarget).sqrMagnitude;

            if (sqrDistanceToPosition < distanceFromObstacleToTarget)
            {
                nearestObstacle = obstacle;
                distanceFromObstacleToTarget = sqrDistanceToPosition;
            }
        }

        return nearestObstacle;
    }
    #endregion

    #region input data retrieval

    public static Vector3 GetMousePositionOnPlayerPlane()
    {
        if (playerTransform == null)
        {
            return Vector3.zero;     
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        Vector3 relativeMousePosition;
        if (instance.playerPlane.Raycast(ray, out distance))
        {
            relativeMousePosition = ray.GetPoint(distance);
        }
        else
        {
            relativeMousePosition = Vector3.zero;
            relativeMousePosition.y = playerTransform.position.y;
        }

        return relativeMousePosition;
    }

    public static Vector3 GetMousePositionInWorldSpace()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 1000))
        {
            return hit.point;
        }
        else
        {
            Debug.LogWarning("Mouse position not found.");
            return mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    #endregion

    #region HUD data retrieval

    [SerializeField]
    GameObject gearRangeArrow;
    GearRangeIndicator gearRangeIndicator;

    public static GearRangeIndicator ActivateAndGetGearRangeIndicator()
    {
        if (instance.gearRangeIndicator == null) 
        {
            instance.gearRangeIndicator = instance.gearRangeArrow.GetComponent<GearRangeIndicator>();
        }
        instance.gearRangeArrow.SetActive(true);

        return instance.gearRangeIndicator;
    }        

    public static void DeactivateGearRangeIndicator()
    {
        instance.gearRangeArrow.SetActive(false);
    }
    #endregion

    #region environment management 

    [SerializeField]
    float roomTransitionTime;
    public static float RoomTransitionTime { get { return instance.roomTransitionTime; } }

    [SerializeField]
    AnimationCurve roomTransitionCurve;
    public static AnimationCurve RoomTransitionCurve { get { return instance.roomTransitionCurve; } }

    #endregion

    // TODO: Between this and the environment management section above, this class is beginning to carry a lot of data
    // for other classes that don't have access to inspector serialization. Should this data be moved to a specific class?
    #region inventory management

    [SerializeField]
    AnimationCurve nullifyEffectCurve;
    public static AnimationCurve NullifyEffectCurve { get { return instance.nullifyEffectCurve; } }

    [SerializeField]
    AnimationCurve blinkCompletionCurve;
    public static AnimationCurve BlinkCompletionCurve { get { return instance.blinkCompletionCurve; } }
    #endregion
}
