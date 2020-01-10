using Assets.Scripts.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class HostManagerMenu : MonoBehaviour
{
	public TMPro.TMP_InputField Lobbyname;
	public HostManager hostManager;

	public Button StartHosting;
	public TMPro.TextMeshProUGUI ErrorMessage;

	public WaitingForPlayers WaitingforPlayers;

	public void TryAndConnect()
	{
		StartHosting.interactable = false;
		ErrorMessage.gameObject.SetActive(true);

		DisplayMessage("Trying to create and connect to lobby", Color.white);
		if (Lobbyname.text != null && Lobbyname.text != "")
		{
			StartCoroutine(hostManager.ConnectToLobby(Lobbyname.text, (a, b) =>
			{
				if (a)
				{
					WaitingforPlayers.gameObject.SetActive(true);
					WaitingforPlayers.Lobbyname.text = "Lobbyname: "+Lobbyname.text;
					this.gameObject.SetActive(false);					
				}
				else
				{
					StartHosting.interactable = true;
					DisplayMessage(b, Color.red);
				}

			}));
		}
		else
		{
			StartHosting.interactable = true;
			DisplayMessage("Enter a Lobbyname!", Color.red);
		}
	}

	private void DisplayMessage(string text, Color color)
	{
		ErrorMessage.text = text;
		ErrorMessage.color = color;
	}
}

