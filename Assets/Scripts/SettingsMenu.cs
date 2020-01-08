using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;

	public GameObject menuButtons;
	public TMPro.TextMeshProUGUI notSupportedBanner;


	[DllImport("__Internal")]
	private static extern string ReadSettings();

	[DllImport("__Internal")]
	private static extern void SaveSettings(string data);


	void Start()
	{
#if UNITY_WEBGL
		text.text = ReadSettings();
		SaveSettings("My Text is amazing" + DateTime.Now.ToShortTimeString());
#endif
	}

	private void Update()
	{
		if (!Application.isEditor)
			CheckSystemCompability();
	}

	private void CheckSystemCompability()
	{
		if (Application.isMobilePlatform)
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your Device is not supported!";

		}
		else if (Screen.width < 1280 || Screen.height < 720)
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your screen resolution (" + Screen.width + "x" + Screen.height + ") is below the minimum required 1280x720!\n";
		}
		else
		{
			menuButtons.SetActive(true);
			notSupportedBanner.gameObject.SetActive(false);
		}
	}

	public void LoadGame()
	{
		SceneManager.LoadScene("TowerDefence");
		DataLogger.Instance.LogStart(false);
	}

	public void LoadTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}	
}
