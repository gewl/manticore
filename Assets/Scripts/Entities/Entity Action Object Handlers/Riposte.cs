using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riposte : MonoBehaviour {

    bool hasRiposted = false;
    Material blinkSkin;

    float timeToAbsorbBullet = 0.4f;

    RiposteHardware riposteHardware;

    private void OnEnable()
    {
        hasRiposted = false;
        blinkSkin = GetComponent<Renderer>().material;
        riposteHardware = GetComponentInParent<RiposteHardware>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Collider>().enabled = false;
        other.GetComponent<BasicBullet>().enabled = false;

        if (!hasRiposted)
        {
            hasRiposted = true;
            StartCoroutine(AbsorbBullet(other.gameObject, true));
        }
        else
        {
            StartCoroutine(AbsorbBullet(other.gameObject));
        }
    }

    IEnumerator AbsorbBullet(GameObject bullet, bool ripostingBullet = false)
    {
        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;

        Renderer bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.material = blinkSkin;

        Vector3 initialSize = bullet.transform.lossyScale;
        Vector3 destinationSize = Vector3.zero;

        Vector3 initialPosition = bullet.transform.position;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToAbsorbBullet)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / timeToAbsorbBullet;

            bullet.transform.position = Vector3.Lerp(initialPosition, transform.position, percentageComplete);
            bullet.transform.localScale = Vector3.Lerp(initialSize, destinationSize, percentageComplete);

            yield return null;
        }

        if (ripostingBullet)
        {
            Transform bulletFirer = bullet.GetComponent<BasicBullet>().firer;
            riposteHardware.BeginRiposte(bulletFirer);
        }
        Destroy(bullet.gameObject);
    }
}
