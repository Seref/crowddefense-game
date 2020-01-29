using UnityEngine;

public partial class Bullet : MonoBehaviour
{
	public enum BULLETUSER { AUTOTOWER, PLAYER };

	public float Speed = 10;

	private Rigidbody2D rigidBody2D;
	private SpriteRenderer sr;
	private bool wasVisible = false;

	public float Damage = 1f;
	public BULLETUSER ShotBy;

	void Awake()
	{
		rigidBody2D = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	public void Fire(float Damage = 1.0f)
	{
		wasVisible = false;
		Damage = 1.0f;
		rigidBody2D.velocity = transform.up * Speed;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{		
		if (collision.tag == "Enemy" || collision.tag == "Wall" || collision.tag == "TankyEnemy")
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
