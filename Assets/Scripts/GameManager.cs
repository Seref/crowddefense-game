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

	private enum Windows { GAME, GAMEOVER, HELPMENU };
	private Windows currentWindow = Windows.GAME;

	public EnemySpawner enemySpawner;
	public AutoTowerSpawner autoTowerSpawner;
	public StatsManager statsManager;
	public BombSpawner bombSpawner;

	void Awake()
	{
		if (Application.isEditor)
			DataLogger.Instance.LogStart();

		Application.runInBackground = true;
		Application.targetFrameRate = 60;

		enemySpawner = GetComponent<EnemySpawner>();
		statsManager = GetComponent<StatsManager>();
		autoTowerSpawner = GetComponent<AutoTowerSpawner>();
		bombSpawner = GetComponent<BombSpawner>();

		enemySpawner.enabled = true;
		statsManager.enabled = true;
		autoTowerSpawner.enabled = true;
        ContinueGame();
        //bombSpawner.enabled = false;
    }

	void LateUpdate()
	{
		if (enemySpawner.Amount <= 0)
		{
			if (statsManager.Score >= enemySpawner.InitialAmount)
			{
				GameEnd(true);
			}
		}
	}

	public void GameEnd(bool isWin)
	{
		if (currentWindow.Equals(Windows.GAME))
		{
			GameObject.FindWithTag("AdditionalUI").SetActive(false);
			if (isWin)
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
			else
			{
				PauseGame();
				currentWindow = Windows.GAMEOVER;
				Title.text = "Game Over!";
				var Stats = GetComponent<StatsManager>();
				var Text = "Killed Enemies\t" + Stats.Score + "\nSurvived Waves\t" + Stats.Wave + "\nSurvived Time\t" + Stats.PlayTime;
				Score.text = Text;
				DataLogger.Instance.LogEnd(false, Stats.Score, Stats.Wave, Stats.PlayTime);
				GameOverScreen.SetActive(true);
			}
		}
	}

	public void Restart()
	{
		currentWindow = Windows.GAME;
		ContinueGame();
		SceneManager.LoadScene(1);
		DataLogger.Instance.LogStart(true);
	}

	public void BackToTheMenu()
	{
        ContinueGame();
        SceneManager.LoadScene("MainMenu");
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

	private void PauseGame()
	{
		Time.timeScale = 0f;
	}

	private void ContinueGame()
	{
		Time.timeScale = 1f;
	}

}
