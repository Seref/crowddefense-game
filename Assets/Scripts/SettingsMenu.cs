using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;

	public GameObject menuButtons;
	public TMPro.TextMeshProUGUI notSupportedBanner;

	private Settings currentSettings;

	public SliderItem AutoTowerBuildCooldown;
	public SliderItem AutoTowerAmount;
	public SliderItem AutoTowerFireCooldown;
	public SliderItem WaveEnemyAmount;
	public SliderItem WaveAmount;

	void Start()
	{
		currentSettings = SettingsManager.Instance.GetCurrentSettings();

		AutoTowerBuildCooldown.SetValue(currentSettings.AutoTowerBuildCooldown);
		AutoTowerBuildCooldown.onValueChanged = (a) => { currentSettings.AutoTowerBuildCooldown = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };

		AutoTowerAmount.SetValue(currentSettings.AutoTowerAmount);
		AutoTowerAmount.onValueChanged = (a) => { currentSettings.AutoTowerAmount = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };

		AutoTowerFireCooldown.SetValue(currentSettings.AutoTowerFireCooldown);
		AutoTowerFireCooldown.onValueChanged = (a) => { currentSettings.AutoTowerFireCooldown = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };

		WaveEnemyAmount.SetValue(currentSettings.WaveEnemyAmount);
		WaveEnemyAmount.onValueChanged = (a) => { currentSettings.WaveEnemyAmount = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };

		WaveAmount.SetValue(currentSettings.WaveAmount);
		WaveAmount.onValueChanged = (a) => { currentSettings.WaveAmount = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };
	}



	private void Update()
	{
		if (!Application.isEditor)
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
		else if (Screen.width < 1280 || Screen.height < 720)
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your screen resolution (" + Screen.width + "x" + Screen.height + ") is below the minimum required 1280x720!\n";
		}
		else
		{
			menuButtons.SetActive(true);
			notSupportedBanner.gameObject.SetActive(false);
		}
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}


}
