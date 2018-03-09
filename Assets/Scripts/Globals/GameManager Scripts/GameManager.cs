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
    public enum GameStates
    {
        InPlay,
        InMenu
    }
    public static GameStates CurrentGameState = GameStates.InPlay;

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

    // Environment references
    static LevelManager currentlyActiveLevel;

    // Player references
    static GameObject playerObject;
    static Transform playerTransform;
    static MobileEntityHealthComponent playerHealthManager;
    Plane playerPlane;

    [SerializeField]
    GameObject manticorePrefab;
    [SerializeField]
    float playerDeathFadeScreenTimer;

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
        // Update plane at player's y-position for raycasting mouseclicks
        playerPlane = new Plane(Vector3.up, GetPlayerPosition());

        if (CurrentGameState == GameStates.InPlay && Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }		
    }

    #region lazyload references
    static Transform _terrain;
    static Transform Terrain
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
    static Camera MainCamera
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

    static Image _fadeScreen;
    static Image FadeScreen
    {
        get
        {
            if (_fadeScreen == null)
            {
                _fadeScreen = GameObject.Find("FadeScreen").GetComponent<Image>();
            }

            return _fadeScreen;
        }
    }

    static List<EntityEmitter> _activeEmittersInScene;
    static List<EntityEmitter> ActiveEmittersInScene
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
    static CameraController CameraController
    {
        get
        {
            if (_cameraController == null)
            {
                _cameraController = MainCamera.GetComponent<CameraController>();
            }

            return _cameraController;
        }
    }

    private static GameObject _player;
    private static GameObject Player
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

    private static MomentumManager _momentumManager;
    public static MomentumManager MomentumManager
    {
        get
        {
            if (_momentumManager == null)
            {
                _momentumManager = Player.GetComponent<MomentumManager>();
            }

            return _momentumManager;
        }
    }

    const string BULLETS_PARENT_ID = "Bullets";
    private static GameObject _bulletsParent;
    public static GameObject BulletsParent
    {
        get
        {
            if (_bulletsParent == null)
            {
                _bulletsParent = GameObject.Find(BULLETS_PARENT_ID);
            }

            return _bulletsParent;
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

    public static void HandlePlayerDeath()
    {
        instance.StartCoroutine(instance.PlayerDeathSequence());
    }

    IEnumerator PlayerDeathSequence()
    {
        float fadeOutCompleteTime = Time.time + playerDeathFadeScreenTimer;

        while (Time.time < fadeOutCompleteTime)
        {
            float timeRemaining = fadeOutCompleteTime - Time.time;

            float percentageComplete = timeRemaining / playerDeathFadeScreenTimer;

            Color fadeColor = FadeScreen.color;

            fadeColor.a = Mathf.Lerp(1f, 0f, percentageComplete);

            FadeScreen.color = fadeColor;
            yield return null;
        }

        MuteAllEmitters();

        GlobalEventEmitter.OnGameStateEvent(GlobalConstants.GameStateEvents.PlayerDied);
        EntityEmitter playerEntityEmitter = Player.GetComponent<EntityEmitter>();
        playerEntityEmitter.isMuted = false;
        playerEntityEmitter.EmitEvent(EntityEvents.Respawning);

        UnmuteAllEmitters();

        playerTransform.position = currentlyActiveLevel.SpawnPoint.position;

        yield return new WaitForSeconds(3f);

        float fadeBackTimer = playerDeathFadeScreenTimer / 2f;
        float fadeBackCompleteTime = Time.time + fadeBackTimer;

        while (Time.time < fadeBackCompleteTime)
        {
            float timeRemaining = fadeBackCompleteTime - Time.time;

            float percentageComplete = timeRemaining / fadeBackTimer;

            Color fadeColor = FadeScreen.color;

            fadeColor.a = Mathf.Lerp(0f, 1f, percentageComplete);

            FadeScreen.color = fadeColor;
            yield return null;
        }

        MomentumManager.RemoveLastMomentumPoint();
    }

    public static void FreezeGame(GlobalConstants.GameFreezeEvent freezeEvent)
    {
        int freezeFrameCount = instance.gameFreezeTimers[freezeEvent];
        IEnumerator freezeCoroutine = CoroutineUtilities.PauseForFrames(freezeFrameCount);

        instance.StartCoroutine(freezeCoroutine);
    }

    public static void ShakeScreen(float duration = 0.5f)
    {
        CameraController.ShakeScreen(duration);
    }

    public static void JoltScreen(Vector3 direction)
    {
        CameraController.JoltScreen(direction);
    }

    public static void EnterMenu()
    {
        TogglePause();

        CurrentGameState = GameStates.InMenu;
    }

    public static void ExitMenu()
    {
        TogglePause();

        CurrentGameState = GameStates.InPlay;
    }

    public static void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            HUD.SetActive(false);
            MuteAllEmitters();
            Time.timeScale = 0f;
            CameraController.ApplyPauseFilter();
        }
        else
        {
            HUD.SetActive(true);
            UnmuteAllEmitters();
            Time.timeScale = 1f;
            CameraController.RevertToOriginalProfile();
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
        ActiveEmittersInScene.Add(emitter);
    }

    public static void DeregisterEmitter(EntityEmitter emitter)
    {
        ActiveEmittersInScene.Remove(emitter);
    }

    public static void MuteAllEmitters()
    {
        for (int i = 0; i < ActiveEmittersInScene.Count; i++)
        {
            ActiveEmittersInScene[i].isMuted = true;
        }
    }

    public static void UnmuteAllEmitters()
    {
        for (int i = 0; i < ActiveEmittersInScene.Count; i++)
        {
            ActiveEmittersInScene[i].isMuted = false;
        }
    }

    #endregion

    #region manticore-specific functionality
    public static GameObject GetPlayerObject()
    {
        return Player;
    }

    public static Transform GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = Player.transform;
        }
        return playerTransform;
    }

    public static Vector3 GetPlayerPosition()
    {
        if (playerTransform == null)
        {
            playerTransform = Player.transform;
        }
        return playerTransform.position;
    }

    public static float GetPlayerInitialHealth()
    {
        if (playerHealthManager == null)
        {
            playerHealthManager = Player.GetComponent<MobileEntityHealthComponent>();
        }

        return playerHealthManager.InitialHealth();
    }

    public static float GetPlayerCurrentHealth()
    {
        if (playerHealthManager == null)
        {
            playerHealthManager = Player.GetComponent<MobileEntityHealthComponent>();
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

        for (int i = 0; i < Terrain.childCount; i++)
        {
            Transform obstacle = Terrain.GetChild(i);
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
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
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
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 1000))
        {
            return hit.point;
        }
        else
        {
            Debug.LogWarning("Mouse position not found.");
            return MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    #endregion

    #region HUD data retrieval
    [SerializeField]
    AnimationCurve belovedSwingCurve;
    public static AnimationCurve BelovedSwingCurve { get { return instance.belovedSwingCurve; } }

    [SerializeField]
    AbilityBarController abilityBar;
    public AbilityBarController AbilityBar { get { return abilityBar; } }

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

    public static void RegisterCurrentLevel(LevelManager currentLevel)
    {
        currentlyActiveLevel = currentLevel;
    }

    [SerializeField]
    float roomTransitionTime;
    public static float RoomTransitionTime { get { return instance.roomTransitionTime; } }

    [SerializeField]
    AnimationCurve roomTransitionCurve;
    public static AnimationCurve RoomTransitionCurve { get { return instance.roomTransitionCurve; } }

    #endregion

    // TODO: Between this and the environment management section above, this class is beginning to carry a lot of data
    // for other classes that don't have access to inspector serialization. Should this data be moved to a specific class?
    #region animation curves for active abilities

    [SerializeField]
    AnimationCurve parrySwingCurve;
    public static AnimationCurve ParrySwingCurve { get { return instance.parrySwingCurve; } }

    [SerializeField]
    AnimationCurve nullifyEffectCurve;
    public static AnimationCurve NullifyEffectCurve { get { return instance.nullifyEffectCurve; } }

    [SerializeField]
    AnimationCurve blinkCompletionCurve;
    public static AnimationCurve BlinkCompletionCurve { get { return instance.blinkCompletionCurve; } }
    #endregion
}
