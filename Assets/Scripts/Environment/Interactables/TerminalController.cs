using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : MonoBehaviour {

    [SerializeField]
    GameObject floatingLetter;
    [SerializeField]
    InventoryMenuController inventoryMenu;

    Vector3 originalLetterPosition;
    bool debugEquipped = false;

    Vector3 originalLetterRotationEuler;

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

        if (!debugEquipped)
        {
            InventoryController.EquipActiveHardware(2, HardwareTypes.Nullify);
            debugEquipped = true;
        }
        else
        {
            InventoryController.UnequipActiveHardware(2);
            debugEquipped = false;
        }
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
                inventoryMenu.ToggleMenu();
            }
            yield return null;
        }
    }
}
