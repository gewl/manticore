using UnityEngine;
using UnityEngine.UI;

public class StaminaPreview : MonoBehaviour {

    [SerializeField]
    Text staminaIndicatorReadout;
    [SerializeField]
    RectTransform staminaBarContainer;
    [SerializeField]
    EntityStaminaComponent manticoreStamina;

    RectTransform staminaBar;
    float startingStamina;
    float barHeight;

    private void Awake()
    {
        staminaBar = (RectTransform)staminaBarContainer.GetChild(0);
        barHeight = staminaBar.rect.height;
    }

    private void OnEnable()
    {
        startingStamina = manticoreStamina.AdjustedMaximumStamina;

        staminaIndicatorReadout.color = Color.black;
        staminaIndicatorReadout.text = startingStamina.ToString();

        UpdateStaminaBarSize(startingStamina);

        manticoreStamina.TotalStaminaUpdated += UpdateStaminaBarSize;
    }

    private void OnDisable()
    {
        manticoreStamina.TotalStaminaUpdated -= UpdateStaminaBarSize;
    }

    public void UpdateStaminaBarSize(float totalStamina)
    {
        float barWidth = totalStamina * 2f;

        staminaBarContainer.sizeDelta = new Vector2(barWidth + 4, barHeight + 4f);
        Vector2 startingBarSize = new Vector2(barWidth, barHeight);
        staminaBar.sizeDelta = startingBarSize;
    }
}
