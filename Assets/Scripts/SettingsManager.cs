using System;
using System.Runtime.InteropServices;
using UnityEngine;


[Serializable]
public class Settings
{
	//AutoTower Default Settings
	public int AutoTowerBuildCooldown = 10;
	public int AutoTowerAmount = 3;
	public int AutoTowerFireCooldown = 2;

	//Wave Default Settings
	public int WaveEnemyAmount = 5;
	public int WaveAmount = 10;
}

public class SettingsManager
{
	[DllImport("__Internal")]
	private static extern string ReadSettings();

	[DllImport("__Internal")]
	private static extern void SaveSettings(string data);


	public static readonly SettingsManager Instance = new SettingsManager();


	private SettingsManager()
	{
		LoadCurrentSettings();
	}


	private Settings currentSettings;

	public void LoadCurrentSettings()
	{
		//load default settings
		currentSettings = new Settings();

		//reading localstored settings;
		try
		{
			var e = ReadSettings();
			if (e != "error")
			{
				currentSettings = JsonUtility.FromJson<Settings>(e);
			}
		}
		catch
		{
			Debug.Log("Couldn't load previously saved stuff");
			currentSettings = new Settings();
		}
	}

	public void LoadTemporarySettings(Settings newSettings) {
		currentSettings = newSettings;
	}

	public Settings GetCurrentSettings()
	{
		return currentSettings;
	}

	public void SetCurrentSettings(Settings newSettings)
	{
		currentSettings = newSettings;
		try
		{
			SaveSettings(JsonUtility.ToJson(newSettings).ToString());
		}
		catch
		{
			Debug.Log("Couldn't save stuff permanently");
		}
	}

}