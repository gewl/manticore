using System;

public enum HardEntityAttributes
{
    StartingHealth,
    BaseRotationSpeed,
    StartsAggroed
}

public static class HardEntityAttributeTypes
{
    public static Type GetType(HardEntityAttributes attribute)
    {
        switch (attribute)
        {
            case HardEntityAttributes.StartingHealth:
                return typeof(float);
            case HardEntityAttributes.BaseRotationSpeed:
                return typeof(float);
            case HardEntityAttributes.StartsAggroed:
                return typeof(bool);
            default:
                return typeof(float);
        }
    }
}