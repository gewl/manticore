using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    abstract public MeshRenderer MeshRenderer { get; }
    public float rotationStrength;
    public Material deathSkin;

    public virtual void Attack() { }

    public void RotateToFace(GameObject target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }
}
