using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string clientID;
	public TMPro.TextMeshProUGUI text;
	public TMPro.TextMeshProUGUI VersionText;	
	private bool userNameSet = false;

	[DllImport("__Internal")]
	private static extern void OnAppReady();

	void Start()
	{		
		if (!Application.isEditor)
			OnAppReady();
		VersionText.text = "Version " + SettingsManager.VERSIONNAME;
	}

	public void LoadGame()
	{
		SceneManager.LoadScene("TowerDefence");
		DataLogger.Instance.LogStart(false);
	}

	public void LoadMultiplayerGame()
	{
		SceneManager.LoadScene("MultiplayerMenu");
	}

	public void LoadTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}

	public void LoadSettings()
	{
		SceneManager.LoadScene("SettingsMenu");
	}


	public void SetClientID(string clientID)
	{
		if (!userNameSet)
		{
			userNameSet = true;
			this.clientID = clientID;
			text.text = clientID;
			DataLogger.Instance.SetUserName(clientID);			
		}
	}
}
