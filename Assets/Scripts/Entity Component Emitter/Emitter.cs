using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

public class Emitter : MonoBehaviour {

    Dictionary<EntityEvent, List<UnityAction>> eventSubscriptions;

	void Start () {
		
	}
	
	void Update () {
	}

    void SubscribeToEvent(EntityEvent entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            eventSubscriptions[entityEvent] = new List<UnityAction>();
        }
        eventSubscriptions[entityEvent].Add(listener);
    }

    void UnsubscribeFromEvent(EntityEvent entityEvent, UnityAction listener)
    {
        if (!eventSubscriptions.ContainsKey(entityEvent))
        {
            return;
        }
        eventSubscriptions[entityEvent].Remove(listener);
    }

    void EmitEvent(EntityEvent entityEvent)
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
