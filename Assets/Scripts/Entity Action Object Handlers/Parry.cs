using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        BasicBullet bullet = other.GetComponent<BasicBullet>();
        bullet.Parry(transform, GetMousePosition());
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
