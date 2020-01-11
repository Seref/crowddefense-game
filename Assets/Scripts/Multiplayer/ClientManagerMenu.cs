using Assets.Scripts.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class ClientManagerMenu : MonoBehaviour
{
	public TMPro.TMP_InputField Lobbyname;
	public ClientManager clientManager;

	public Button JoinLobby;
	public TMPro.TextMeshProUGUI Message;

	public void TryAndConnect()
	{
		JoinLobby.interactable = false;
		Message.gameObject.SetActive(true);

		if (Lobbyname.text != null && Lobbyname.text != "")
		{
			DisplayMessage("Trying to join " + Lobbyname.text + " lobby", Color.white);

			clientManager.ConnectToLobby(Lobbyname.text, (a, b) =>
			{
				if (a)
				{
					clientManager.ReceiveData();
					gameObject.SetActive(false);					
				}
				else
				{
					JoinLobby.interactable = true;
					DisplayMessage(b, Color.red);
					Lobbyname.text = "";
				}

			});
		}
		else
		{
			JoinLobby.interactable = true;
			DisplayMessage("Enter a Lobbyname!", Color.red);
		}
	}

	private void DisplayMessage(string text, Color color)
	{
		Message.text = text;
		Message.color = color;
	}
}

