using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global GameManager exposes information about gamestate & references to GameObjects to prevent expensive Find calls.
/// </summary>
public static class GameManager {

    static GameObject playerObject;
    static Transform playerTransform;

    public static GameObject GetPlayerObject()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        return playerObject;
    }

    public static Transform GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        return playerTransform;
    }

    public static Vector3 GetPlayerPosition()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        return playerTransform.position;
    }
}
