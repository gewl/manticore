using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{

    abstract public MeshRenderer MeshRenderer { get; }
    public Material deathSkin;

    public virtual void Attack() { }
}
