using System;

public enum HardEntityAttributes
{
    StartingHealth,
    BaseMoveSpeed,
    BaseRotationSpeed,
    StartsFriendly
}

public static class HardEntityAttributeTypes
{
    public static Type GetType(HardEntityAttributes attribute)
    {
        switch (attribute)
        {
            case HardEntityAttributes.StartingHealth:
                return typeof(float);
            case HardEntityAttributes.BaseMoveSpeed:
                return typeof(float);
            case HardEntityAttributes.BaseRotationSpeed:
                return typeof(float);
            case HardEntityAttributes.StartsFriendly:
                return typeof(bool);
            default:
                return typeof(float);
        }
    }
}