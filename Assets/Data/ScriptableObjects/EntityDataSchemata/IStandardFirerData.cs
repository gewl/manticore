using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStandardFirerData {
    int ProjectileStrength { get; }
    float BulletSpeed { get; }
    float AimNoiseInDegrees { get; }
}
