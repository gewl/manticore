using System;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityData/BasicEntityData")]
public class EntityData : ScriptableObject {
    public string ID = "Entity";
    public int Health;
    public int MomentumValue;

    public float BaseMoveSpeed = 10f;
    public float AggroRange = 15f;

    public GlobalConstants.EntityAllegiance Allegiance = GlobalConstants.EntityAllegiance.Enemy;
    public bool isPlayer = false;
}
