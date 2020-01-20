using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
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
		audioSource = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody2D>();
		GetComponent<CircleCollider2D>().radius = Range;		
		coolDown = false;
        audioSource.volume = (SettingsManager.Instance.GetCurrentSettings().MasterSound / 100.0f);
    }

	void OnEnable()
	{
		Dropped = false;
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

	private readonly List<GameObject> Focustargets = new List<GameObject>();
	private readonly HashSet<GameObject> IsInRange = new HashSet<GameObject>();

	private GameObject GetCurrentTarget()
	{
		foreach (var target in Focustargets)
		{
			if (IsInRange.Contains(target))
			{
				return target;
			}
		}

		return null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{		
		if (collision.gameObject.tag == "Enemy")
		{
			IsInRange.Add(collision.gameObject);
			Focustargets.Add(collision.gameObject);			
		}
	}

	void LateUpdate()
	{		
		foreach(var target in Focustargets.ToArray())
		{
			if (!target.activeSelf)
				Focustargets.Remove(target);
		}

		currentTarget = GetCurrentTarget();
		if (currentTarget != null)
		{
			if (currentTarget.activeSelf && Dropped)
			{
				var destination = Quaternion.Euler(0, 0, HelperFunctions.LookAt2D(transform.position, currentTarget.transform.position).eulerAngles.z + 90.0f);
				rigidBody.rotation = Quaternion.Slerp(transform.rotation, destination, Time.deltaTime * Smoothness).eulerAngles.z;
				Fire();
			}
		}
	}	

	private void OnTriggerExit2D(Collider2D collision)
	{
		IsInRange.Remove(collision.gameObject);		
	}	
}
