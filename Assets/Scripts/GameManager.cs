using Assets.Scripts.Multiplayer;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("HUD Values")]
	public TextMeshProUGUI dataText;

	[Header("Multiplayer Values")]
	public TextMeshProUGUI connectionStatus;
	public GameObject multiplayerMenu;

	public TextMeshProUGUI address;
	public Host hostGame;

	//Game Data
	private int m_Score = 0;
	public int Score
	{
		get { return m_Score; }
		set
		{
			if (m_Score == value) return;
			UpdateText();
			m_Score = value;
		}
	}

	private int m_Wave = 0;
	public int Wave
	{
		get { return m_Wave; }
		set
		{
			if (m_Wave == value) return;
			UpdateText();
			m_Wave = value;
		}
	}

	private int PlayTime = 0;

	void Start()
	{
		StartCoroutine(Timer());
	}


	private IEnumerator Timer()
	{
		while (true)
		{
			PlayTime++;
			UpdateText();
			yield return new WaitForSeconds(1);
		}
	}

	private void UpdateText()
	{
		dataText.text =
			"Enemies killed: " + Score + "\n" +
			"Wave: " + Wave + "\n" +
			"Time: " + PlayTime + "s";
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
