using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    ParryComponent parryComponent;

    void Awake()
    {
        parryComponent = GetComponentInParent<ParryComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BasicBullet bullet = other.GetComponent<BasicBullet>();
        if (bullet == null)
        {
            return;
        }
        float parryDamage = parryComponent.ParryDamage;
        bullet.Parry(transform, GetMousePosition(), parryDamage);
    }

    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            hitPoint.y = 0f;
            return hitPoint;
		}
        
        return Vector3.zero;
    }
}
