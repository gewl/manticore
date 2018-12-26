using System;
using UnityEngine;
using UnityEngine.UI;

public class ActiveRenewableController : MonoBehaviour {

    const string COOLDOWN_OVERLAY = "CooldownOverlay";
    const string COOLDOWN_TEXT = "CooldownText";
    const string IN_USE_OVERLAY = "InUseOverlay";

    bool isApplicationQuitting = false;

    Image renewableBubImage;
    Image cooldownOverlay;
    Image inUseOverlay;

    Text cooldownText;

    EntityGearManagement _manticoreGear;
    EntityGearManagement ManticoreGear
    {
        get
        {
            if (_manticoreGear == null)
            {
                if (GameManager.GetPlayerTransform() == null)
                {
                    return null;
                }
                _manticoreGear = GameManager.GetPlayerTransform().GetComponent<EntityGearManagement>();
            }

            return _manticoreGear;
        }
    }

    private void Awake()
    {
        renewableBubImage = GetComponent<Image>();

        ManticoreGear.activeRenewableUpdated += UpdateRenewable;
        cooldownOverlay = transform.Find(COOLDOWN_OVERLAY).GetComponent<Image>();
        cooldownOverlay.fillAmount = 0.0f;

        cooldownText = transform.Find(COOLDOWN_TEXT).GetComponent<Text>();
        cooldownText.enabled = false;

        inUseOverlay = transform.Find(IN_USE_OVERLAY).GetComponent<Image>();
        inUseOverlay.enabled = false;
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    private void OnDestroy()
    {
        if (!isApplicationQuitting && ManticoreGear != null)
        {
            ManticoreGear.activeRenewableUpdated -= UpdateRenewable;
        }
    }

    void UpdateRenewable(ref IRenewable activeRenewable)
    {
        RenewableTypes activeRenewableType = activeRenewable.Type;
        Sprite activeHardwareBubSprite = DataAssociations.GetRenewableTypeBubImage(activeRenewableType);

        renewableBubImage.sprite = activeHardwareBubSprite;

        activeRenewable.DurationUpdater = null;
        activeRenewable.DurationUpdater += GeneratePercentDurationUpdater();

        activeRenewable.CooldownPercentUpdater = null;
        activeRenewable.CooldownPercentUpdater += GenerateCooldownPercentUpdater();

        activeRenewable.CooldownDurationUpdater = null;
        activeRenewable.CooldownDurationUpdater += GenerateCooldownDurationUpdater();
    }

    DurationDelegate GeneratePercentDurationUpdater()
    {
        return (percentOfDurationRemaining) =>
        {
            if (percentOfDurationRemaining == 0.0f)
            {
                inUseOverlay.enabled = false;
                return;
            }

            if (inUseOverlay.enabled == false)
            {
                inUseOverlay.enabled = true;
            }

            inUseOverlay.rectTransform.sizeDelta = new Vector2(renewableBubImage.rectTransform.sizeDelta.x, renewableBubImage.rectTransform.sizeDelta.y * percentOfDurationRemaining);
        };
    }

    CooldownDelegate GenerateCooldownPercentUpdater()
    {
        return (percentOfCooldownRemaining) =>
        {
            cooldownOverlay.fillAmount = percentOfCooldownRemaining;
        };
    }

    CooldownDelegate GenerateCooldownDurationUpdater()
    {
        return (durationOfCooldownRemaining) =>
        {
            if (durationOfCooldownRemaining <= 0.1f)
            {
                cooldownText.enabled = false;
                return;
            }

            if (cooldownText.enabled == false)
            {
                cooldownText.enabled = true;
            }
            float readoutNumber = (float)Math.Round((double)durationOfCooldownRemaining, 1);
            cooldownText.text = readoutNumber.ToString();
        };
    }
}
