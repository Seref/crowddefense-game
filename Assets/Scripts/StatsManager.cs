using Assets.Scripts.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public StatsPanel StatsPanel;

	public int PlayTime = -1;

	private int mScore = -1;
	private int mWave = -1;
	private int mMoney = -1;
	

	private TextMeshProUGUI itemScore;
	private TextMeshProUGUI itemWave;
	private TextMeshProUGUI itemPlayTime;
	private TextMeshProUGUI itemMoney;

	private GameManager gameManager;


	public void Start()
	{
		itemScore = StatsPanel.AddItem();
		itemWave = StatsPanel.AddItem();
		itemPlayTime = StatsPanel.AddItem();
		itemMoney = StatsPanel.AddItem();

		Score = 0;
		PlayTime = 0;
		Wave = 0;
		Money = 100;

		StartCoroutine(Timer());

		gameManager = FindObjectOfType<GameManager>();
	}

	public int Money
	{
		get { return mMoney; }
		set
		{
			if (mMoney == value) return;
			mMoney = value;
			itemMoney.text = "Money: " + mMoney+ "$";
		}
	}

	private IEnumerator Timer()
	{
		while (true)
		{
			PlayTime++;
			itemPlayTime.text = "Time: " + PlayTime + "s";
			yield return new WaitForSeconds(1);
		}
	}

	public int Score
	{
		get { return mScore; }
		set
		{
			if (mScore == value) return;
			mScore = value;
			itemScore.text = "Score: " + mScore;
			if (gameManager != null)
				Wave = (int)(Math.Max(mScore, 1) / gameManager.enemySpawner.WaveSize);
		}
	}

	public int Wave
	{
		get { return mWave; }
		set
		{
			if (mWave == value) return;
			mWave = value;
			itemWave.text = "Wave: " + mWave;
		}
	}
}
