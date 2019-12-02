using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
	public float CoolDownTime = 20.0f;
	public FloatingCounter floatCounter;

	private Rigidbody2D rigidBody;
	private GameManager gameManager;
    private AudioSource audioSource;

    private bool coolDown = false;

	void Start()
	{
        audioSource = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody2D>();
		gameManager = FindObjectOfType<GameManager>();
	}

	void LateUpdate()
	{
		if (Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
		{
			var mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);
			rigidBody.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z + 90;

			if (Input.GetButtonDown("Fire1"))
				Fire();
		}
	}

	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null && !coolDown)
		{
			coolDown = true;
            audioSource.Play();
            bullet.transform.position = transform.position + transform.up * 1.25f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);
			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.Speed = 20;
			bulletScript.Fire();

			var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UIEffects").transform);
			Text.Show(CoolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y);
			StartCoroutine(CoolDownCounter());
		}
	}

	private IEnumerator CoolDownCounter()
	{
		yield return new WaitForSeconds(CoolDownTime);
		coolDown = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			transform.gameObject.SetActive(false);
			gameManager.GameOver();
		}
	}

}
