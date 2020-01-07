using UnityEngine;

public class DataLogger : MonoBehaviour
{
	public static DataLogger Instance { get; set; }

	private string userName;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

		}
		else
		{
			Destroy(gameObject);
		}
	}
	public void SetUserName(string userName)
	{
		this.userName = userName;
	}

	public void LogData(string Data)
	{
		Debug.Log("Data Logged to Server: " + Data);
	}
}
