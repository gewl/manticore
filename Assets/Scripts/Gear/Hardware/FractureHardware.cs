﻿using System.Collections;
using UnityEngine;

public class FractureHardware : MonoBehaviour, IHardware {

    HardwareType _type = HardwareType.Fracture;
    public HardwareType Type { get { return _type; } }

    HardwareUseType useType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return useType; } }

    EntityGearManagement gear;

    FractureHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as FractureHardwareData;
    }

    int FractureMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(_type); } }
    float ActiveProjectileDamage { get { return subtypeData.GetDamageDealt(FractureMomentum); } }
    public int StaminaCost { get { return subtypeData.GetStaminaCost(FractureMomentum); } }
    public int ActiveNumberOfProjectiles { get { return subtypeData.GetNumberOfBullets(FractureMomentum); } }
    float ActiveArcOfFire { get { return subtypeData.GetArcOfFire(FractureMomentum); } }

    float ActiveProjectileSpeed { get { return 30f; } }

    float TravelTime { get { return subtypeData.GetTravelTime(FractureMomentum); } }
    float LingerTime { get { return subtypeData.GetLingerTime(FractureMomentum); } }

    int PassiveNumberOfProjectiles = 2;
    float PassiveProjectileDamage = 10f;
    float PassiveArcOfFire = 40f;
    float PassiveProjectileSpeed = 20.0f;

    bool isInUse = false;
    public bool IsInUse { get { return isInUse; } }

    bool isOnCooldown = false;
    float FractureCooldown { get { return subtypeData.GetCooldown(FractureMomentum); } }
    float percentOfCooldownRemaining = 0.0f;
    public bool IsOnCooldown { get { return isOnCooldown; } }

    public CooldownDelegate CooldownPercentUpdater { get; set; }
    public CooldownDelegate CooldownDurationUpdater { get; set; }

    const string FRACTURE_PROJECTILE_PATH = "Prefabs/Abilities/FractureProjectile";
    GameObject _fractureProjectile;
    GameObject FractureProjectile
    {
        get
        {
            if (_fractureProjectile == null)
            {
                _fractureProjectile = (GameObject)Resources.Load(FRACTURE_PROJECTILE_PATH);
            }

            return _fractureProjectile;
        }
    }

    void Awake()
    {
        gear = GetComponent<EntityGearManagement>();
    }

    IEnumerator GoOnCooldown()
    {
        isOnCooldown = true;
        float timeOffCooldown = Time.time + FractureCooldown;

        while (Time.time < timeOffCooldown)
        {
            float cooldownRemaining = timeOffCooldown - Time.time;
            percentOfCooldownRemaining = 1 - cooldownRemaining / FractureCooldown;

            if (CooldownDurationUpdater != null)
            {
                CooldownDurationUpdater(cooldownRemaining);
            }

            if (CooldownPercentUpdater != null)
            {
                CooldownPercentUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownDurationUpdater != null)
        {
            CooldownDurationUpdater(percentOfCooldownRemaining);
        }

        if (CooldownPercentUpdater != null)
        {
            CooldownPercentUpdater(percentOfCooldownRemaining);
        }
        isOnCooldown = false;
    }

    public void UseActiveHardware()
    {
        StartCoroutine(FireFractureProjectile());
    }

    IEnumerator FireFractureProjectile()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 instantiationPosition = transform.position + transform.forward + (transform.up * 2f);
        GameObject newFractureProjectile = GameObject.Instantiate(FractureProjectile, instantiationPosition, transform.rotation);
        newFractureProjectile.GetComponent<Rigidbody>().velocity = transform.forward * 30.0f;

        Fracture fractureController = newFractureProjectile.GetComponent<Fracture>();
        fractureController.PassReferenceToHardware(this);
        fractureController.TimeToStop = TravelTime;
        fractureController.TimeToDestroy = TravelTime + LingerTime;

        GameManager.JoltScreen(-transform.forward, 0.8f);
        StartCoroutine(GoOnCooldown());
    }

    public void FractureBullet(Vector3 impactPoint, Vector3 impactNormal, bool isActiveHardware = true)
    {
        int numberOfProjectiles = isActiveHardware ? ActiveNumberOfProjectiles : PassiveNumberOfProjectiles;
        float arcOfFire = isActiveHardware ? ActiveArcOfFire : PassiveArcOfFire;
        float projectileDamage = isActiveHardware ? ActiveProjectileDamage : PassiveProjectileDamage;
        float projectileSpeed = isActiveHardware ? ActiveProjectileSpeed : PassiveProjectileSpeed;

        int projectilesSpawned = 0;

        while (projectilesSpawned < numberOfProjectiles)
        {
            GameObject newBullet = Instantiate(GameManager.BulletPrefab, impactPoint, Quaternion.identity, GameManager.BulletsParent.transform);

            float angleAdjustment = Random.Range(-arcOfFire, arcOfFire);
            Vector3 updatedDirection = VectorUtilities.RotatePointAroundPivot(impactNormal + impactPoint, impactPoint, angleAdjustment);

            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.InitializeValues(projectileDamage, updatedDirection, transform, null, projectileSpeed);
            bulletController.SetFriendly();

            if (isActiveHardware)
            {
                gear.ApplyPassiveHardware(typeof(FractureHardware), newBullet);
            }

            projectilesSpawned++;
        }
    }

    public void ApplyPassiveHardware(HardwareType activeHardwareType, IHardware hardware, GameObject subject)
    {
        switch (activeHardwareType)
        {
            case HardwareType.Parry:
                ApplyPassiveHardware_Parry(subject);
                break;
            case HardwareType.Blink:
                ApplyPassiveHardware_Blink(hardware as BlinkHardware);
                break;
            case HardwareType.Nullify:
                ApplyPassiveHardware_Nullify(subject);
                break;
            case HardwareType.Fracture:
                Debug.LogError("Trying to apply Fracture pasive effect to Fracture active hardware.");
                break;
            case HardwareType.Yank:
                ApplyPassiveHardware_Yank(subject);
                break;
            default:
                break;
        }
    }

    void ApplyPassiveHardware_Parry(GameObject bullet)
    {
        Vector3 bulletPosition = bullet.transform.position;
        Vector3 bulletToMouse = GameManager.GetMousePositionOnPlayerPlane() - bulletPosition;
        FractureBullet(bulletPosition, bulletToMouse, false);
    }

    void ApplyPassiveHardware_Blink(BlinkHardware blinkHardware)
    {
        blinkHardware.DoesBlinkStun = true;
    }
    
    void ApplyPassiveHardware_Nullify(GameObject nullificationField)
    {
        nullificationField.GetComponent<Nullify>().IsFracturing = true;
    }

    void ApplyPassiveHardware_Yank(GameObject yankProjectile)
    {
        yankProjectile.GetComponent<Yank>().IsFracturing = true;
    }
}
