using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OneUpSpawner : MonoBehaviour
{


	[Header("Variables")]
	public float Randomness = 0.3f;
	public int Time = 60;

	public GameObject OneUp;

	public Player player;

	private GameObject currenteOneUp;

	//[Header("Dependencies")]
	private int Counter = 0;

	private Coroutine startCoroutine;

	private StatsManager statsManager;
	private AutoTowerSpawner autoTowerSpawner;
	private EnemySpawner enemySpawner;
	private GameObject additionalLayer;


	void Start()
	{
		Counter = Time;		
		statsManager = GetComponent<StatsManager>();
		autoTowerSpawner = GetComponent<AutoTowerSpawner>();
		enemySpawner = GetComponent<EnemySpawner>();
		additionalLayer = GameObject.FindGameObjectWithTag("AdditionalUI");
		startCoroutine = StartCoroutine(SpawnTimer());
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
				
		currenteOneUp = Instantiate(OneUp, Vector3.zero, Quaternion.identity, additionalLayer.transform);
		var panel = additionalLayer.GetComponent<RectTransform>();
		currenteOneUp.GetComponent<RectTransform>().anchoredPosition = GetBottomLeftCorner(panel) - new Vector3(Random.Range(0, panel.rect.x), Random.Range(0, panel.rect.y), 0);

		deleteTimer = StartCoroutine(DeleteTimer());		
		currenteOneUp.GetComponent<Button>().onClick.RemoveAllListeners();
		currenteOneUp.GetComponent<Button>().onClick.AddListener(() => { player.Health++; RemoveOneUp(); });
	}

	private IEnumerator DeleteTimer()
	{
		yield return new WaitForSeconds(3);
		RemoveOneUp();
		startCoroutine = StartCoroutine(SpawnTimer());
	}

	private void RemoveOneUp()
	{
		StopCoroutine(deleteTimer);
		if (currenteOneUp != null)
		{
			Destroy(currenteOneUp);
			currenteOneUp = null;
		}
		startCoroutine = StartCoroutine(SpawnTimer());
	}

	Vector3 GetBottomLeftCorner(RectTransform rt)
	{
		Vector3[] v = new Vector3[4];
		rt.GetWorldCorners(v);
		return v[0];
	}
}
