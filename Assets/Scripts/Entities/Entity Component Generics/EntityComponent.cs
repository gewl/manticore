using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Parent component class that all EntityComponents derive from. 
/// </summary>
[RequireComponent(typeof(EntityEmitter), typeof(EntityManagement), typeof(EntityStatHandler))]
[RequireComponent(typeof(EntityModifierHandler))]
public abstract class EntityComponent : SerializedMonoBehaviour {

    EntityEmitter _entityEmitter;
    protected EntityEmitter entityEmitter
    {
        get
        {
            if (_entityEmitter == null)
            {
                _entityEmitter = GetComponent<EntityEmitter>();
            }

            return _entityEmitter;
        }
    }

    private EntityManagement _entityInformation;
    protected EntityManagement entityInformation
    {
        get
        {
            if (_entityInformation == null)
            {
                _entityInformation = GetComponent<EntityManagement>();
            }

            return _entityInformation;
        }
    }

    private EntityStatHandler _entityStats;
    protected EntityStatHandler entityStats
    {
        get
        {
            if (_entityStats == null)
            {
                _entityStats = GetComponent<EntityStatHandler>();
            }

            return _entityStats;
        }

    }

    private EntityModifierHandler _entityModifierHandler;
    protected EntityModifierHandler entityModifierHandler
    {
        get
        {
            if (_entityModifierHandler == null)
            {
                _entityModifierHandler = GetComponent<EntityModifierHandler>();
            }

            return _entityModifierHandler;
        }
    }

    protected virtual void OnEnable()
    {
        Subscribe();
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }

    protected virtual void Awake()
    {
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
