using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimplePlayer : MonoBehaviour
{	

    	
    void LateUpdate()
    {
        var mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos.y -= Screen.height / 2;
        transform.LookAt(new Vector3(mousePos.x, 0, mousePos.y));

		if (Input.GetButtonDown("Fire1"))
			Fire();
	}

	private void Fire() {
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null)
		{
			bullet.transform.position = transform.forward*1;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);
			bullet.GetComponent<Bullet>().Fire();
		}
	}

}
