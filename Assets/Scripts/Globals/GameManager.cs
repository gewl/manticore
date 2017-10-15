using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {

    // State
    static bool isPaused = false;
    static IEnumerator freezeCoroutine;

    // Player references
    static GameObject playerObject;
    static Transform playerTransform;
    static MobileEntityHealthComponent playerHealthManager;

    #region lazyload references
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
    
    static GameManagerHelper _helper;
    static GameManagerHelper helper
    {
        get
        {
            if (_helper == null)
            {
                _helper = Component.FindObjectOfType<GameManagerHelper>();
            }

            return _helper;
        }
    }

    static GameObject _hud;
    static GameObject hud
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
                _cameraController = Camera.main.GetComponent<CameraController>();
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

    #region Gamestate handlers

    public static void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            hud.SetActive(false);
            MuteAllEmitters();
            Time.timeScale = 0f;
            cameraController.ApplyPauseFilter();
        }
        else
        {
            hud.SetActive(true);
            UnmuteAllEmitters();
            Time.timeScale = 1f;
            cameraController.RevertToOriginalProfile();
        }
    }

    public static void HandleFreezeEvent(GlobalConstants.GameFreezeEvent freezeEvent)
    {
        helper.FreezeFrame(freezeEvent);
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

    #region player data retrieval
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
    #endregion

    #region input data retrieval

    public static Vector3 GetMousePositionInWorldSpace()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100))
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
}
