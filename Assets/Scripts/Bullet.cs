using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 10;

	private Rigidbody2D rigidBody2D;
	private SpriteRenderer sr;

	void Awake()
	{
		rigidBody2D = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	public void Fire()
	{
		rigidBody2D.velocity = transform.up * speed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		rigidBody2D.velocity = new Vector3(0, 0, 0);
		transform.gameObject.SetActive(false);

	}

	private void Update()
	{
		if (!sr.isVisible)
		{
			rigidBody2D.velocity = new Vector3(0, 0, 0);
			transform.gameObject.SetActive(false);
		}
	}
}
