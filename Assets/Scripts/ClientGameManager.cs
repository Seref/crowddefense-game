using Assets.Scripts.Multiplayer;
using Assets.Scripts.Multiplayer.Client;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : MonoBehaviour
{
	[Header("GameOver Screen")]
	public GameObject GameOverScreen;
	public TextMeshProUGUI Title;
	public TextMeshProUGUI Score;

	[Header("Help Screen")]
	public GameObject HelpMenuScreen;

	[Header("Multiplayer Screen")]
	public GameObject HostLobbyMenuScreen;


	private enum Windows { GAME, GAMEOVER, HELPMENU };
	private Windows currentWindow = Windows.GAME;

	public ClientManager clientManager;

	void Awake()
	{
		if (Application.isEditor)
			DataLogger.Instance.LogStart();

		Application.runInBackground = true;
		Application.targetFrameRate = 60;

		clientManager = GetComponent<ClientManager>();
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

	public void OpenHelpMenu()
	{
		if (currentWindow.Equals(Windows.GAME))
		{

			DataLogger.Instance.LogTutorialPressed();

			HelpMenuScreen.SetActive(true);
			currentWindow = Windows.HELPMENU;			
		}
	}

	public void CloseHelpMenu()
	{
		if (currentWindow.Equals(Windows.HELPMENU))
		{
			HelpMenuScreen.SetActive(false);
			currentWindow = Windows.GAME;			
		}
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
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
