using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

    Text tooltipHeader;
    Text tooltipText;

    private void Awake()
    {
        tooltipHeader = transform.GetChild(0).GetComponent<Text>();
        tooltipText = transform.GetChild(1).GetComponent<Text>();
    }

    public void UpdateText(string header, string body)
    {
        tooltipHeader.text = header;
        tooltipText.text = body;
    }
}
