using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoTower : MonoBehaviour
{
	public float Range = 5.0f;
	public float CoolDownTime = 2.0f;
	public float Smoothness = 1.3f;
	private StatsManager statsmanager;

	public float AttackStrength = 1.0f;

	public float AutoTowerUpgradeTime = 2;
	public float AutoTowerUpgradeIncrease = 1.1f;
	public Bullet.BULLETUSER bullettype;

	public FloatingCounter FloatCounter;
	public GameObject RangeIndicator;
	public GameObject UpgradeIndicator;

	public bool Dropped = false;

	private Rigidbody2D rigidBody;

	private bool coolDown = false;

	private AudioSource audioSource;

	private GameObject UpgradeButton = null;
	private SpriteRenderer sr;


	void SetRange(float range)
	{
		Range = range;
		GetComponent<CircleCollider2D>().radius = Range;
		RangeIndicator.transform.localScale = new Vector3(0.406f * Range, 0.406f * Range, 1);
	}


	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody2D>();
		audioSource.volume = (SettingsManager.Instance.GetCurrentSettings().MasterSound / 100.0f);
		sr = GetComponent<SpriteRenderer>();
		statsmanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StatsManager>();
		SetRange(Range);
	}

	void OnEnable()
	{
		fireAmount = 0;
		Dropped = false;
		coolDown = false;
	}

	void OnDisable()
	{
		if (UpgradeButton != null)
			UpgradeButton.SetActive(false);
	}

	public void Drop()
	{
		Dropped = true;
		StartCoroutine(UpgradeCoolDown());

		if (UpgradeButton == null)
		{
			UpgradeButton = Instantiate(UpgradeIndicator, RectTransformUtility.WorldToScreenPoint(null, (transform.position)), Quaternion.identity, GameObject.FindWithTag("AdditionalUI").transform);
		}
		else
		{
			UpgradeButton.transform.position = RectTransformUtility.WorldToScreenPoint(null, (transform.position));
		}

		UpgradeButton.SetActive(false);

		UpgradeButton.GetComponent<Button>().onClick.RemoveAllListeners();
		UpgradeButton.GetComponent<Button>().onClick.AddListener(() => { UpgradeTower(); });
		RangeIndicator.SetActive(false);
	}

	// Firing Method to get a Bullet and Fire it
	int fireAmount = 0;
	private void Fire()
	{
		GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
		if (bullet != null && !coolDown && fireAmount++ > 0)
		{
			audioSource.Play();
			coolDown = true;
			bullet.transform.position = transform.position + transform.up * 1.2f;
			bullet.transform.rotation = transform.rotation;
			bullet.SetActive(true);

			var bulletScript = bullet.GetComponent<Bullet>();
			bulletScript.Speed = 20;
			bulletScript.Fire(AttackStrength);
			bulletScript.ShotBy = bullettype;
			/*
			var Text = Instantiate(FloatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("AdditionalUI").transform);
			Text.Show(CoolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y / 2);			
			*/


			StartCoroutine(FillIndicator());
			StartCoroutine(CoolDownCounter());
		}
	}

	private IEnumerator FillIndicator()
	{
		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.2f);
		float increaseBy = 0.8f / (CoolDownTime / 0.1f);
		while (sr.color.a < 1.0f)
		{
			yield return new WaitForSeconds(0.1f);
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + increaseBy);
		}
	}

	private IEnumerator UpgradeCoolDown()
	{
		yield return new WaitForSeconds(AutoTowerUpgradeTime);
		UpgradeButton.gameObject.SetActive(true);

	}

	private void UpgradeTower()
	{
		if (statsmanager.Money >= 20)
		{
			statsmanager.Money -= 20;
			SetRange(Range * AutoTowerUpgradeIncrease);
			CoolDownTime *= (1.0f - (AutoTowerUpgradeIncrease - 1.0f));
			StartCoroutine(UpgradeCoolDown());
			UpgradeButton.SetActive(false);
		}
	}


	// Cooldown Counter for Firing Bullets

	private IEnumerator CoolDownCounter()
	{
		yield return new WaitForSeconds(CoolDownTime);
		coolDown = false;
	}


	// Simple AI that targets the first Enemy it sees to increase the effectiveness!
	private GameObject currentTarget = null;

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
		foreach (var target in Focustargets.ToArray())
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
				rigidBody.rotation = destination.eulerAngles.z; // Quaternion.Slerp(transform.rotation, destination, Time.deltaTime * Smoothness).eulerAngles.z;

				Fire();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		IsInRange.Remove(collision.gameObject);
	}

}
