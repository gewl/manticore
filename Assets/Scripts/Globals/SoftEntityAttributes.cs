using System;

public enum SoftEntityAttributes
{
    CurrentHealth,
    CurrentMoveSpeed,
    IsFriendly
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
            default:
                return typeof(string);
        }
    }
}
