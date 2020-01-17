using System.Collections;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{


	[Header("Variables")]
	public float Randomness = 0.5f;
	public int Time = 15;

	public GameObject Bomb;
	public GameObject Explosion;	

	//[Header("Dependencies")]
	private int Counter = 0;
	private GameObject currentBomb;

	private Coroutine startCoroutine;

	private StatsManager statsManager;
	private AutoTowerSpawner autoTowerSpawner;
	private EnemySpawner enemySpawner;
	private GameObject additionalLayer;

    private bool bombExist = false;

	void Start()
	{        

            Counter = Time;

            startCoroutine = StartCoroutine(SpawnTimer());
            statsManager = GetComponent<StatsManager>();
            autoTowerSpawner = GetComponent<AutoTowerSpawner>();
            enemySpawner = GetComponent<EnemySpawner>();
            additionalLayer = GameObject.FindGameObjectWithTag("Additional");
        
	}

	private IEnumerator SpawnTimer()
	{
		yield return new WaitForSeconds(Time);
		Gamble();			
	}

	private void Gamble()
	{
		StopCoroutine(startCoroutine);
		if (Random.value < Randomness)
		{
			SpawnBomb();
		}
		else
		{
			startCoroutine = StartCoroutine(SpawnTimer());
		}
	}

	private Coroutine deleteTimer;

	private void SpawnBomb()
	{
		Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.1f,0.9f), Random.Range(0.1f, 0.9f)));
		currentBomb = Instantiate(Bomb, randomPositionOnScreen, Quaternion.identity, additionalLayer.transform);
		deleteTimer = StartCoroutine(DeleteTimer());
		currentBomb.GetComponent<Bomb>().callback = ExplodeEverything;
	}

	private IEnumerator DeleteTimer()
	{
		yield return new WaitForSeconds(3);
		RemoveBomb();
		startCoroutine = StartCoroutine(SpawnTimer());
	}

	private void ExplodeEverything()
	{		
		StopCoroutine(deleteTimer);
		RemoveBomb();
		var Enemies = GameObject.Find("Enemy");
		var AutoTower = GameObject.Find("AutoTower");

		foreach (Transform transformChild in AutoTower.transform)
		{
			var child = transformChild.gameObject;
			if (child.activeSelf)
			{
				if (child.GetComponent<Renderer>().isVisible)
				{
					var position = child.transform.position;
					child.SetActive(false);					
					var e = Instantiate(Explosion, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), additionalLayer.transform);
					e.transform.localScale = new Vector2(Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f));
					autoTowerSpawner.RefillAutoTower();
				}
			}
		}

		foreach (Transform transformChild in Enemies.transform)
		{
			var child = transformChild.gameObject;
			if (child.activeSelf)
			{
				if (child.GetComponent<Renderer>().isVisible)
				{					
					var position = child.transform.position;
					Enemy enemy = child.GetComponent<Enemy>();
					enemy.Die();
					var e = Instantiate(Explosion, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), additionalLayer.transform);
					e.transform.localScale = new Vector2(Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f));
					
				}
			}
		}

		startCoroutine = StartCoroutine(SpawnTimer());
	}

	private void RemoveBomb()
	{
		Destroy(currentBomb);
		currentBomb = null;
	}
}
