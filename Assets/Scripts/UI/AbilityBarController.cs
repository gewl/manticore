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

    // Used for diffing, so counters can flash different colors & give a little more 
    // visual feedback when these values change.
    int[] displayedMomentumValues;

    const string COOLDOWN_OVERLAY = "CooldownOverlay";
    const string ABILITY_MOMENTUM_COUNTER = "AbilityMomentumCounter";
    const string ASSIGN_MOMENTUM_BUTTON = "AssignMomentumButton";

    [SerializeField]
    Sprite emptyAbilityBub;

    Color defaultMomentumDisplayColor;
    [SerializeField]
    Color momentumIncreasedFlashColor;
    [SerializeField]
    Color momentumDecreasedFlashColor;
    [SerializeField]
    float flashTime = 0.5f;

    EntityGearManagement _manticoreGear;
    EntityGearManagement ManticoreGear
    {
        get
        {
            if (_manticoreGear == null)
            {
                _manticoreGear = GameManager.GetPlayerTransform().GetComponent<EntityGearManagement>();
            }

            return _manticoreGear;
        }
    }

    private void Awake()
    {
        abilityBubImages = new Image[4];
        cooldownOverlays = new Image[4];
        abilityMomentumCounters = new GameObject[4];
        abilityMomentumCounterTextElements = new Text[4];
        assignMomentumButtons = new Button[4];

        displayedMomentumValues = new int[4];

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

        defaultMomentumDisplayColor = abilityMomentumCounters[0].GetComponent<Image>().color;

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        ManticoreGear.activeHardwareUpdated += UpdateAbilities;
        MomentumManager.OnMomentumUpdated += UpdateMomentumPointButtons;
        MomentumManager.OnMomentumUpdated += UpdateAbilityMomentumCounters;
        GlobalEventEmitter.OnGameStateEvent += OnGlobalEvent;

        UpdateMomentumPointButtons(MomentumManager.CurrentMomentumData);
        UpdateAbilityMomentumCounters(MomentumManager.CurrentMomentumData);
    }

    void UnsubscribeFromEvents()
    {
        ManticoreGear.activeHardwareUpdated -= UpdateAbilities;
        MomentumManager.OnMomentumUpdated -= UpdateMomentumPointButtons;
        MomentumManager.OnMomentumUpdated -= UpdateAbilityMomentumCounters;
        GlobalEventEmitter.OnGameStateEvent -= OnGlobalEvent;
    }

    void OnGlobalEvent(GlobalConstants.GameStateEvents gameStateEvent, string eventInformation)
    {
        if (gameStateEvent == GlobalConstants.GameStateEvents.NewSceneLoaded)
        {
        }
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

    void UpdateAbilityMomentumCounters(MomentumData newMomentumData)
    {
        HardwareType[] allEquippedActiveHardware = InventoryController.GetEquippedActiveHardware();
        for (int i = 0; i < abilityMomentumCounters.Length; i++)
        {
            HardwareType activeHardwareType = allEquippedActiveHardware[i];
            if (activeHardwareType != HardwareType.None)
            {
                int newMomentumValue = MomentumManager.GetMomentumPointsByHardwareType(activeHardwareType);

                if (newMomentumValue != displayedMomentumValues[i])
                {
                    Color flashColor = newMomentumValue > displayedMomentumValues[i] ? momentumIncreasedFlashColor : momentumDecreasedFlashColor;
                    abilityMomentumCounterTextElements[i].text = newMomentumValue.ToString();

                    StartCoroutine(FlashAbilityMomentumCounter(abilityMomentumCounters[i].GetComponent<Image>(), flashColor));

                    displayedMomentumValues[i] = newMomentumValue;
                }
            }
        }
    }

    IEnumerator FlashAbilityMomentumCounter(Image momentumCounter, Color flashColor)
    {
        momentumCounter.color = flashColor;

        float timeToReset = Time.time + flashTime;

        while (Time.time < timeToReset)
        {
            float percentageComplete = 1f - (timeToReset - Time.time) / flashTime;

            momentumCounter.color = Color.Lerp(flashColor, defaultMomentumDisplayColor, percentageComplete);

            yield return null;
        }

        momentumCounter.color = defaultMomentumDisplayColor;
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
