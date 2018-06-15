using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    [SerializeField]
    float timeToComplete = 1f;
    float destinationYDisplacement = 100f;
    float baseYDisplacement = 75f;

    Camera mainCamera;
    public float DamageValue;
    public Transform attachedTransform;

    [HideInInspector]
    public bool isHealing = false;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        mainCamera = Camera.main;
    }

	void Start () {
        StartCoroutine("CrawlAndFade");
        if (isHealing)
        {
            text.text = "+" + DamageValue;
        }
        else
        {
            text.text = "-" + DamageValue;
        }
	}

    IEnumerator CrawlAndFade()
    { 
        float timeElapsed = 0f;
        RectTransform rectTransform = GetComponent<RectTransform>();

        Color originalColor = new Color(1.0f, 0f, 0f);
        if (isHealing)
        {
            originalColor = Color.green;
        }

        Vector3 initialWorldPosition = attachedTransform.position;

        while (timeElapsed < timeToComplete)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / timeToComplete;

            Vector3 initialScreenPosition = mainCamera.WorldToScreenPoint(initialWorldPosition);
            initialScreenPosition.z = 1f;
            Vector3 destinationPosition = initialScreenPosition;

            initialScreenPosition.y += baseYDisplacement;
            destinationPosition.y += destinationYDisplacement;

            rectTransform.anchoredPosition = Vector3.Lerp(initialScreenPosition, destinationPosition, percentageComplete);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, percentageComplete));
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
