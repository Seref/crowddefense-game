using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void LoadGame()
	{
		SceneManager.LoadScene(1);
	}

	public void LoadTutorial()
	{
		SceneManager.LoadScene(0);
	}
}
