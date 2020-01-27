using Assets.Scripts.Path;
using Assets.Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("PreparationTime Screen")]
	public GameObject Preparation;
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

	void Start()
	{
		statsManager = GetComponent<StatsManager>();
		gameManager = GetComponent<GameManager>();

		Settings s = SettingsManager.Instance.GetCurrentSettings();
		Amount = s.WaveEnemyAmount * s.WaveAmount;
		WaveSize = s.WaveEnemyAmount;

		InitialAmount = Amount;
		StartCoroutine(SpawnWaves());
	}	

	private int CurrentWave = 1;

	IEnumerator SpawnWaves()
	{
		//yield return new WaitForSeconds(2);

		while (true)
		{

			StartCoroutine(PreparationTime());
			yield return new WaitForSeconds(10.9f);

			int scoreBefore = statsManager.Score;

			for (int i = 0; i < WaveSize; i++)
			{
				GameObject enemy = ObjectPooler.Instance.GetPooledObject("Enemy");

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

			CurrentWave ++;

			while(statsManager.Score< scoreBefore+ WaveSize) {
				Debug.Log(statsManager.Score);
				yield return new WaitForSeconds(0.5f);
			}

		}

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
