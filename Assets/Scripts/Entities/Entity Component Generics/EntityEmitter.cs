using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

/// <summary>
/// EntityEmitter is a generic event system for communicating to/between an entity's components.
/// </summary>
public class EntityEmitter : MonoBehaviour {

    Dictionary<string, List<UnityAction>> eventSubscriptions;

    [HideInInspector]
    public bool isMuted = false;
    bool isStunned = false;

    public void SetStunned()
    {
        EmitEvent(EntityEvents.Stun);
        isStunned = true;
    }

    public void SetUnstunned()
    {
        isStunned = false;
        EmitEvent(EntityEvents.Unstun);
    }

    private void Awake()
    {
        eventSubscriptions = new Dictionary<string, List<UnityAction>>();
    }

    private void OnEnable()
    {
        GameManager.RegisterEmitter(this);
    }

    private void OnDisable()
    {
        GameManager.DeregisterEmitter(this);
    }

    private void Update()
    {
        EmitEvent(EntityEvents.Update);
    }

    private void FixedUpdate()
    {
        EmitEvent(EntityEvents.FixedUpdate);
    }

    private void LateUpdate()
    {
        EmitEvent(EntityEvents.LateUpdate);
    }

    public void SubscribeToEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            eventSubscriptions[entityEvent] = new List<UnityAction>();
        }
        if (eventSubscriptions[entityEvent].Contains(listener))
        {
            Debug.LogError(entityEvent + " already contains " + listener);
            return;
        }
        eventSubscriptions[entityEvent].Add(listener);
    }

    public void UnsubscribeFromEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            Debug.LogError(entityEvent + " does not contain " + listener);
            return;
        }
        eventSubscriptions[entityEvent].Remove(listener);
    }

    public void EmitEvent(string entityEvent)
    {
        // Adding this conditional logic feels like a slippery slope, but entities *need*
        // to be reacting to damage/death even when they're stunned.
        bool isReactingToDamage = entityEvent == EntityEvents.Hurt || entityEvent == EntityEvents.Dead;
        if (!isReactingToDamage && (isMuted || isStunned) || !eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].ForEach(action => {
            action.Invoke();
        });
    }
}
