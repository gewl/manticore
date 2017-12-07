using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CooldownDelegate(float percentageCooldownRemaining);

public interface IHardware
{
    int BaseStaminaCost { get; }
    int UpdatedStaminaCost { get; }

    bool IsInUse { get; }
    bool IsOnCooldown { get; }
    CooldownDelegate CooldownUpdater { get; set; }

    HardwareTypes Type { get; }
    HardwareUseTypes HardwareUseType { get; }

    void UseActiveHardware();

    /// <summary>
    /// This universal 'ApplyPassiveHardware' member is called for any active hardware
    /// on which the passive hardware bearing this interface is implemented.
    /// </summary>
    /// <param name="equippedHardwareType">The type of (active) hardware on which this passive hardware is equipped.</param>
    /// <param name="subject">The active-hardware-related object on which this hardware's passive effect is applied.</param>
    /// <remarks>
    /// What 'subject' refers to will depend on the hardware type.
    /// e.g. a bullet for Parry hardware, spawnedNullification for Nullify hardware, or riposteTarget for Riposte.
    /// </remarks>
    void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject);
}
