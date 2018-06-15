using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UnitHealthBar : MonoBehaviour {

    public Transform attachedUnit;

    Camera mainCamera;
    float barHeight = 8f;
    float damageBarAdjustmentTime = 1f;

    RectTransform containerTransform;
    Image healthBarBackground;
    Image damageBar;
    Image healthBar;
    [SerializeField]
    AnimationCurve damageBarAdjustmentCurve;

    bool isAdjustingDamageBar = false;
    RectTransform rectTransform;

    void Awake()
    {
        mainCamera = Camera.main;

        containerTransform = (RectTransform)transform;
        healthBarBackground = transform.GetChild(0).GetComponent<Image>();
        damageBar = transform.GetChild(1).GetComponent<Image>();
        healthBar = transform.GetChild(2).GetComponent<Image>();

        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 attachedUnitPosition = mainCamera.WorldToScreenPoint(attachedUnit.position);
        Debug.Log(attachedUnitPosition);

        attachedUnitPosition.x -= 30f;
        attachedUnitPosition.y -= 50f;
        attachedUnitPosition.z = 1f;

        rectTransform.anchoredPosition = attachedUnitPosition;
    }

    public void SetTotalHealth(float totalHealth)
    {
        Vector2 startingBarSize = new Vector2(totalHealth, barHeight);

        containerTransform.sizeDelta = new Vector2(startingBarSize.x + 4f, barHeight + 4f);
        healthBarBackground.rectTransform.sizeDelta = startingBarSize;
        damageBar.rectTransform.sizeDelta = startingBarSize;
        healthBar.rectTransform.sizeDelta = startingBarSize;
    }

    public void UpdateHealth(float currentHealth)
    {
        float barWidth = currentHealth;
        healthBar.rectTransform.sizeDelta = new Vector2(barWidth, barHeight);

        if (isAdjustingDamageBar)
        {
            CancelInvoke();
            isAdjustingDamageBar = false;
        }
        StartCoroutine("DamageBarAdjustment");
    }

    IEnumerator DamageBarAdjustment()
    {
        isAdjustingDamageBar = true;

        Vector2 initialSize = damageBar.rectTransform.sizeDelta;
        Vector2 targetSize = healthBar.rectTransform.sizeDelta;
        float timeElapsed = 0f;

        while (timeElapsed < damageBarAdjustmentTime)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / damageBarAdjustmentTime;
            float curveCompletion = damageBarAdjustmentCurve.Evaluate(percentageComplete);

            Vector2 newDamageBarSize = new Vector2(Mathf.Lerp(initialSize.x, targetSize.x, curveCompletion), barHeight);
            damageBar.rectTransform.sizeDelta = newDamageBarSize;
            yield return null;
        }

        isAdjustingDamageBar = false;
        gameObject.SetActive(false);
        yield return null;

    }
}
