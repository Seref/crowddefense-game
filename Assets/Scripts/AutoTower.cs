using System.Collections;
using UnityEngine;

public class AutoTower : MonoBehaviour
{

	public float Range = 5.0f;
	public float CoolDownTime = 2.0f;
	public FloatingCounter FloatCounter;
	public bool Dropped = false;
	public float Smoothness = 1.3f;

	private Rigidbody2D rb;
	private bool coolDown = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		GetComponent<CircleCollider2D>().radius = Range;
	}

	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null && !coolDown)
		{
			coolDown = true;
			bullet.transform.position = transform.position + transform.up * 1.2f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);

			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.Speed = 20;
			bulletScript.Fire();

			var Text = Instantiate(FloatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UI").transform);
			Text.Show(CoolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y / 2);
			StartCoroutine(CoolDownCounter());
		}
	}

	private IEnumerator CoolDownCounter()
	{
		yield return new WaitForSeconds(CoolDownTime);
		coolDown = false;
	}

	private GameObject currentTarget = null;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			if (currentTarget == null)
			{
				currentTarget = collision.gameObject;
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			if (currentTarget != null)
			{
				if (currentTarget.activeSelf && Dropped)
				{
					var destination = Quaternion.Euler(0, 0, HelperFunctions.LookAt2D(transform.position, collision.gameObject.transform.position).eulerAngles.z + 90.0f);
					rb.rotation = Quaternion.Slerp(transform.rotation, destination, Time.deltaTime * Smoothness).eulerAngles.z;
					Fire();
				}
				else
				{
					currentTarget = null;
				}
			}
			else
			{
				currentTarget = collision.gameObject;
			}
		}
		
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy" && currentTarget != null && currentTarget == collision.gameObject)
		{
			currentTarget = null;
		}
	}

}
