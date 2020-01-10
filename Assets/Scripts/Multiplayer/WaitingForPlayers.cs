using Assets.Scripts.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayers : MonoBehaviour
{
	public TMPro.TextMeshProUGUI Lobbyname;
	public TMPro.TextMeshProUGUI Message;

	public HostManager hostManager;

	void Start()
	{
		StartCoroutine(hostManager.WaitForPlayers());
	}
}
