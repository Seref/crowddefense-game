using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastAutoTowerSpawner : MonoBehaviour
{

	[Header("Variables")]
	public int Cost = 20;

	[Header("Dependencies")]
	public Button AutoSpawnButton;
	public FloatingCounter FloatingCounter;
	public GameObject NotPlaceable;
	public GameObject Placeable;

	private GameObject additionalLayer;
	private GameObject additionalLayerUI;

	private bool canSpawn = true;	

	private bool towerDropped = true;
	private GameObject AutoTower;
	private GameObject Cross;
	private StatsManager statsManager;

	public void Start()
	{
		additionalLayer = GameObject.FindGameObjectWithTag("Additional");
		additionalLayerUI = GameObject.FindGameObjectWithTag("AdditionalUI");

		statsManager = GetComponent<GameManager>().statsManager;

		Settings s = SettingsManager.Instance.GetCurrentSettings();
		Cost = s.FastAutoTowerBuildCost;

		AutoSpawnButton.onClick.AddListener(SpawnAutoTower);
		Cross = Instantiate(NotPlaceable, new Vector3(0, 0, 10), Quaternion.identity, additionalLayer.transform);
		Placeable.transform.position = new Vector3(Placeable.transform.position.x, Placeable.transform.position.y, 8);
		Placeable.SetActive(false);
		Cross.SetActive(false);
	}

	public void RefillAutoTower()
	{		
		AutoSpawnButton.interactable = true;
		canSpawn = true;		
	}

	public void SpawnAutoTower()
	{
		if (statsManager.Money >= Cost && towerDropped)
		{
			AutoTower = ObjectPooler.Instance.GetPooledObject("AutoTowerWhite");
			if (AutoTower != null)
			{
				statsManager.Money -= Cost;
				AutoTower.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				AutoTower.transform.rotation = Quaternion.identity;
				AutoTower.layer = LayerMask.NameToLayer("UI");

				AutoTower.SetActive(true);

				Settings s = SettingsManager.Instance.GetCurrentSettings();
				AutoTower.GetComponent<AutoTower>().CoolDownTime = s.FastAutoTowerFireCooldown;

				towerDropped = false;				
				Placeable.SetActive(true);
				AutoSpawnButton.interactable = false;

			}
		}
	}

	void Update()
	{
		if (!towerDropped && AutoTower != null)
		{
			Vector3 p1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			bool canPlace = true;

			RaycastHit2D hit = Physics2D.CircleCast(p1, 0.1f, Vector3.forward, 0f, 1 << LayerMask.NameToLayer("Default"));

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
					towerDropped = true;
					AutoTower.layer = LayerMask.NameToLayer("AutoTower");
					AutoTower.GetComponent<AutoTower>().Dropped = true;
					AutoTower = null;
					Placeable.SetActive(false);
				}
			}
			else
			{
				var newPosition = AutoTower.transform.position;
				Cross.transform.position = new Vector3(newPosition.x, newPosition.y, -2);
				Cross.SetActive(true);
			}
		}
		else
		{
			if (statsManager.Money >= Cost)
				AutoSpawnButton.interactable = true;
			else
				AutoSpawnButton.interactable = false;
		}
	}


}
