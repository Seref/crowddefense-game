using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 10;

	private Rigidbody rigidBody;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	public void Fire()
	{		
			rigidBody.velocity = transform.forward * speed;		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Ground")
		{
			rigidBody.velocity = new Vector3(0, 0, 0);
			transform.gameObject.SetActive(false);
		}
	}
}
