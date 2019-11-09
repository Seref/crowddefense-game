using UnityEngine;


public class Player2D : MonoBehaviour
{

	void LateUpdate()
	{
		var mousePos = Input.mousePosition;
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);

		transform.rotation = Quaternion.Euler(HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles + new Vector3(0, 0, 90));

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
			var bullet2D = bullet.GetComponent<Bullet2D>();
			bullet2D.speed = 20;
			bullet2D.Fire();
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log(collision.gameObject.tag);
		if (collision.gameObject.tag == "Enemy")
			transform.gameObject.SetActive(false);
	}

}
