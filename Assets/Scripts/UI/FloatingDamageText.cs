using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingDamageText : MonoBehaviour {

    [SerializeField]
    float timeToComplete = 1f;
    [SerializeField]
    float textDisplacement = 1f;

    public float DamageValue;

    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

	void Start () {
        StartCoroutine("CrawlAndFade");
        text.text = "-" + DamageValue;
	}

    IEnumerator CrawlAndFade()
    { 
        float timeElapsed = 0f;
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector3 originalPosition = rectTransform.position;
        Vector3 destinationPosition = new Vector3(originalPosition.x, originalPosition.y + textDisplacement, originalPosition.z);

        Color originalColor = text.color;

        while (timeElapsed < timeToComplete)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / timeToComplete;

            rectTransform.position = Vector3.Lerp(originalPosition, destinationPosition, percentageComplete);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, percentageComplete));
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
