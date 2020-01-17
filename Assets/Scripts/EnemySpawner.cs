﻿using Assets.Scripts.Path;
using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
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
		Amount = s.WaveEnemyAmount*s.WaveAmount;
		WaveSize = s.WaveEnemyAmount;

		InitialAmount = Amount;
		StartCoroutine(SpawnWaves());
	}

	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(2);

		while (true)
		{			

			for (int i = 0; i <WaveSize; i++)
			{				
				GameObject enemy = ObjectPooler.Instance.GetPooledObject("Enemy");

				if (enemy != null)
				{
					enemy.transform.position = new Vector3(-10f + i, 15f, 0);
					enemy.transform.rotation = Quaternion.identity;
					enemy.SetActive(true);
					enemy.GetComponent<Enemy>().StartPath(PathCreator.path, statsManager);
				}

				if (--Amount <= 0)
					yield break;
			}			

			yield return new WaitForSeconds(Timer);
		}

	}



}
