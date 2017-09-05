using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

public class EntityEmitter : MonoBehaviour {

    Dictionary<string, List<UnityAction>> eventSubscriptions;

    private void Awake()
    {
        eventSubscriptions = new Dictionary<string, List<UnityAction>>();
    }

    private void Update()
    {
        EmitEvent(EntityEvents.Update);
    }

    public void SubscribeToEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            eventSubscriptions[entityEvent] = new List<UnityAction>();
        }
        eventSubscriptions[entityEvent].Add(listener);
    }

    public void UnsubscribeFromEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].Remove(listener);
    }

    public void EmitEvent(string entityEvent)
    {
        Debug.Log("emitting event:");
        Debug.Log(entityEvent);
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].ForEach(action => {
            action.Invoke();
        });
    }
}
