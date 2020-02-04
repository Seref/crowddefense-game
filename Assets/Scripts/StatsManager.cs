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
	private int mLives = -1;
	private int mOutpostLives = -1;
	public int moneyEarned = 0;


	

	private TextMeshProUGUI itemScore;
	private TextMeshProUGUI itemWave;
	private TextMeshProUGUI itemPlayTime;
	private TextMeshProUGUI itemMoney;
	private TextMeshProUGUI itemLives;
	private TextMeshProUGUI itemOutPostLives;

	private GameManager gameManager;


	public void Start()
	{
		itemScore = StatsPanel.AddItem();
		itemWave = StatsPanel.AddItem();
		itemPlayTime = StatsPanel.AddItem();
		itemMoney = StatsPanel.AddItem();
		itemLives = StatsPanel.AddItem();
		itemOutPostLives = StatsPanel.AddItem();

		Score = 0;
		PlayTime = 0;
		Wave = 0;
		Money = 100;
		moneyEarned = 0;
		Lives = 3;
		OutpostLives = 3;

		StartCoroutine(Timer());

		gameManager = FindObjectOfType<GameManager>();
	}

	public int OutpostLives
	{
		get { return mOutpostLives; }
		set
		{
			if (mOutpostLives == value) return;
			mOutpostLives = value;
			itemOutPostLives.text = "OutPost Lives: " + mOutpostLives;
		}
	}

	public int Lives {
		get { return mLives; }
		set
		{
			if (mLives == value) return;
			mLives = value;
			itemLives.text = "Lives: " + mLives ;
		}
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
			Money += 30;
			moneyEarned += 30;
		}
	}
}
