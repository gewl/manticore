using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    abstract public MeshRenderer MeshRenderer { get; }
    public float rotationStrength;
    public Material deathSkin;
    public Rigidbody enemyRigidbody;

    public float baseMoveSpeed = 10f;
    public float minCombatMovementDistance = 20f;
    public float maxCombatMovementDistance = 25f;

    public virtual void Attack() { }

    public void RotateToFace(GameObject target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }

    public void RotateToFace(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }

    public void ChangeVelocity(Vector3 direction, float movementModifier = 1f)
    {
        direction.Normalize();
        enemyRigidbody.velocity = direction * baseMoveSpeed * movementModifier;
    }

    public void Stop()
    {
        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;
    }

    public Vector3 GenerateCombatMovementPosition(Vector3 target, Vector3 currentPositionDifference)
    {
        Vector3 tempCheckpoint = currentPositionDifference.normalized;
        tempCheckpoint.x = tempCheckpoint.x + Random.Range(-.2f, .2f);
        tempCheckpoint.z = tempCheckpoint.z + Random.Range(-.2f, .2f);

        tempCheckpoint.x = target.x + (tempCheckpoint.x * Random.Range(minCombatMovementDistance, maxCombatMovementDistance));
        tempCheckpoint.z = target.z + (tempCheckpoint.z * Random.Range(minCombatMovementDistance, maxCombatMovementDistance));
        tempCheckpoint.y = transform.position.y;

        return tempCheckpoint;
    }
}
