using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent component class that all EntityComponents derive from. 
/// </summary>
[RequireComponent(typeof(EntityEmitter), typeof(EntityData))]
public abstract class EntityComponent : MonoBehaviour {

    protected EntityEmitter entityEmitter;
    protected EntityData entityData;

    protected void OnEnable()
    {
        Subscribe();
    }

    protected void OnDisable()
    {
        Unsubscribe();
    }

    protected virtual void Awake()
    {
        entityEmitter = GetComponent<EntityEmitter>();
        entityData = GetComponent<EntityData>();
    }

    /// <summary>
    /// Subscribes component to the events it requires, sets up anything else required for functionality.
    /// </summary>
    protected abstract void Subscribe();

    /// <summary>
    /// Unsubscribes component from events in preparation to be disabled/removed.
    /// </summary>
    protected abstract void Unsubscribe();
}
