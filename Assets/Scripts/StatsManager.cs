using System.Collections;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public StatsPanel StatsPanel;	

	private int m_Score = -1;
	private int m_Wave = -1;
	public int PlayTime = -1;

	private TextMeshProUGUI itemScore;
	private TextMeshProUGUI itemWave;
	private TextMeshProUGUI itemPlayTime;


	void Start()
	{
		itemScore = StatsPanel.AddItem();
		itemWave = StatsPanel.AddItem();
		itemPlayTime = StatsPanel.AddItem();

		Score = 0;
		PlayTime = 0;
		Wave = 0;

		StartCoroutine(Timer());
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
		get { return m_Score; }
		set
		{
			if (m_Score == value) return;
			m_Score = value;
			itemScore.text = "Score: " + m_Score;
		}
	}

	public int Wave
	{
		get { return m_Wave; }
		set
		{
			if (m_Wave == value) return;
			m_Wave = value;
			itemWave.text = "Wave: " + m_Wave;
		}
	}

}
