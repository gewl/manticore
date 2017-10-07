using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {

    // State
    static bool isPaused = false;

    // Player references
    static GameObject playerObject;
    static Transform playerTransform;
    static MobileEntityHealthComponent playerHealthManager;

    #region lazyload references
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


    static CameraManager _cameraManager;
    static CameraManager cameraManager
    {
        get
        {
            if (_cameraManager == null)
            {
                _cameraManager = Camera.main.GetComponent<CameraManager>();
            }

            return _cameraManager;
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
            cameraManager.ApplyPauseFilter();
        }
        else
        {
            hud.SetActive(true);
            UnmuteAllEmitters();
            Time.timeScale = 1f;
            cameraManager.RevertToOriginalProfile();
        }
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

    static void MuteAllEmitters()
    {
        for (int i = 0; i < activeEmittersInScene.Count; i++)
        {
            activeEmittersInScene[i].isMuted = true;
        }
    }

    static void UnmuteAllEmitters()
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
}
