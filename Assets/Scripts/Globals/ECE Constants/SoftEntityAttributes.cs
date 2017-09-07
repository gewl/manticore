using System;

public enum SoftEntityAttributes
{
    CurrentHealth,
    CurrentMoveSpeed,
    IsFriendly,
    NextWaypoint,
    CurrentRotationSpeed
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
            case SoftEntityAttributes.IsFriendly:
                return typeof(bool);
            case SoftEntityAttributes.NextWaypoint:
                return typeof(UnityEngine.Vector3);
            case SoftEntityAttributes.CurrentRotationSpeed:
                return typeof(float);
            default:
                return typeof(float);
        }
    }
}
