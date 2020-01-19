using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
    
	private Settings currentSettings;


    public SliderItem MasterSound;
    public SliderItem AutoTowerBuildCooldown;
	public SliderItem AutoTowerAmount;
	public SliderItem AutoTowerFireCooldown;
	public SliderItem WaveEnemyAmount;
	public SliderItem WaveAmount;

	void Start()
	{
		currentSettings = SettingsManager.Instance.GetCurrentSettings();

        MasterSound.SetValue(currentSettings.MasterSound);
        MasterSound.onValueChanged = (a) => { currentSettings.MasterSound = a; SettingsManager.Instance.SetCurrentSettings(currentSettings); };


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
