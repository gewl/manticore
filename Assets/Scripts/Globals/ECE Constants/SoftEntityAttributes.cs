using System;

public enum SoftEntityAttributes
{
    CurrentHealth,
    CurrentMoveSpeed,
    IsAggroed,
    NextWaypoint,
    CurrentRotationSpeed,
    CurrentTarget,
}

public static class SoftEntityAttributeTypes
{
    public static Type GetType(SoftEntityAttributes attribute)
    {
        switch (attribute)
        {
            case SoftEntityAttributes.CurrentHealth:
                return typeof(float);
            case SoftEntityAttributes.CurrentMoveSpeed:
                return typeof(float);
            case SoftEntityAttributes.IsAggroed:
                return typeof(bool);
            case SoftEntityAttributes.NextWaypoint:
                return typeof(UnityEngine.Vector3);
            case SoftEntityAttributes.CurrentTarget:
                return typeof(UnityEngine.Transform);
            case SoftEntityAttributes.CurrentRotationSpeed:
                return typeof(float);
            default:
                return typeof(float);
        }
    }
}
