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
	public GameObject AutoTowerPrefab;
	public Button AutoSpawnButton;
	public FloatingCounter FloatingCounter;

	private bool canSpawn = true;
	private float time = 0.0f;

	public void Start()
	{
		AutoSpawnButton.onClick.AddListener(SpawnAutoTower);		
	}

	public void SpawnAutoTower()
	{
		if (Amount > 0 && canSpawn)
		{
			--Amount;
			canSpawn = false;
			AutoSpawnButton.interactable = false;
			StartCoroutine(CoolDown());

			var Text = Instantiate(FloatingCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("UI").transform);
			Text.Show(CoolDownTime, AutoSpawnButton.gameObject.transform, 0, true);
		}
	}

	private IEnumerator CoolDown()
	{
		time = CoolDownTime;
		while (time >= 0.0f)
		{
			yield return new WaitForSeconds(0.1f);
			time -= 0.1f;
		}
		AutoSpawnButton.interactable = true;
		canSpawn = true;
	}

	void Update()
	{

	}
	
}
