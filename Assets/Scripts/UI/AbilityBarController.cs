﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AbilityBarController : SerializedMonoBehaviour {

    [SerializeField]
    RectTransform[] abilityBubs;
    Image[] abilityBubImages;
    Image[] cooldownOverlays;

    [SerializeField]
    Sprite emptyAbilityBub;
    [SerializeField]
    Dictionary<HardwareTypes, Sprite> bubImageDict;
    [SerializeField]
    EntityGearManagement manticoreGear;

    private void Awake()
    {
        abilityBubImages = new Image[4]
        {
            abilityBubs[0].GetComponent<Image>(),
            abilityBubs[1].GetComponent<Image>(),
            abilityBubs[2].GetComponent<Image>(),
            abilityBubs[3].GetComponent<Image>(),
        };

        cooldownOverlays = new Image[4];

        for (int i = 0; i < abilityBubs.Length; i++)
        {
            RectTransform abilityBub = abilityBubs[i];
            Image cooldownOverlay = abilityBub.GetChild(0).GetComponent<Image>();
            cooldownOverlays[i] = cooldownOverlay;
            cooldownOverlay.enabled = false;
        }
    }

    private void OnEnable()
    {
        manticoreGear.activeHardwareUpdated += UpdateAbilities;
    }

    private void OnDisable()
    {
        manticoreGear.activeHardwareUpdated -= UpdateAbilities;
    }

    void UpdateParryCooldown(float percentageCooldownRemaining)
    {
        if (percentageCooldownRemaining == 0.0f)
        {
            cooldownOverlays[0].enabled = false;
            return;
        }
        if (cooldownOverlays[0].enabled == false)
        {
            cooldownOverlays[0].enabled = true;
        }
        Vector3 newScale = new Vector3(1f, percentageCooldownRemaining, 1f);
        cooldownOverlays[0].rectTransform.localScale = newScale;
    }

    CooldownDelegate GenerateCooldownUpdater(int abilityIndex)
    {
        return (percentageCooldownRemaining) =>
        {
            if (percentageCooldownRemaining == 0.0f)
            {
                cooldownOverlays[abilityIndex].enabled = false;
                return;
            }
            if (cooldownOverlays[abilityIndex].enabled == false)
            {
                cooldownOverlays[abilityIndex].enabled = true;
            }
            Vector3 newScale = new Vector3(1f, percentageCooldownRemaining, 1f);
            cooldownOverlays[abilityIndex].rectTransform.localScale = newScale;
        };
    }

    public void UpdateAbilities(ref IHardware[] activeHardwareList)
    {

        for (int i = 0; i < activeHardwareList.Length; i++)
        {
            IHardware activeHardware = activeHardwareList[i];
            Sprite activeHardwareBubSprite;
            if (activeHardware == null)
            {
                activeHardwareBubSprite = emptyAbilityBub;
            }
            else
            {
                HardwareTypes activeHardwareType = activeHardware.Type;

                activeHardwareBubSprite = bubImageDict[activeHardwareType];
                abilityBubImages[i].sprite = activeHardwareBubSprite;

                activeHardware.CooldownUpdater = null;
                activeHardware.CooldownUpdater += GenerateCooldownUpdater(i);
            }
        }
    }
}
