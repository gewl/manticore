using System.Collections;
using UnityEngine;

public class FractureHardware : MonoBehaviour, IHardware {

    HardwareType _type = HardwareType.Fracture;
    public HardwareType Type { get { return _type; } }

    HardwareUseType useType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return useType; } }

    FractureHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as FractureHardwareData;
    }

    int FractureMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(_type); } }
    public int StaminaCost { get { return subtypeData.GetStaminaCost(FractureMomentum); } }

    bool isInUse = false;
    public bool IsInUse { get { return isInUse; } }

    bool isOnCooldown = false;
    float FractureCooldown { get { return subtypeData.GetCooldown(FractureMomentum); } }
    float percentageOfCooldownRemaining = 0.0f;
    public bool IsOnCooldown { get { return isOnCooldown; } }

    public CooldownDelegate CooldownUpdater { get; set; }

    IEnumerator GoOnCooldown()
    {
        float timeOffCooldown = Time.time + FractureCooldown;

        while (Time.time < timeOffCooldown)
        {
            percentageOfCooldownRemaining = 1 - (timeOffCooldown - Time.time) / FractureCooldown;
            if (CooldownUpdater != null)
            {
                CooldownUpdater(percentageOfCooldownRemaining);
            }
            yield return null;
        }

        percentageOfCooldownRemaining = 0.0f;
        if (CooldownUpdater != null)
        {
            CooldownUpdater(percentageOfCooldownRemaining);
        }
        isOnCooldown = false;
    }

    public void UseActiveHardware()
    {

    }

    public void ApplyPassiveHardware(HardwareType hardwareType, IHardware hardware, GameObject subject)
    {

    }
}
