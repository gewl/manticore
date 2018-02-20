using System;
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

    GameObject[] abilityMomentumCounters;
    Text[] abilityMomentumCounterTextElements;

    Button[] assignMomentumButtons;

    const string COOLDOWN_OVERLAY = "CooldownOverlay";
    const string ABILITY_MOMENTUM_COUNTER = "AbilityMomentumCounter";
    const string ASSIGN_MOMENTUM_BUTTON = "AssignMomentumButton";

    [SerializeField]
    Sprite emptyAbilityBub;
    [SerializeField]
    EntityGearManagement manticoreGear;

    private void Awake()
    {
        abilityBubImages = new Image[4];
        cooldownOverlays = new Image[4];
        abilityMomentumCounters = new GameObject[4];
        abilityMomentumCounterTextElements = new Text[4];
        assignMomentumButtons = new Button[4];

        for (int i = 0; i < abilityBubs.Length; i++)
        {
            RectTransform abilityBub = abilityBubs[i];

            Image cooldownOverlay = abilityBub.Find(COOLDOWN_OVERLAY).GetComponent<Image>();
            cooldownOverlays[i] = cooldownOverlay;
            cooldownOverlay.enabled = false;

            GameObject abilityMomentumCounter = abilityBub.Find(ABILITY_MOMENTUM_COUNTER).gameObject;
            abilityMomentumCounters[i] = abilityMomentumCounter;
            Text abilityMomentumCounterText = abilityMomentumCounter.GetComponentInChildren<Text>();
            abilityMomentumCounterTextElements[i] = abilityMomentumCounterText;

            Button assignMomentumButton = abilityBub.Find(ASSIGN_MOMENTUM_BUTTON).GetComponent<Button>();
            assignMomentumButtons[i] = assignMomentumButton;

            abilityBubImages[i] = abilityBub.GetComponent<Image>();
        }

        manticoreGear.activeHardwareUpdated += UpdateAbilities;
        MomentumManager.OnMomentumUpdated += UpdateMomentumPointButtons;
        MomentumManager.OnMomentumUpdated += UpdateAbilityMomentumCounters;

        UpdateMomentumPointButtons(MomentumManager.CurrentMomentumData);
        UpdateAbilityMomentumCounters(MomentumManager.CurrentMomentumData);
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
                abilityBubImages[i].sprite = activeHardwareBubSprite;
                abilityMomentumCounters[i].SetActive(false);

                if (cooldownOverlays[i].enabled == true)
                {
                    cooldownOverlays[i].enabled = false;
                }
            }
            else
            {
                HardwareType activeHardwareType = activeHardware.Type;

                activeHardwareBubSprite = DataAssociations.GetHardwareTypeBubImage(activeHardwareType);
                abilityBubImages[i].sprite = activeHardwareBubSprite;
                abilityMomentumCounters[i].SetActive(true);

                activeHardware.CooldownUpdater = null;
                activeHardware.CooldownUpdater += GenerateCooldownUpdater(i);
            }
        }
    }

    void UpdateMomentumPointButtons(MomentumData momentumData)
    {
        int availablePoints = momentumData.UnassignedAvailableMomentumPoints;
        if (availablePoints <= 0)
        {
            ToggleMomentumButtons(false);
            return;
        }

        ToggleMomentumButtons(true);

        HardwareType[] activeHardware = InventoryController.GetEquippedActiveHardware();
        for (int i = 0; i < activeHardware.Length; i++)
        {
            HardwareType hardwareType = activeHardware[i];
            bool isSlotOccupied = hardwareType != HardwareType.None;
            bool isMaxedOut = false;
            if (isSlotOccupied)
            {
                isMaxedOut = momentumData.HardwareTypeToMomentumMap[hardwareType] >= 5;
            }
            assignMomentumButtons[i].interactable = isSlotOccupied && !isMaxedOut;
        }
    }

    void UpdateAbilityMomentumCounters(MomentumData momentumData)
    {
        HardwareType[] allEquippedActiveHardware = InventoryController.GetEquippedActiveHardware();
        for (int i = 0; i < abilityMomentumCounters.Length; i++)
        {
            HardwareType activeHardwareType = allEquippedActiveHardware[i];
            if (activeHardwareType != HardwareType.None)
            {
                abilityMomentumCounterTextElements[i].text = MomentumManager.GetMomentumPointsByHardwareType(activeHardwareType).ToString();
            }
        } 
    }

    void ToggleMomentumButtons(bool isEnabled)
    {
        for (int i = 0; i < assignMomentumButtons.Length; i++)
        {
            assignMomentumButtons[i].gameObject.SetActive(isEnabled);
        }
    }

    public void OnAssignMomentumButtonClick(int buttonIndex)
    {
        HardwareType equippedHardwareType = InventoryController.GetEquippedActiveHardware()[buttonIndex];
        MomentumManager.AssignMomentumPointToHardware(equippedHardwareType);
    }

}
