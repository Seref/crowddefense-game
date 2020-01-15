using System;
using System.Runtime.InteropServices;
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

public class DataLogger
{
	public static bool DEBUG = true;

	[DllImport("__Internal")]
	private static extern void PushGameData(string data);

	public static readonly DataLogger Instance = new DataLogger();

	private string currentVersion = Application.version;

	private DataLogger() { }

	private string userName;

	private Log currentRound;

	public void SetUserName(string userName)
	{
		this.userName = userName;
	}

	public string GetUserName()
	{
		return userName;
	}

	public void LogStart(bool restartedRound = false)
	{
		currentRound = new Log
		{
			userName = userName,
			timeStamp = DateTime.UtcNow,
			restartedRound = restartedRound,
			version = currentVersion
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

	private string ConvertBool(bool b)
	{
		return (b ? "true" : "false");
	}

	public void SendData(Log log)
	{
		//(string username, string score, string wavesurvived, string secoondssurvived, string win, string restartround, string tutorialpressed)
		var logData = JsonUtility.ToJson(log).ToString();
		Console.WriteLine(logData);

		var send = "Username=" + log.userName + "&" +
			"Score=" + log.score.ToString() + "&" +
			"WaveSurvived=" + log.waveSurvived.ToString() + "&" +
			"SecondsSurvived=" + log.secondsSurvived.ToString() + "&" +
			"Win=" + ConvertBool(log.win) + "&" +
			"RestartedRound=" + ConvertBool(log.restartedRound) + "&" +
			"TutorialPressed=" + log.tutorialPressed.ToString();

		if (!DEBUG)
			PushGameData(send);

		Debug.Log(logData);
	}
}