using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoTowerSpawner : MonoBehaviour
{

	[Header("Variables")]
	public float CoolDownTime = 10.0f;
	public int Amount = 3;

	[Header("Dependencies")]
	public Button AutoSpawnButton;
	public FloatingCounter FloatingCounter;
	public GameObject NotPlaceable;
	public GameObject Additional;

	private bool canSpawn = true;
	private float time = 0.0f;

	private bool towerDropped = true;
	private GameObject AutoTower;
	private GameObject Cross;


	public void Start()
	{
		Settings s = SettingsManager.Instance.GetCurrentSettings();
		Amount = s.AutoTowerAmount;
		CoolDownTime = s.AutoTowerBuildCooldown;

		AutoSpawnButton.onClick.AddListener(SpawnAutoTower);
		Cross = Instantiate(NotPlaceable, new Vector3(0, 0, 10), Quaternion.identity, Additional.transform);
		Cross.SetActive(false);
	}

	public void RefillAutoTower()
	{
		Amount++;
		if (!isCoolingDown)
		{
			AutoSpawnButton.interactable = true;
			canSpawn = true;
		}

	}

	public void SpawnAutoTower()
	{
		if (Amount > 0 && canSpawn)
		{
			--Amount;

			AutoTower = ObjectPooler.Instance.GetPooledObject("AutoTower");
			if (AutoTower != null)
			{
				AutoTower.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				AutoTower.transform.rotation = Quaternion.identity;
				AutoTower.layer = LayerMask.NameToLayer("UI");
				AutoTower.SetActive(true);

				towerDropped = true;
				canSpawn = false;
				AutoSpawnButton.interactable = false;

				StartCoroutine(CoolDown());
				var Text = Instantiate(FloatingCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UIEffects").transform);
				Text.Show(CoolDownTime, AutoSpawnButton.gameObject.transform, 0, true);
			}
		}
	}

	private bool isCoolingDown = false;
	private IEnumerator CoolDown()
	{
		time = CoolDownTime;
		isCoolingDown = true;
		while (time >= 0.0f)
		{
			yield return new WaitForSeconds(0.1f);
			time -= 0.1f;
		}
		isCoolingDown = false;
		if (Amount > 0)
		{
			AutoSpawnButton.interactable = true;
			canSpawn = true;
		}
	}

	void Update()
	{
		if (towerDropped && AutoTower != null)
		{
			Vector3 p1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			bool canPlace = true;

			RaycastHit2D hit = Physics2D.CircleCast(p1, 1f, Vector3.forward, 0f, 1 << 0);

			if (hit.collider != null)
			{
				if (hit.collider.CompareTag("Path") || hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Bullet"))
				{
					canPlace = false;
				}
			}

			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			AutoTower.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

			if (!EventSystem.current.IsPointerOverGameObject() && canPlace)
			{
				Cross.transform.position = new Vector3(-1000, -1000, 5);
				Cross.SetActive(false);

				if (Input.GetButtonDown("Fire1"))
				{
					towerDropped = false;
					AutoTower.layer = LayerMask.NameToLayer("Default");
					AutoTower.GetComponent<AutoTower>().Dropped = true;
				}
			}
			else
			{
				var newPosition = AutoTower.transform.position;
				Cross.transform.position = new Vector3(newPosition.x, newPosition.y, -2);
				Cross.SetActive(true);
			}
		}
	}


}
