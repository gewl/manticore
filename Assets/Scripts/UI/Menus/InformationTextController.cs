using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationTextController : MonoBehaviour {

    Text informationHeader;
    Text informationText;

    private void Awake()
    {
        informationHeader = transform.GetChild(0).GetComponent<Text>();
        informationText = transform.GetChild(1).GetComponent<Text>();
    }

    public void UpdateText(string header, string body)
    {
        informationHeader.text = header;
        informationText.text = body;
    }

    public void ClearText()
    {
        informationHeader.text = "";
        informationText.text = "";
    }
}
