using System;
using System.Runtime.InteropServices;
using UnityEngine;



[Serializable]
public class Settings
{
    //zum Settings Datenbank
    public int version = SettingsManager.VERSION;

    //General Settings
    public int MasterSound = 100;

	//AutoTower Default Settings
	public int AutoTowerBuildCost = 20;	
	public float AutoTowerFireCooldown = 2.0f;

    //AutoTower Default Settings
    public int FastAutoTowerBuildCost = 50;    
    public float FastAutoTowerFireCooldown = 1.0f;

    //Wave Default Settings
    public int WaveEnemyAmount = 5;
	public int WaveAmount = 10;
}

public class SettingsManager
{
    public static int VERSION = 2;

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
                var dec = HelperFunctions.DecompressString(e);

				currentSettings = JsonUtility.FromJson<Settings>(dec);

                if (currentSettings.version != VERSION)            
                    currentSettings = new Settings();
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
			SaveSettings(HelperFunctions.CompressString(JsonUtility.ToJson(newSettings).ToString()));
		}
		catch
		{
			Debug.Log("Couldn't save stuff permanently");
		}
	}

}