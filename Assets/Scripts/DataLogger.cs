using System;
using UnityEngine;

public class Log
{
	public string userName;
	public DateTime timeStamp;
	public string version;
	public int score;
	public int waveSurvived;
	public int secondsSurvived;
	public bool win;
	public bool restartedRound;
	public int tutorialPressed;
}

public class DataLogger : MonoBehaviour
{
	private static DataLogger _instance;

	public static DataLogger Instance
	{
		get { return _instance; }
	}

	private string userName;

	private Log currentRound;
	
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(this.gameObject);
	}
	public void SetUserName(string userName)
	{
		this.userName = userName;
	}

	public void LogStart(bool restartedRound = false)
	{
		currentRound = new Log
		{
			userName = userName,
			timeStamp = DateTime.UtcNow,
			restartedRound = restartedRound,
			version = Application.version
		};
	}

	public void LogEnd(bool win, int score, int wavesSurvived, int secondsSurvived)
	{
		if (currentRound != null)
		{
			currentRound.score = score;
			currentRound.win = win;
			currentRound.waveSurvived = wavesSurvived;
			currentRound.secondsSurvived = secondsSurvived;
			SendData(currentRound);			
		}
	}

	public void LogTutorialPressed()
	{
		if (currentRound != null)
			currentRound.tutorialPressed = currentRound.tutorialPressed + 1;
	}

	public void SendData(Log log)
	{
		var logData = JsonUtility.ToJson(log).ToString();
		Console.WriteLine(logData);
		Debug.Log(logData);
	}
}