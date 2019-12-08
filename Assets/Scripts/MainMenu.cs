using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string clientID;
	public TMPro.TextMeshProUGUI text;

	void Start()
	{
		Application.ExternalEval("OnAppReady();");
	}

	public void LoadGame()
	{
		SceneManager.LoadScene("TowerDefence");
	}

	public void LoadTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}

	public void SetClientID(string clientID)
	{
		this.clientID = clientID;
		text.text = clientID;
	}
}
