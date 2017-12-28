using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityData/RangedEntityData")]
public class RangedEntityData : EntityData {
    public float ArcOfFire = 25f;
    public int AttackRange;
    public float FireCooldown;

    public int ProjectileStrength = 50;
    public float BulletSpeed = 30f;
    public float AimNoiseInDegrees = 2f;
    public Transform Projectile;
    public int NumberOfProjectiles = 1;
}
