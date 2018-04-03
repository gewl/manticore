using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YankHardware : MonoBehaviour, IHardware {

    EntityGearManagement gear;
    HardwareType type = HardwareType.Yank;
    public HardwareType Type { get { return type; } }

    bool isInUse;
    public bool IsInUse { get { return isInUse; } }

    HardwareUseType hardwareUseType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return hardwareUseType; } }

    YankHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as YankHardwareData;
    }

    int YankMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(HardwareType.Yank); } }

    public int StaminaCost { get { return subtypeData.GetStaminaCost(YankMomentum); } }
    float YankCooldown { get { return subtypeData.GetCooldown(YankMomentum); } }

    private bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float percentOfCooldownRemaining = 0.0f;
    public CooldownDelegate CooldownUpdater { get; set; }

    private void OnEnable()
    {
        gear = GetComponent<EntityGearManagement>();
    }

    #region Active hardware use

    public void UseActiveHardware()
    {

    }

    IEnumerator GoOnCooldown()
    {
        float timeElapsed = 0.0f;
        isOnCooldown = true;

        while (timeElapsed < YankCooldown)
        {
            timeElapsed += Time.deltaTime;
            percentOfCooldownRemaining = 1 - (timeElapsed / YankCooldown);
            if (CooldownUpdater != null)
            {
                CooldownUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownUpdater != null)
        {
            CooldownUpdater(percentOfCooldownRemaining);
        }
        isOnCooldown = false;
    }
    #endregion

    #region Passive hardware use

    public void ApplyPassiveHardware(HardwareType activeHardwareType, IHardware activeHardware, GameObject subject)
    {

    }

    #endregion
}
