using System;

[Serializable]
public class CubeData : EntityData, IRangedCombatAIData, IMobileEntityData, IAggroData, IStandardFirerData
{
    // EntityData overrides
    protected override string displayName { get { return "Cube"; } }
    protected override int initialHealth { get { return 90; } }
    protected override int momentumValue { get { return 5; } }

    // IRangedCombatAI implementations
    public int ArcOfFire { get { return 25; } }
    public int AttackRange { get { return 30; } }
    public float FireCooldown { get { return 0.6f; } }

    // IAggroData implementations
    public float AggroRange { get { return 15; } }

    // IMobileEntityData implementations
    public float BaseMoveSpeed { get { return 10f; } }

    // IStandardFirer implementations
    public int ProjectileStrength { get { return 50; } }
    public float BulletSpeed { get { return 30f; } }
    public float AimNoiseInDegrees { get { return 2f; } }
}


