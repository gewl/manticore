using UnityEngine;

[CreateAssetMenu(menuName="Modifiers/OnDamageMarkModifier")]
public class OnDamageMarkModifier : Modifier {

    public virtual void ActivateMark(Transform markedEntity) { }
}
