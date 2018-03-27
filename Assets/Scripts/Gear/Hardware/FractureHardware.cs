using System.Collections;
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
    float FractureBulletDamage { get { return subtypeData.GetDamageDealt(FractureMomentum); } }
    public int StaminaCost { get { return subtypeData.GetStaminaCost(FractureMomentum); } }
    int NumberOfProjectiles { get { return subtypeData.GetNumberOfBullets(FractureMomentum); } }
    float ArcOfFire { get { return subtypeData.GetArcOfFire(FractureMomentum); } }
    float ProjectileSpeed { get { return subtypeData.GetFragmentationSpeed(FractureMomentum); } }

    bool isInUse = false;
    public bool IsInUse { get { return isInUse; } }

    bool isOnCooldown = false;
    float FractureCooldown { get { return subtypeData.GetCooldown(FractureMomentum); } }
    float percentageOfCooldownRemaining = 0.0f;
    public bool IsOnCooldown { get { return isOnCooldown; } }

    public CooldownDelegate CooldownUpdater { get; set; }

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
        float timeOffCooldown = Time.time + FractureCooldown;

        while (Time.time < timeOffCooldown)
        {
            percentageOfCooldownRemaining = 1 - (timeOffCooldown - Time.time) / FractureCooldown;
            if (CooldownUpdater != null)
            {
                CooldownUpdater(percentageOfCooldownRemaining);
            }
            yield return null;
        }

        percentageOfCooldownRemaining = 0.0f;
        if (CooldownUpdater != null)
        {
            CooldownUpdater(percentageOfCooldownRemaining);
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
        Vector3 instantiationPosition = transform.position + (transform.forward);
        GameObject newFractureProjectile = GameObject.Instantiate(FractureProjectile, instantiationPosition, transform.rotation);
        newFractureProjectile.GetComponent<Rigidbody>().velocity = transform.forward * 30.0f;
        newFractureProjectile.GetComponent<Fracture>().PassReferenceToHardware(this);

        GameManager.JoltScreen(-transform.forward, 0.8f);
    }

    public void FractureBullet(Vector3 impactPoint, Vector3 impactNormal)
    {
        int numberOfProjectiles = NumberOfProjectiles;

        int projectilesSpawned = 0;

        while (projectilesSpawned < numberOfProjectiles)
        {
            GameObject newBullet = Instantiate(GameManager.BulletPrefab, impactPoint, Quaternion.identity, GameManager.BulletsParent.transform);

            float angleAdjustment = Random.Range(-ArcOfFire, ArcOfFire);
            Vector3 updatedDirection = VectorUtilities.RotatePointAroundPivot(impactNormal + impactPoint, impactPoint, angleAdjustment);
            newBullet.GetComponent<BulletController>().InitializeValues(FractureBulletDamage, updatedDirection, transform, null, ProjectileSpeed);
            newBullet.GetComponent<BulletController>().SetFriendly();

            gear.ApplyPassiveHardware(typeof(FractureHardware), newBullet);

            projectilesSpawned++;
        }
    }

    public void ApplyPassiveHardware(HardwareType hardwareType, IHardware hardware, GameObject subject)
    {

    }
}
