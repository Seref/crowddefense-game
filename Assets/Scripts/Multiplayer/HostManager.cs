using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	//a hybrid authorative server approach
	public class HostManager : MonoBehaviour
	{
		//Lists of GameObjects that will appear on the scene
		private GameObject playerList;
		private GameObject bulletsList;
		private GameObject enemyList;

		public string address = "wss://localhost:8080/endpoint";

		private WebSocket currentWebSocket;
		public string lobbyName;

		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

		public IEnumerator ConnectToLobby(string lobbyName, Action<bool, string> callback = null)
		{
			WebSocket w = new WebSocket(new Uri(address));
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");

			if (w.error != null)
			{
				Debug.Log("ERROR CONNECTING TO WEBSOCKET: " + w.error);
				callback(false, "Error: Can't connect to Server!");
				yield break;
			}

			ServerRequest serverRequest = new ServerRequest
			{
				header = new Header
				{
					Method = "CREATE_LOBBY",
					LobbyName = lobbyName
				}
			};

			w.SendString(JsonUtility.ToJson(serverRequest).ToString());

			// Wait until Server replies
			int counter = 50 * 2;
			while (true)
			{
				string mess = w.RecvString();
				Debug.Log("Server Reply " + mess);

				if (mess != null && mess.Contains("Success"))
				{
					break;
				}
				else if (mess != null && mess.Contains("Error"))
				{
					callback(false, mess);
					currentWebSocket.Close();
					yield break;
				}

				if (counter-- <= 0)
				{
					break;
				}

				yield return new WaitForSecondsRealtime(0.25f);
			}
			callback(true, "Success, created lobby!");
			this.lobbyName = lobbyName;
			Debug.Log("Created a Lobby");
		}

		public IEnumerator WaitForPlayers(Action<bool, string> callback = null)
		{
			if (currentWebSocket != null)
				while (true)
				{
					string message = currentWebSocket.RecvString();


					if (message != null)
					{
						try
						{
							Debug.Log("ServerMessage: " + message);
							ServerRequest client = JsonUtility.FromJson<ServerRequest>(message);

							if (message != null && message == "Asdasdaökasödkaödkasd")
							{
								callback?.Invoke(true, "fas");
							}
						}
						catch
						{

						}
						yield return new WaitForSecondsRealtime(0.017f);
					}
				}
		}

		public IEnumerator TransmitData()
		{
			// Get all Gameobjects, so that we can order the Objects
			playerList = GameObject.Find("Players");
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemy");

			if (currentWebSocket != null)
				while (true)
				{
					DataGroup dataGroup = new DataGroup
					{
						clientID = -1,
						dataList = new List<DataPackage>()
					};

					ServerRequest serverRequest = new ServerRequest
					{
						header = new Header()
						{
							Method = "DATA_TRANSFER",
							LobbyName = lobbyName
						},

						body = new Body()
						{
							Data = dataGroup
						}
					};

					// read message
					string message = currentWebSocket.RecvString();

					Debug.Log(message);

					if (message != null)
					{
						try
						{
							DataGroup client = JsonUtility.FromJson<DataGroup>(message);

							int clientID = client.clientID;
							foreach (DataPackage package in client.dataList)
							{
								switch (package.type)
								{
									case DataType.DataPing:
										dataGroup.dataList.Add(package);
										break;
									case DataType.DataClientMouseClick:
										break;
									default:
										break;
								}
							}
						}
						catch
						{
						}
					}

					if (currentWebSocket.error != null)
					{
						Debug.LogError("Error: " + currentWebSocket.error);
						break;
					}

					foreach (Transform t in playerList.transform)
						if (t.hasChanged)
							dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.PLAYER));

					foreach (Transform t in bulletsList.transform)
						if (t.hasChanged)
							dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.BULLETS));

					foreach (Transform t in enemyList.transform)
						if (t.hasChanged)
							dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.ENEMY));

					currentWebSocket.SendString(JsonUtility.ToJson(dataGroup).ToString());

					yield return new WaitForSecondsRealtime(0.017f);
				}
		}


		private DataPackage CreatePositionDataPackage(Transform gameObject, DataPrefabType prefabType)
		{
			var dataPackage = new DataPackage();
			dataPackage.type = (int)DataType.DataPrefabPosition;

			var positionData = new DataPrefabPosition();
			positionData.objectID = gameObject.GetInstanceID();
			positionData.prefabType = prefabType;
			positionData.active = gameObject.gameObject.activeSelf;
			positionData.position = gameObject.transform.position;
			positionData.rotation = gameObject.transform.rotation;
			dataPackage.data = JsonUtility.ToJson(positionData).ToString();

			return dataPackage;
		}

	}
}