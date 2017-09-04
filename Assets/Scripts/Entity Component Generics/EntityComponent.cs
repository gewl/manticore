using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityEmitter))]
public abstract class EntityComponent : MonoBehaviour {

    private EntityEmitter emitter;

    private void Awake()
    {
        emitter = GetComponent<EntityEmitter>();
    }

    public virtual void Initialize() { }
}
