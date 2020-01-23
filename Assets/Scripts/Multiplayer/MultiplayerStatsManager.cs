using Assets.Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class MultiplayerStatsManager : StatsManager
{	
	private int mHostScore = -1;
	private int mClientScore = -1;	
	
	private TextMeshProUGUI itemEnemyScore;		    

	new void Start()
	{
		base.Start();
		itemEnemyScore = StatsPanel.AddItem();				

		HostScore = 0;
		ClientScore = 0;
		PlayTime = 0;
		Wave = 0;		
        
    }


	public int HostScore
	{
		get { return mHostScore; }
		set
		{
			if (mHostScore == value) return;
			mHostScore = value;			
		}
	}

	public int ClientScore
	{
		get { return mClientScore; }
		set
		{
			if (mClientScore == value) return;
			mClientScore = value;
			itemEnemyScore.text = "Client Score: " + mClientScore;            
		}
	}

	
}
