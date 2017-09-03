using System;

public enum HardEntityAttributes
{
    StartingHealth,
    BaseMoveSpeed
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
            default:
                return typeof(string);
        }
    }
}