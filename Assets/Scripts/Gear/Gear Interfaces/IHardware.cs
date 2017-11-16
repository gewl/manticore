using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHardware
{
    int baseStaminaCost { get; }

    void UseHardware();
    void ApplyPassiveHardware();
}
