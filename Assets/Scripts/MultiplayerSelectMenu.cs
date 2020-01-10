using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerSelectMenu : MonoBehaviour
{
	public void HostGame()
	{
		SceneManager.LoadScene("HostTowerDefence");
	}

	public void JoinGame()
	{
		SceneManager.LoadScene("ClientTowerDefence");
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
