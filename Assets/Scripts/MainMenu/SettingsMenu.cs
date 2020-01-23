using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
    
	private Settings currentSettings;


    public SliderItem MasterSound;

	void Start()
	{
		currentSettings = SettingsManager.Instance.GetCurrentSettings();

        MasterSound.SetValue(currentSettings.MasterSound);
        MasterSound.onValueChanged = (a) => { currentSettings.MasterSound = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };        
	}

    public void ResetSettings()
	{
		Settings reset = new Settings();
		SettingsManager.Instance.SetCurrentSettings(reset);
		currentSettings = reset;
		SceneManager.LoadScene("SettingsMenu");
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}


}
