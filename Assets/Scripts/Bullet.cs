using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float Speed = 10;

	private Rigidbody2D rigidBody2D;
	private SpriteRenderer sr;
	private bool wasVisible = false;

	void Awake()
	{
		rigidBody2D = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	public void Fire()
	{
		wasVisible = false;
		rigidBody2D.velocity = transform.up * Speed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{		
		if (collision.tag == "Enemy" || collision.tag == "Wall")
		{
			rigidBody2D.velocity = new Vector3(0, 0, 0);
			transform.gameObject.SetActive(false);
		}
	}

	
	private void Update()
	{
		if (sr.isVisible) {
			wasVisible = true;
		}
		if (!sr.isVisible && wasVisible)
		{
			wasVisible = false;
			rigidBody2D.velocity = new Vector3(0, 0, 0);
			transform.gameObject.SetActive(false);
		}
	}
}
