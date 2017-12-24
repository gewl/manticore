using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    [SerializeField]
    float timeToComplete = 1f;
    float destinationYDisplacement = 65f;
    float baseYDisplacement = 15f;

    Camera mainCamera;
    public float DamageValue;
    public Transform attachedTransform;

    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        mainCamera = Camera.main;
    }

	void Start () {
        StartCoroutine("CrawlAndFade");
        text.text = "-" + DamageValue;
	}

    IEnumerator CrawlAndFade()
    { 
        float timeElapsed = 0f;
        RectTransform rectTransform = GetComponent<RectTransform>();

        Color originalColor = text.color;

        while (timeElapsed < timeToComplete)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / timeToComplete;

            Vector3 basePosition = mainCamera.WorldToScreenPoint(attachedTransform.position);
            basePosition.y += baseYDisplacement;
            Vector3 destinationPosition = basePosition;
            destinationPosition.y += destinationYDisplacement;

            rectTransform.position = Vector3.Lerp(basePosition, destinationPosition, percentageComplete);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, percentageComplete));
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
