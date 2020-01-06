using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string clientID;
	public TMPro.TextMeshProUGUI text;

	public GameObject menuButtons;
	public TMPro.TextMeshProUGUI notSupportedBanner;

	void Start()
	{		
		//Application.ExternalEval(("alert(\"" + SystemInfo.deviceType.ToString() + "\");"));
		Application.ExternalEval("OnAppReady();");		
	}

	private void Update()
	{
		CheckSystemCompability();
	}

	private void CheckSystemCompability()
	{
		if (Application.isMobilePlatform)
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your Device is not supported!";
			
		}
		if (Screen.width < 1280 || Screen.height < 720)
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your screen resolution ("+Screen.width + "x" + Screen.height+") is below the minimum required 1280x720!\n";			
		}

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
