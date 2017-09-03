using System;

public enum SoftEntityAttributes
{
    CurrentHealth,
    CurrentMoveSpeed,
    IsFriendly
}

public static class SoftEntityAttributeTypes
{
    public static string GetType(SoftEntityAttributes attribute)
    {
        switch (attribute)
        {
            case SoftEntityAttributes.CurrentHealth:
                return "float";
            case SoftEntityAttributes.CurrentMoveSpeed:
                return "float";
            case SoftEntityAttributes.IsFriendly:
                return "bool";
            default:
                return "string";
        }
    }
}
