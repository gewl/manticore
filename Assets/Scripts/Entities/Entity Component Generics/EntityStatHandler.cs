﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityEmitter), typeof(EntityInformation))]
public class EntityStatHandler : EntityComponent {

    float baseDamage { get { return entityInformation.Data.BaseDamage; } }
    float baseMoveSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }

    protected override void Subscribe()
    {
    }

    protected override void Unsubscribe()
    {
    }
}
