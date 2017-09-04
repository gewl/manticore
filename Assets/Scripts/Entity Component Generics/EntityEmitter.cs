using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

public class EntityEmitter : MonoBehaviour {

    Dictionary<string, List<UnityAction>> eventSubscriptions;

    void SubscribeToEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            eventSubscriptions[entityEvent] = new List<UnityAction>();
        }
        eventSubscriptions[entityEvent].Add(listener);
    }

    void UnsubscribeFromEvent(string entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].Remove(listener);
    }

    void EmitEvent(string entityEvent)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].ForEach(action => {
            action.Invoke();
        });
    }
}
