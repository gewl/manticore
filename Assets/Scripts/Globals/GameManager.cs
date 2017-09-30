using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global GameManager exposes information about gamestate & references to GameObjects to prevent expensive Find calls.
/// </summary>
public static class GameManager {

    static GameObject playerObject;
    static Transform playerTransform;
    static MobileEntityHealthComponent playerHealthManager;

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
}
