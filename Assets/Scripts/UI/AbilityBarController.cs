using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AbilityBarController : SerializedMonoBehaviour {

    bool applicationQuitting = false;

    const string MOMENTUM_COUNTER_PATH = "Sprites/MomentumCounters/MomentumCounter_";

    [SerializeField]
    RectTransform[] abilityBubs;
    Image[] abilityBubImages;
    Image[] cooldownOverlays;
    Text[] cooldownTexts;

    GameObject[] abilityMomentumCounters;

    Button[] assignMomentumButtons;

    // Used for diffing, so counters can flash different colors & give a little more 
    // visual feedback when these values change.
    int[] displayedMomentumValues;

    const string COOLDOWN_OVERLAY = "CooldownOverlay";
    const string COOLDOWN_TEXT = "CooldownText";
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
        cooldownTexts = new Text[4];
        abilityMomentumCounters = new GameObject[4];
        assignMomentumButtons = new Button[4];

        displayedMomentumValues = new int[4];

        for (int i = 0; i < abilityBubs.Length; i++)
        {
            RectTransform abilityBub = abilityBubs[i];

            Image cooldownOverlay = abilityBub.Find(COOLDOWN_OVERLAY).GetComponent<Image>();
            cooldownOverlays[i] = cooldownOverlay;
            cooldownOverlay.enabled = false;

            Text cooldownText = abilityBub.Find(COOLDOWN_TEXT).GetComponent<Text>();
            cooldownTexts[i] = cooldownText;
            cooldownText.enabled = false;

            GameObject abilityMomentumCounter = abilityBub.Find(ABILITY_MOMENTUM_COUNTER).gameObject;
            abilityMomentumCounters[i] = abilityMomentumCounter;

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

    private void OnApplicationQuit()
    {
        applicationQuitting = true;
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
        if (!applicationQuitting)
        {
            ManticoreGear.activeHardwareUpdated -= UpdateAbilities;
        }
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
        Vector3 newScale = new Vector3(1f, 1f - percentageCooldownRemaining, 1f);
        cooldownOverlays[0].rectTransform.localScale = newScale;
    }

    CooldownDelegate GenerateCooldownPercentUpdater(int abilityIndex)
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
            cooldownOverlays[abilityIndex].fillAmount = 1f - percentageCooldownRemaining;
        };
    }

    CooldownDelegate GenerateCooldownDurationUpdater(int abilityIndex)
    {
        return (durationCooldownRemaining) =>
        {
            Text cooldownText = cooldownTexts[abilityIndex];
            if (durationCooldownRemaining == 0.0f)
            {
                cooldownText.enabled = false;
                return;
            }

            if (cooldownText.enabled == false)
            {
                cooldownText.enabled = true;
            }
            float readoutNumber = (float)Math.Round((double)durationCooldownRemaining, 1);
            cooldownText.text = readoutNumber.ToString();
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

                activeHardware.CooldownPercentUpdater = null;
                activeHardware.CooldownPercentUpdater += GenerateCooldownPercentUpdater(i);

                activeHardware.CooldownDurationUpdater = null;
                activeHardware.CooldownDurationUpdater += GenerateCooldownDurationUpdater(i);
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

                    Image momentumCounterImage = abilityMomentumCounters[i].GetComponent<Image>();
                    Sprite counterSprite = Resources.Load<Sprite>(MOMENTUM_COUNTER_PATH + newMomentumValue.ToString());
                    momentumCounterImage.sprite = counterSprite;

                    StartCoroutine(FlashAbilityMomentumCounter(momentumCounterImage, flashColor));

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
