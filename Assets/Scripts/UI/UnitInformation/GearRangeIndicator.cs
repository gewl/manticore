using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRangeIndicator : MonoBehaviour {

    Camera mainCamera;
    RectTransform rectTransform;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdatePosition(Vector3 aimPosition)
    {
        Vector2 origin = (Vector2)mainCamera.WorldToScreenPoint(GameManager.GetPlayerPosition());
        Vector2 screenToAimPosition = (Vector2)mainCamera.WorldToScreenPoint(aimPosition) - origin;

        float arrowAngle = Vector2.Angle(Vector2.right, screenToAimPosition) * Mathf.Sign(screenToAimPosition.y);

        rectTransform.position = origin;
        rectTransform.eulerAngles = new Vector3(0f, 0f, arrowAngle);

        rectTransform.sizeDelta = new Vector2(screenToAimPosition.magnitude, 20f);
    }
}
