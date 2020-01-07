using Assets.Scripts.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("GameOver Screen")]
	public GameObject GameOverScreen;
	public TextMeshProUGUI Title;
	public TextMeshProUGUI Score;

	[Header("Help Screen")]
	public GameObject HelpMenuScreen;


	[Header("Multiplayer Values")]
	public TextMeshProUGUI connectionStatus;
	public GameObject multiplayerMenu;

	public TextMeshProUGUI address;
	public Host hostGame;


	private enum Windows { GAME, GAMEOVER, HELPMENU };
	private Windows currentWindow = Windows.GAME;

	private EnemySpawner enemySpawner;
	private StatsManager statsManager;

	void Start()
	{
		if (Application.isEditor)
			DataLogger.Instance.LogStart();

		Application.runInBackground = true;
		Application.targetFrameRate = 60;

		enemySpawner = GetComponent<EnemySpawner>();
		statsManager = GetComponent<StatsManager>();
	}

	void LateUpdate()
	{
		if (enemySpawner.Amount <= 0)
		{
			if (statsManager.Score >= enemySpawner.InitialAmount)
			{
				GameWin();
			}
		}
	}

	public void GameOver()
	{
		if (currentWindow.Equals(Windows.GAME))
		{
			PauseGame();
			currentWindow = Windows.GAMEOVER;
			var Stats = GetComponent<StatsManager>();
			var Text = "Killed Enemies\t" + Stats.Score + "\nSurvived Waves\t" + Stats.Wave + "\nSurvived Time\t" + Stats.PlayTime;
			Score.text = Text;
			DataLogger.Instance.LogEnd(false, Stats.Score, Stats.Wave, Stats.PlayTime);
			GameOverScreen.SetActive(true);
		}
	}

	public void GameWin()
	{
		if (currentWindow.Equals(Windows.GAME))
		{
			PauseGame();
			currentWindow = Windows.GAMEOVER;
			Title.text = "You Won!";
			var Stats = GetComponent<StatsManager>();
			var Text = "Killed Enemies\t" + Stats.Score + "\nSurvived Waves\t" + Stats.Wave + "\nSurvived Time\t" + Stats.PlayTime;
			Score.text = Text;
			DataLogger.Instance.LogEnd(true, Stats.Score, Stats.Wave, Stats.PlayTime);
			GameOverScreen.SetActive(true);
		}
	}

	public void Restart()
	{
		currentWindow = Windows.GAME;
		ContinueGame();
		SceneManager.LoadScene(1);
		DataLogger.Instance.LogStart(true);
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

	public void OpenHelpMenu()
	{
		if (currentWindow.Equals(Windows.GAME))
		{

			DataLogger.Instance.LogTutorialPressed();

			HelpMenuScreen.SetActive(true);
			currentWindow = Windows.HELPMENU;
			PauseGame();
		}
	}

	public void CloseHelpMenu()
	{
		if (currentWindow.Equals(Windows.HELPMENU))
		{
			HelpMenuScreen.SetActive(false);
			currentWindow = Windows.GAME;
			ContinueGame();
		}
	}

	private bool GamePaused = false;
	private void PauseGame()
	{
		GamePaused = true;
		Time.timeScale = 0f;
	}
	private void ContinueGame()
	{
		GamePaused = false;
		Time.timeScale = 1f;
	}
}
