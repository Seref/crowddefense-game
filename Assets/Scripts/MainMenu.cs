using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void LoadGame()
	{
		SceneManager.LoadScene("TowerDefence");
	}

	public void LoadTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}
}
