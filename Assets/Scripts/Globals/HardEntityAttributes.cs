using System;

public enum HardEntityAttributes
{
    StartingHealth,
    BaseMoveSpeed
}

public static class HardEntityAttributeTypes
{
    public static string GetType(HardEntityAttributes attribute)
    {
        switch (attribute)
        {
            case HardEntityAttributes.StartingHealth:
                return "float";
            case HardEntityAttributes.BaseMoveSpeed:
                return "float";
            default:
                return "string";
        }
    }
}