using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YankHardware : MonoBehaviour, IHardware {

    EntityGearManagement gear;
    HardwareType type = HardwareType.Yank;
    public HardwareType Type { get { return type; } }

    bool isInUse;
    public bool IsInUse { get { return isInUse; } }

    HardwareUseType hardwareUseType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return hardwareUseType; } }

    YankHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as YankHardwareData;
    }

    int YankMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(HardwareType.Yank); } }

    public int StaminaCost { get { return subtypeData.GetStaminaCost(YankMomentum); } }
    float YankCooldown { get { return subtypeData.GetCooldown(YankMomentum); } }
    public float TravelTime { get { return subtypeData.GetTravelTime(YankMomentum); } }
    public float Range { get { return subtypeData.GetRange(YankMomentum); } }

    private bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float percentOfCooldownRemaining = 0.0f;
    public CooldownDelegate CooldownUpdater { get; set; }

    const string YANK_PROJECTILE_PATH = "Prefabs/Abilities/YankProjectile";
    GameObject _yankProjectile;
    GameObject YankProjectile
    {
        get
        {
            if (_yankProjectile == null)
            {
                _yankProjectile = (GameObject)Resources.Load(YANK_PROJECTILE_PATH);
            }

            return _yankProjectile;
        }
    }

    private void OnEnable()
    {
        gear = GetComponent<EntityGearManagement>();
    }

    #region Active hardware use

    public void UseActiveHardware()
    {
        StartCoroutine(FireYankProjectile());
    }

    IEnumerator FireYankProjectile()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GoOnCooldown());
        Vector3 instantiationPosition = transform.position + (transform.forward);
        GameObject newYankProjectile = Instantiate(YankProjectile, instantiationPosition, transform.rotation);

        gear.ApplyPassiveHardware(typeof(YankHardware), newYankProjectile);

        Yank yankController = newYankProjectile.GetComponent<Yank>();
        yankController.PassReferenceToHardware(this);

        GameManager.JoltScreen(-transform.forward, 0.4f);
    }

    IEnumerator GoOnCooldown()
    {
        float timeElapsed = 0.0f;
        isOnCooldown = true;

        while (timeElapsed < YankCooldown)
        {
            timeElapsed += Time.deltaTime;
            percentOfCooldownRemaining = 1 - (timeElapsed / YankCooldown);
            if (CooldownUpdater != null)
            {
                CooldownUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownUpdater != null)
        {
            CooldownUpdater(percentOfCooldownRemaining);
        }
        isOnCooldown = false;
    }
    #endregion

    #region Passive hardware use

    public void ApplyPassiveHardware(HardwareType activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        switch (activeHardwareType)
        {
            case HardwareType.None:
                Debug.LogError("Attempting to apply Yank passive effect to empty hardware.");
                break;
            case HardwareType.Parry:
                ApplyPassiveHardware_Parry(subject);
                break;
            case HardwareType.Blink:
                ApplyPassiveHardware_Blink(activeHardware as BlinkHardware);
                break;
            case HardwareType.Nullify:
                ApplyPassiveHardware_Nullify(subject);
                break;
            case HardwareType.Fracture:
                ApplyPassiveHardware_Fracture(subject);
                break;
            case HardwareType.Yank:
                Debug.LogError("Trying to apply Yank passive effect to Yank active effect.");
                break;
            default:
                break;
        }
    }

    void ApplyPassiveHardware_Parry(GameObject bullet)
    {
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.SetHoming();
        bulletController.SetStrength(bulletController.Strength / 2f);
    }

    void ApplyPassiveHardware_Blink(BlinkHardware blinkHardware)
    {
        blinkHardware.PutInReturnState();
    }

    void ApplyPassiveHardware_Nullify(GameObject nullifyEffect)
    {
        nullifyEffect.GetComponent<Nullify>().SetTravelAndReturn();
    }

    void ApplyPassiveHardware_Fracture(GameObject bulletFragment)
    {
        BulletController bulletController = bulletFragment.GetComponent<BulletController>();
        bulletController.SetHoming();
        bulletController.SetStrength(bulletController.Strength / 2f);
    }

    #endregion
}
