using System.Collections;
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{
	public PathCreator pathCreator;
	public int count;
	public float timer;
	public int waveSize = 5;


	GameManager2D gameManager2D;
	void Start()
	{
		gameManager2D = GetComponent<GameManager2D>();
		StartCoroutine(SpawnWaves());
	}

	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(2);

		var gameManager = GetComponent<GameManager2D>();
		while (true)
		{
			gameManager.Wave++;

			for (int i = 0; i < 5; i++)
			{
				GameObject enemy = ObjectPooler.Instance.GetPooledObject("Enemy");
				if (enemy != null)
				{
					enemy.transform.position = new Vector3(-10f + i, 10f, 0);
					enemy.transform.rotation = Quaternion.identity;
					enemy.SetActive(true);
					enemy.GetComponent<Enemy2D>().StartPath(pathCreator.path, gameManager);
				}

				if (count-- <= 0) yield break;				
			}
			yield return new WaitForSeconds(timer);
		}
		
	}



}
