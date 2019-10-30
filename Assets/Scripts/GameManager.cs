using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public TextMeshProUGUI connectionStatus;
	public GameObject multiplayerMenu;

	public TextMeshProUGUI address;
	public Host hostGame;

	void Start()
	{
	}


	public void MultiplayerMenu()
	{		
		if (multiplayerMenu.activeSelf)
		{
			Time.timeScale = 1f;
			multiplayerMenu.SetActive(false);
		}
		else {
			Time.timeScale = 0f;
			multiplayerMenu.SetActive(true);
		}
	}

	public void HostGame() {
        /*
		if (address.text != "")
			hostGame.address = address.text.Trim();
		else
			hostGame.address = "localhost:8000";


		hostGame.enabled = true;
		connectionStatus.text = "hosting";
        */
	}

	public void JoinGame() {
		address.transform.parent = transform.parent;
		DontDestroyOnLoad(address);
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
