using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSystemCompability : MonoBehaviour
{

	public GameObject menuButtons;
	public TMPro.TextMeshProUGUI notSupportedBanner;	

    // Update is called once per frame
    void Update()
    {
		if (!Application.isEditor)
			CheckSystem();
	}

	private void CheckSystem()
	{
		if (Application.isMobilePlatform)
		{			
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your Device is not supported!";

		}
		else if (Screen.width < 1280 || Screen.height < 720)
		{
			menuButtons.SetActive(true);
			notSupportedBanner.gameObject.SetActive(true);
			notSupportedBanner.text = "Your screen resolution (" + Screen.width + "x" + Screen.height + ") is below the minimum required 1280x720!\n";
		}
		else
		{
			menuButtons.SetActive(false);
			notSupportedBanner.gameObject.SetActive(false);
		}
	}
}
