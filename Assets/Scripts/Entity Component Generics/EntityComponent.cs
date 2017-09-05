using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityEmitter), typeof(EntityData))]
public abstract class EntityComponent : MonoBehaviour {

    protected EntityEmitter entityEmitter;
    protected EntityData entityData;

    protected virtual void Awake()
    {
        entityEmitter = GetComponent<EntityEmitter>();
        entityData = GetComponent<EntityData>();
    }

    public abstract void Initialize();
    public abstract void Cleanup();
}
