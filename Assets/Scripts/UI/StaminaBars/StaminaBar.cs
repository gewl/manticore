using UnityEngine;
using UnityEngine.UI;

public abstract class StaminaBar : MonoBehaviour {

    abstract public void UpdateCurrentStamina(float newCurrentStamina);
    abstract public void UpdateTotalStamina(float newTotalStamina);
}
