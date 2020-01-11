using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;

public class AutoTower : MonoBehaviour
{

	public float Range = 5.0f;
	public float FocusTime = 3.0f;
	public float CoolDownTime = 2.0f;
	public float Smoothness = 1.3f;

	public FloatingCounter FloatCounter;

	public bool Dropped = false;

	private Rigidbody2D rigidBody;
	private bool coolDown = false;
	private bool isFocusing = false;
	private AudioSource audioSource;

	void Start()
	{
		Settings s = SettingsManager.Instance.GetCurrentSettings();
		CoolDownTime = s.AutoTowerFireCooldown;

		audioSource = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody2D>();
		GetComponent<CircleCollider2D>().radius = Range;
		coolDown = false;
	}

	void OnEnable()
	{
		coolDown = false;
	}

	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null && !coolDown)
		{
			audioSource.Play();
			coolDown = true;
			bullet.transform.position = transform.position + transform.up * 1.2f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);

			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.Speed = 20;
			bulletScript.Fire();

			var Text = Instantiate(FloatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("AdditionalUI").transform);
			Text.Show(CoolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y / 2);
			StartCoroutine(CoolDownCounter());
		}
	}

	private IEnumerator CoolDownCounter()
	{
		yield return new WaitForSeconds(CoolDownTime);
		coolDown = false;
	}

	private IEnumerator FocusCounter()
	{
		yield return new WaitForSeconds(FocusTime);
		isFocusing = false;
		currentTarget = null;
	}

	private GameObject currentTarget = null;
	private Coroutine focusCoroutine;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			if (currentTarget == null && !isFocusing)
			{
				currentTarget = collision.gameObject;
				isFocusing = true;
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
					rigidBody.rotation = Quaternion.Slerp(transform.rotation, destination, Time.deltaTime * Smoothness).eulerAngles.z;
					Fire();
				}
				else
				{
					currentTarget = null;
					isFocusing = false;
					StopFocusing();
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
			StartFocusing();
		}
	}

	private void StopFocusing()
	{
		if (focusCoroutine != null)
			StopCoroutine(focusCoroutine);
	}

	private void StartFocusing()
	{
		if (focusCoroutine != null)
			StopCoroutine(focusCoroutine);

		if (gameObject.activeSelf)
			focusCoroutine = StartCoroutine(FocusCounter());
	}

}
