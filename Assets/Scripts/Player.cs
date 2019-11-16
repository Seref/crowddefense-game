using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

	Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();		
	}
	
	void LateUpdate()
	{
		var mousePos = Input.mousePosition;
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);

		rb.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z+90;
		
		if (Input.GetButtonDown("Fire1"))
			Fire();
	}

	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null)
		{
			bullet.transform.position = transform.position + transform.up * 1.25f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);
			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.speed = 20;
			bulletScript.Fire();
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{		
		if (collision.gameObject.tag == "Enemy")
			transform.gameObject.SetActive(false);
	}

}
