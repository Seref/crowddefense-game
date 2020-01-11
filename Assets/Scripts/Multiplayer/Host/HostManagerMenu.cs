using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer.Host
{
	public class HostManagerMenu : MonoBehaviour
	{
		[Header("Create Lobby Menu")]
		public GameObject LobbyCreateMenu;
		public TMPro.TMP_InputField Lobbyname;

		public Button StartHosting;
		public TMPro.TextMeshProUGUI ErrorMessage;

		[Header("Wait For Screen Menu")]
		public GameObject WaitingforPlayers;

		public TMPro.TextMeshProUGUI WaitingforPlayersLobbyname;
		public TMPro.TextMeshProUGUI WaitingforPlayersMessage;

		[Header("Dependencies")]
		public HostManager hostManager;
		public HostGameManager hostGameManager;

		public void TryAndConnect()
		{
			StartHosting.interactable = false;
			ErrorMessage.gameObject.SetActive(true);

			DisplayMessage("Trying to create and connect to lobby", Color.white);
			if (Lobbyname.text != null && Lobbyname.text != "")
			{
				hostManager.ConnectToLobby(Lobbyname.text, (a, b) =>
				{
					if (a)
					{
						LobbyCreateMenu.SetActive(false);
						WaitingforPlayers.SetActive(true);
						WaitingforPlayersLobbyname.text = "Lobbyname: " + Lobbyname.text;
						hostManager.WaitForPlayers((c, d) =>
						{
							gameObject.SetActive(false);
							hostManager.StartTransmitting();
							hostGameManager.StartGame();
						});
					}
					else
					{
						StartHosting.interactable = true;
						DisplayMessage(b, Color.red);
					}
				});
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
}