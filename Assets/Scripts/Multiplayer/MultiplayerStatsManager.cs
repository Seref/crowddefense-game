using Assets.Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class MultiplayerStatsManager : MonoBehaviour
{
	public StatsPanel StatsPanel;

	public int PlayTime = -1;

	private int mHostScore = -1;
	private int mClientScore = -1;
	private int mWave = -1;

	private TextMeshProUGUI itemYourScore;
	private TextMeshProUGUI itemEnemyScore;
	private TextMeshProUGUI itemWave;
	private TextMeshProUGUI itemPlayTime;

    private GameManager gameManager;


    void Start()
	{
		itemYourScore = StatsPanel.AddItem();
		itemEnemyScore = StatsPanel.AddItem();
		itemWave = StatsPanel.AddItem();
		itemPlayTime = StatsPanel.AddItem();


		HostScore = 0;
		ClientScore = 0;
		PlayTime = 0;
		Wave = 0;

		StartCoroutine(Timer());

        gameManager = FindObjectOfType<GameManager>();
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

	public int HostScore
	{
		get { return mHostScore; }
		set
		{
			if (mHostScore == value) return;
			mHostScore = value;
			itemYourScore.text = "Host Score: " + mHostScore;            
		}
	}

	public int ClientScore
	{
		get { return mClientScore; }
		set
		{
			if (mClientScore == value) return;
			mClientScore = value;
			itemEnemyScore.text = "Client Score: " + mClientScore;            
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
