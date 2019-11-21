using System.Collections;
using UnityEngine;

public class AutoTower : MonoBehaviour
{

    public float range = 5.0f;
	public float coolDownTime = 2.0f;
    public FloatingCounter floatCounter;

	private Rigidbody2D rb;

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

    private bool coolDown = false;
	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null && !coolDown)
		{
            coolDown = true;
			bullet.transform.position = transform.position + transform.up * 1.25f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);
			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.speed = 20;
			bulletScript.Fire();
            var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UI").transform);
            Text.Show(coolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y);
            StartCoroutine(CoolDownCounter());
		}
	}

    private IEnumerator CoolDownCounter() {
        yield return new WaitForSeconds(coolDownTime);
        coolDown = false;        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{		
		if (collision.gameObject.tag == "Enemy")
			transform.gameObject.SetActive(false);
	}

}
