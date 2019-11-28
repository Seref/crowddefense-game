﻿using Assets.Scripts.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("GameOver Screen")]
	public GameObject GameOverScreen;
	public TextMeshProUGUI Score;

	[Header("Multiplayer Values")]
	public TextMeshProUGUI connectionStatus;
	public GameObject multiplayerMenu;

	public TextMeshProUGUI address;
	public Host hostGame;

	private bool gameOver = false;

	void Start()
	{
		Application.runInBackground = true;
		Application.targetFrameRate = 60;
	}

	public void GameOver()
	{
		if (!gameOver)
		{
			Time.timeScale = 0f;
			gameOver = true;
			GameOverScreen.SetActive(true);
			var Stats = GetComponent<StatsManager>();
			var Text = "Killed Enemies\t" + Stats.Score + "\nSurvived Waves\t" + Stats.Wave + "\nSurvived Time\t" + Stats.PlayTime;
			Score.text = Text;
		}
	}

	public void Restart()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(1);
	}

	
	public void MultiplayerMenu()
	{
		if (multiplayerMenu.activeSelf)
		{
			Time.timeScale = 1f;
			multiplayerMenu.SetActive(false);
		}
		else
		{
			Time.timeScale = 0f;
			multiplayerMenu.SetActive(true);
		}
	}

	public void HostGame()
	{

		if (address.text != "")
			hostGame.address = address.text.Trim();
		else
			hostGame.address = "localhost:8000";


		hostGame.enabled = true;
		connectionStatus.text = "hosting";
	}

	public void JoinGame()
	{
		address.transform.parent = transform.parent;
		DontDestroyOnLoad(address);
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
