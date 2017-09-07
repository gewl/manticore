using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAggroComponent : EntityComponent {
    [SerializeField]
    float aggroRange = 0f;

    void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
    }

    public override void Cleanup()
    {
    }
}
