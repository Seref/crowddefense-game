﻿using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string clientID;
	public TMPro.TextMeshProUGUI text;

	public GameObject menuButtons;
	public TMPro.TextMeshProUGUI notSupportedBanner;


	[DllImport("__Internal")]
	private static extern void OnAppReady();

	void Start()
	{				
		SetClientID("Prototyp 2"); //TODO: Remove

		OnAppReady();
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
		this.clientID = clientID;
		text.text = clientID;
		DataLogger.Instance.SetUserName(clientID);
	}
}