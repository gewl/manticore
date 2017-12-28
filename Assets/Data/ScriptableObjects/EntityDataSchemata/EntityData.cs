using System;
using UnityEngine;

[Serializable]
public abstract class EntityData : ScriptableObject {
    protected abstract string displayName { get; }
    public virtual string Name { get { return displayName; } }

    protected abstract int initialHealth { get; }
    public virtual int InitialHealth { get { return initialHealth; } }

    protected abstract int momentumValue { get; }
    public virtual int MomentumValue { get { return momentumValue; } }
}
