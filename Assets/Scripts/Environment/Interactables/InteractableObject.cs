using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    [SerializeField]
    GameObject floatingLetter;
    Vector3 originalLetterPosition;
    Vector3 originalLetterRotationEuler;

    IInteractableObjectController objectController;

    private void Awake()
    {
        objectController = GetComponent<IInteractableObjectController>();
    }

    private void OnEnable()
    {
        originalLetterPosition = floatingLetter.transform.position;
        originalLetterRotationEuler = floatingLetter.transform.rotation.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        floatingLetter.SetActive(true);
        floatingLetter.transform.position = originalLetterPosition;
        floatingLetter.transform.rotation = Quaternion.Euler(originalLetterRotationEuler);
        StartCoroutine("ActivateTerminal");
    }

    private void OnTriggerExit(Collider other)
    {
        floatingLetter.SetActive(false);
        StopCoroutine("ActivateTerminal");
    }

    IEnumerator ActivateTerminal()
    {
        float timeElapsed = 0.0f;
        while (true)
        {
            timeElapsed += Time.deltaTime;
            Vector3 letterRotationEuler = originalLetterRotationEuler;
            letterRotationEuler.z += timeElapsed * 90f;
            floatingLetter.transform.rotation = Quaternion.Euler(letterRotationEuler);

            float verticalAdjustment = Mathf.PingPong(timeElapsed + 1f, 2.0f);
            verticalAdjustment -= 1f;
            floatingLetter.transform.position = new Vector3(originalLetterPosition.x, originalLetterPosition.y + verticalAdjustment, originalLetterPosition.z);
            if (Input.GetKeyDown(KeyCode.F))
            {
                objectController.RegisterInteraction();
            }
            yield return null;
        }
    }
}
