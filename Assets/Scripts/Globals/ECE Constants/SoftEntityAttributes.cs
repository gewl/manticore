using System;
using UnityEngine;

public enum SoftEntityAttributes
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

public static class SoftEntityAttributeTypes
{
    public static Type GetType(SoftEntityAttributes attribute)
    {
        switch (attribute)
        {
            case SoftEntityAttributes.CurrentHealth:
                return typeof(float);
            case SoftEntityAttributes.BaseMoveSpeed:
                return typeof(float);
            case SoftEntityAttributes.CurrentMoveSpeed:
                return typeof(float);
            case SoftEntityAttributes.IsAggroed:
                return typeof(bool);
            case SoftEntityAttributes.NextWaypoint:
                return typeof(Vector3);
            case SoftEntityAttributes.CurrentTarget:
                return typeof(Transform);
            case SoftEntityAttributes.CurrentTargetPosition:
                return typeof(Vector3);
            case SoftEntityAttributes.CurrentRotationSpeed:
                return typeof(float);
            case SoftEntityAttributes.CurrentDirection:
                return typeof(Vector3);
            default:
                return typeof(float);
        }
    }
}
