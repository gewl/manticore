using System.Collections;
using UnityEngine;

public class Nullify : MonoBehaviour {

    public bool IsFracturing = false;
    float handicap = 0.5f;
    public float TimeToComplete;

    LayerMask terrainLayer;

    const string TERRAIN_LAYER_ID = "Terrain";

    private void Awake()
    {
        terrainLayer = LayerMask.NameToLayer(TERRAIN_LAYER_ID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == terrainLayer)
        {
            return;
        }

        BulletController bullet = other.GetComponent<BulletController>();
        if (bullet == null)
        {
            return;
        }

        if (IsFracturing)
        {
            if (bullet.CompareTag(BulletController.ENEMY_BULLET))
            {
                bullet.Parry(transform.parent, bullet.Strength * handicap, handicap);
            }
        }
        else 
        {
            bullet.Dissolve();
        }
    }

    #region Yank passive: Return behavior
    Vector3 initialPosition, destinationPosition;
    Vector3[] cachedPlayerPositions;
    int currentPlayerPositionIndex = 0;
    float range = 15f;
    float travelTime, returnTime;
    [SerializeField]
    AnimationCurve travelCurve;
    [SerializeField]
    AnimationCurve returnCurve;

    public void SetTravelAndReturn()
    {
        travelTime = TimeToComplete * 0.66f;
        returnTime = TimeToComplete - travelTime;
        
        cachedPlayerPositions = new Vector3[5];
        initialPosition = transform.position;
        Vector3 initialDirection = (GameManager.GetMousePositionOnPlayerPlane() - initialPosition).normalized;
        destinationPosition = transform.position + (initialDirection * range);

        for (int i = 0; i < cachedPlayerPositions.Length; i++)
        {
            cachedPlayerPositions[i] = initialPosition;
        }

        StartCoroutine(TravelAndReturn());
    }

    IEnumerator TravelAndReturn()
    {
        float timeToDestination = Time.time + travelTime;
        float timeToReturn = timeToDestination + returnTime;

        while (Time.time < timeToDestination)
        {
            float percentageToDestination = 1f - (timeToDestination - Time.time) / travelTime;
            float curvedPercentage = travelCurve.Evaluate(percentageToDestination);

            transform.position = Vector3.Lerp(initialPosition, destinationPosition, curvedPercentage);
            yield return null;
        }

        while (Time.time < timeToReturn)
        {
            Vector3 playerPosition = GameManager.GetPlayerPosition();
            float percentageToDestination = 1f - (timeToReturn - Time.time) / returnTime;
            float curvedPercentage = returnCurve.Evaluate(percentageToDestination);

            Vector3 averagePlayerPosition = GetAveragePlayerPosition();
            UpdateCachedPositions(playerPosition);
            transform.position = Vector3.Lerp(destinationPosition, averagePlayerPosition, curvedPercentage);

            Vector3 lookDirection = transform.position - averagePlayerPosition;
            yield return null;
        }
    }

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
}
