using Assets.Scripts.Path;
using Assets.Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
	[Header("PreparationTime Screen")]
	public GameObject Preparation;
	public GameObject WaveWon;
	public TextMeshProUGUI PrepTime;

	[Header("Variables")]
	public int Amount;
	public float Timer;
	public int WaveSize = 5;
	public int InitialAmount;

	[Header("Dependencies")]
	public PathCreator PathCreator;
	public FloatingCounter FloatCounter;

	private StatsManager statsManager;
	private GameManager gameManager;

	private GameObject UpgradeButtons;

	void Start()
	{
		statsManager = GetComponent<StatsManager>();
		gameManager = GetComponent<GameManager>();

		Settings s = SettingsManager.Instance.GetCurrentSettings();
		Amount = s.WaveEnemyAmount * s.WaveAmount;
		WaveSize = s.WaveEnemyAmount;
		UpgradeButtons = gameManager.AdditionalUpgrade;
		InitialAmount = Amount;
		StartCoroutine(SpawnWaves());
	}

	private int CurrentWave = 1;

	IEnumerator SpawnWaves()
	{
		//yield return new WaitForSeconds(2);
		while (true)
		{

			if(CurrentWave % 5 == 1 && CurrentWave>1) { 
				var Message = Instantiate(WaveWon, new Vector3(0, 0, 0), Quaternion.identity);
				var e = Message.GetComponentInChildren<ManagedWave>();
				e.PlayAnimation("Wave " + (CurrentWave - 1) + " cleared!");
				yield return new WaitForSeconds(1.4f);
				Destroy(Message);
			}
			
			//Preparation Time
			StartCoroutine(PreparationTime());
			UpgradeButtons.GetComponent<Canvas>().enabled = true;
			UpgradeButtons.GetComponent<Canvas>().sortingLayerName = "Default";

			UpgradeButtons.GetComponent<GraphicRaycaster>().enabled = true;
			yield return new WaitForSeconds(10.9f);

			UpgradeButtons.GetComponent<GraphicRaycaster>().enabled = false;
			UpgradeButtons.GetComponent<Canvas>().sortingLayerName = "BehindBackground";
			UpgradeButtons.GetComponent<Canvas>().enabled = false;
			int scoreBefore = statsManager.Score;

			for (int i = 0; i < WaveSize; i++)
			{
				GameObject enemy = null;
				if (Gamble(0.125f))
				{
					enemy = ObjectPooler.Instance.GetPooledObject("TankyEnemy");
				}
				else
				{
					enemy = ObjectPooler.Instance.GetPooledObject("Enemy");
				}

				if (enemy != null)
				{
					enemy.transform.position = new Vector3(-10f + i, 15f, 0);
					enemy.transform.rotation = Quaternion.identity;
					enemy.SetActive(true);

					var enemyScript = enemy.GetComponent<Enemy>();
					enemyScript.StartPath(PathCreator.path, statsManager);
					enemyScript.SetHealth(CurrentWave);

				}

				if (--Amount <= 0)
					yield break;


			}

			CurrentWave++;

			while (statsManager.Score < scoreBefore + WaveSize)
			{
				yield return new WaitForSeconds(0.5f);
			}

		}

	}

	private bool Gamble(float bid)
	{
		return (Random.value < bid);
	}

	private IEnumerator PreparationTime()
	{
		float time = 10f;
		Preparation.SetActive(true);
		while (time >= 0.0f)
		{
			yield return new WaitForSeconds(0.1f);
			time -= 0.1f;
			PrepTime.text = time.ToString("F1") + "s";
		}
		Preparation.SetActive(false);
	}

}
