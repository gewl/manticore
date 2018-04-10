using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yank : MonoBehaviour {

    [SerializeField]
    AnimationCurve travelCurve;
    [SerializeField]
    AnimationCurve returnCurve;
    Rigidbody yankRigidbody;

    YankHardware yankHardware;

    float travelTime;
    float returnTime;
    bool hasReachedDestination = false;

    Vector3[] cachedPlayerPositions;
    int currentPlayerPositionIndex = 0;

    #region Passive hardware values.

    public bool IsFracturing = false;
    float fractureBulletFactor = 0.75f;
    float arcRange = 30f;

    bool IsNullifying = false;
    NullifyHardware nullifyHardware;
    public void SetNullifying(NullifyHardware _nullifyHardware)
    {
        IsNullifying = true;
        nullifyHardware = _nullifyHardware;
    }

    #endregion

    public void PassReferenceToHardware(YankHardware _yankHardware)
    {
        yankHardware = _yankHardware;

        travelTime = yankHardware.TravelTime;
        returnTime = yankHardware.TravelTime * 0.5f;
    }

    const string TERRAIN_LAYER_ID = "Terrain";
    LayerMask terrainLayer;

    float timeElapsed = 0.0f;

    Vector3 initialPosition;
    Vector3 destinationPosition;

    void Awake()
    {
        terrainLayer = LayerMask.NameToLayer(TERRAIN_LAYER_ID);
        yankRigidbody = GetComponent<Rigidbody>();

        cachedPlayerPositions = new Vector3[5];
    }

    void Start()
    {
        initialPosition = transform.position;
        Vector3 initialDirection = (GameManager.GetMousePositionOnPlayerPlane() - initialPosition).normalized;
        destinationPosition = transform.position + (initialDirection * yankHardware.Range);

        for (int i = 0; i < cachedPlayerPositions.Length; i++)
        {
            cachedPlayerPositions[i] = initialPosition;
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!hasReachedDestination)
        {
            if (timeElapsed >= travelTime)
            {
                hasReachedDestination = true;
                timeElapsed = 0.0f;
                return;
            }
            float percentageToDestination = (timeElapsed / travelTime);
            float curvedPercentage = travelCurve.Evaluate(percentageToDestination);

            transform.position = Vector3.Lerp(initialPosition, destinationPosition, curvedPercentage);
        }
        else 
        {
            Vector3 playerPosition = GameManager.GetPlayerPosition();
            if (timeElapsed >= returnTime)
            {
                Vector3 fireDirection = playerPosition - destinationPosition;

                if (IsFracturing)
                {
                    FireAndDestroy_Fracture(fireDirection);
                }
                else if (IsNullifying)
                {
                    FireAndDestroy_Nullify(fireDirection);
                }
                else
                {
                    FireAndDestroy_Default(fireDirection);
                }
            }
            float percentageToDestination = timeElapsed / returnTime;
            float curvedPercentage = returnCurve.Evaluate(percentageToDestination);

            Vector3 averagePlayerPosition = GetAveragePlayerPosition();
            UpdateCachedPositions(playerPosition);
            transform.position = Vector3.Lerp(destinationPosition, averagePlayerPosition, curvedPercentage);

            Vector3 lookDirection = transform.position - averagePlayerPosition;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    #region Projectile travel management
    void UpdateCachedPositions(Vector3 newPosition)
    {
        cachedPlayerPositions[currentPlayerPositionIndex] = newPosition;
        currentPlayerPositionIndex++;

        if (currentPlayerPositionIndex >= cachedPlayerPositions.Length)
        {
            currentPlayerPositionIndex = 0;
        }
    }

    Vector3 GetAveragePlayerPosition()
    {
        Vector3 averagePosition = Vector3.zero;

        for (int i = 0; i < cachedPlayerPositions.Length; i++)
        {
            averagePosition += cachedPlayerPositions[i];
        }

        return averagePosition / cachedPlayerPositions.Length;
    }
    #endregion

    void FireAndDestroy_Default(Vector3 fireDirection)
    {
        BulletController[] bulletChildren = transform.GetComponentsInChildren<BulletController>();

        for (int i = 0; i < bulletChildren.Length; i++)
        {
            BulletController bulletChild = bulletChildren[i];
            bulletChild.SetHoming();
            ApplyArcNoiseAndLaunch(bulletChild, fireDirection);
        }

        Destroy(gameObject);
    }
    
    void FireAndDestroy_Fracture(Vector3 fireDirection)
    {
        GameObject[] children = new GameObject[transform.childCount];

        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < children.Length; i++)
        {
            Instantiate(children[i], transform);
        }

        BulletController[] bulletChildren = transform.GetComponentsInChildren<BulletController>();

        for (int i = 0; i < bulletChildren.Length; i++)
        {
            BulletController bulletChild = bulletChildren[i];

            bulletChild.SetStrength(bulletChild.Strength * fractureBulletFactor);
            ApplyArcNoiseAndLaunch(bulletChild, fireDirection);
        }

        Destroy(gameObject);
    }

    void FireAndDestroy_Nullify(Vector3 fireDirection)
    {
        BulletController[] bulletChildren = transform.GetComponentsInChildren<BulletController>();

        for (int i = 0; i < bulletChildren.Length; i++)
        {
            BulletController bulletChild = bulletChildren[i];
            nullifyHardware.SpawnBulletNullification(bulletChild.gameObject);

            ApplyArcNoiseAndLaunch(bulletChild, fireDirection);
        }

        Destroy(gameObject);
    }

    void ApplyArcNoiseAndLaunch(BulletController bullet, Vector3 fireDirection)
    {
        float arcNoise = Random.Range(-arcRange, arcRange);
        Vector3 newDirection = VectorUtilities.RotatePointAroundPivot(fireDirection, Vector3.zero, arcNoise);

        bullet.Launch(newDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == terrainLayer)
        {
            timeElapsed = travelTime;
            return;
        }

        BulletController bulletController = other.GetComponent<BulletController>();

        if (bulletController != null)
        {
            bulletController.Attach(transform);
        }
    }
}
