using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour {

    [SerializeField]
    GameObject floatingLetter;
    Vector3 originalLetterPosition;
    Vector3 originalLetterRotationEuler;

    Animator animator;
    const string IS_ACTIVE_PARAMETER = "isActive";

    [SerializeField]
    Renderer objectRenderer;

    IInteractableObjectController objectController;

    private void Awake()
    {
        objectController = GetComponent<IInteractableObjectController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        originalLetterPosition = floatingLetter.transform.position;
        originalLetterRotationEuler = floatingLetter.transform.rotation.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (animator != null)
        {
            animator.SetBool(IS_ACTIVE_PARAMETER, true);
        }

        floatingLetter.SetActive(true);
        floatingLetter.transform.position = originalLetterPosition;
        floatingLetter.transform.rotation = Quaternion.Euler(originalLetterRotationEuler);

        StartCoroutine("ActivateTerminal");
    }

    private void OnTriggerExit(Collider other)
    {
        if (animator != null)
        {
            animator.SetBool(IS_ACTIVE_PARAMETER, false);
        }

        objectRenderer.material.SetFloat("_OutlineExtrusion", 0.0f);

        floatingLetter.SetActive(false);
        StopCoroutine("ActivateTerminal");
    }

    IEnumerator ActivateTerminal()
    {
        float timeElapsed = 0.0f;

        while (true)
        {
            timeElapsed += Time.deltaTime;

            float pingPongTime = Mathf.PingPong(timeElapsed, 1.0f);
            objectRenderer.material.SetFloat("_OutlineExtrusion", pingPongTime);

            Vector3 letterRotationEuler = originalLetterRotationEuler;
            letterRotationEuler.z += timeElapsed * 90f;
            floatingLetter.transform.rotation = Quaternion.Euler(letterRotationEuler);

            float verticalAdjustment = Mathf.PingPong(timeElapsed + 1f, 2.0f);
            verticalAdjustment -= 1f;
            floatingLetter.transform.position = new Vector3(originalLetterPosition.x, originalLetterPosition.y + verticalAdjustment, originalLetterPosition.z);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (objectController != null)
                {
                    objectController.RegisterInteraction();
                }
                else
                {
                    Debug.Log("This object doesn't have an IInteractableObjectController attached.");
                }
            }
            yield return null;
        }
    }
}
