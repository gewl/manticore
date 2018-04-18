using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveRenewableController : MonoBehaviour {

    const string COOLDOWN_OVERLAY = "CooldownOverlay";

    Image renewableBubImage;
    Image cooldownOverlay;

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
        renewableBubImage = GetComponent<Image>();

        ManticoreGear.activeRenewableUpdated += UpdateRenewable;
        cooldownOverlay = transform.Find(COOLDOWN_OVERLAY).GetComponent<Image>();
        cooldownOverlay.fillAmount = 0.0f;
    }

    private void OnDestroy()
    {
        ManticoreGear.activeRenewableUpdated -= UpdateRenewable;
    }

    void UpdateRenewable(ref IRenewable activeRenewable)
    {
        RenewableTypes activeRenewableType = activeRenewable.Type;
        Sprite activeHardwareBubSprite = DataAssociations.GetRenewableTypeBubImage(activeRenewableType);

        renewableBubImage.sprite = activeHardwareBubSprite;

        activeRenewable.CooldownUpdater = null;
        activeRenewable.CooldownUpdater += GenerateCooldownUpdater();
    }

    CooldownDelegate GenerateCooldownUpdater()
    {
        return (percentOfCooldownRemaining) =>
        {
            cooldownOverlay.fillAmount = percentOfCooldownRemaining;
        };
    }
}
