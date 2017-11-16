using System;
using UnityEngine;

public enum EntityAttributes
{
    CurrentHealth,
    BaseMoveSpeed,
    CurrentMoveSpeed,
    IsAggroed,
    NextWaypoint,
    CurrentRotationSpeed,
    CurrentTarget,
    CurrentTargetPosition,
    CurrentDirection
}

public static class EntityAttributeTypes
{
    public static Type GetType(EntityAttributes attribute)
    {
        switch (attribute)
        {
            case EntityAttributes.CurrentHealth:
                return typeof(float);
            case EntityAttributes.BaseMoveSpeed:
                return typeof(float);
            case EntityAttributes.CurrentMoveSpeed:
                return typeof(float);
            case EntityAttributes.IsAggroed:
                return typeof(bool);
            case EntityAttributes.NextWaypoint:
                return typeof(Vector3);
            case EntityAttributes.CurrentTarget:
                return typeof(Transform);
            case EntityAttributes.CurrentTargetPosition:
                return typeof(Vector3);
            case EntityAttributes.CurrentRotationSpeed:
                return typeof(float);
            case EntityAttributes.CurrentDirection:
                return typeof(Vector3);
            default:
                return typeof(float);
        }
    }
}
