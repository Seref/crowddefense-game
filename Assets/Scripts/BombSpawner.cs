using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombSpawner : MonoBehaviour
{

	[Header("Variables")]
	public int Cost = 100;
	public int Time = 120;


	[Header("Dependencies")]
	public Button AutoSpawnButton;
	public FloatingCounter FloatingCounter;

	public GameObject Bomb;
	public GameObject Explosion;

	[Header("Dependencies")]

	private Coroutine startCoroutine;

	private StatsManager statsManager;
	private AutoTowerSpawner autoTowerSpawner;
	private EnemySpawner enemySpawner;
	private GameObject additionalLayerUI;
	private GameObject additionalLayer;

	private bool canSpawn = true;

	void Start()
	{
		AutoSpawnButton.onClick.AddListener(KillEverything);
		statsManager = GetComponent<StatsManager>();
		autoTowerSpawner = GetComponent<AutoTowerSpawner>();
		enemySpawner = GetComponent<EnemySpawner>();
		additionalLayerUI = GameObject.FindGameObjectWithTag("AdditionalUI");
		additionalLayer = GameObject.FindGameObjectWithTag("Additional");
	}

	private IEnumerator SpawnTimer()
	{
		var Text = Instantiate(FloatingCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, additionalLayerUI.transform);
		Text.Show(Time + 1, AutoSpawnButton.gameObject.transform, 0, true);

		canSpawn = false;
		yield return new WaitForSeconds(Time);
		canSpawn = true;
	}

	private void RemoveAutoTower(GameObject group)
	{
		foreach (Transform transformChild in group.transform)
		{
			var child = transformChild.gameObject;
			if (child.activeSelf)
			{				
					var position = child.transform.position;
					child.SetActive(false);
					var e = Instantiate(Explosion, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), additionalLayer.transform);
					var scale = Random.Range(0.1f, 0.6f);
					e.transform.localScale = new Vector2(scale, scale);				
			}
		}
	}

	private void KillEnemies(GameObject Group)
	{
		foreach (Transform transformChild in Group.transform)
		{
			var child = transformChild.gameObject;
			if (child.activeSelf)
			{

				var position = child.transform.position;
				Enemy enemy = child.GetComponent<Enemy>();
				enemy.Die();				
				var e = Instantiate(Explosion, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), additionalLayer.transform);
				var scale = Random.Range(0.1f, 0.6f);
				e.transform.localScale = new Vector2(scale, scale);

			}
		}
	}

	void Update()
	{
		if (statsManager.Money >= Cost && canSpawn)
			AutoSpawnButton.interactable = true;
		else
			AutoSpawnButton.interactable = false;
	}

	private void KillEverything()
	{
		if (statsManager.Money >= Cost && canSpawn)
		{
			AutoSpawnButton.interactable = false;
			statsManager.Money -= Cost;

			var Enemies = GameObject.Find("Enemy");
			var Enemies2 = GameObject.Find("TankyEnemy");
			var AutoTower = GameObject.Find("AutoTower");
			var AutoTower1 = GameObject.Find("AutoTowerWhite");
			var AutoTower2 = GameObject.Find("AutoTowerYellow");

			RemoveAutoTower(AutoTower);
			RemoveAutoTower(AutoTower1);
			RemoveAutoTower(AutoTower2);

			KillEnemies(Enemies);
			KillEnemies(Enemies2);

			startCoroutine = StartCoroutine(SpawnTimer());
		}
	}
}
