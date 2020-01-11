using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Host
{
	public class HostManager : MonoBehaviour
	{
		private GameObject playerList;
		private GameObject bulletsList;
		private GameObject enemyList;
		private GameObject autoTowerList;

		public GameObject clientPlayer;

		public string address = "wss://localhost:8080/endpoint";

		private WebSocket currentWebSocket;

		public string lobbyName;

		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

		public void ConnectToLobby(string lobbyName, Action<bool, string> callback = null)
		{
			StartCoroutine(ConnectToLobbyIEnumerator(lobbyName, callback));
		}

		public void WaitForPlayers(Action<bool, string> callback = null)
		{
			StartCoroutine(WaitForPlayersIEnumerator(callback));
		}

		public void StartTransmitting()
		{
			StartCoroutine(TransmitData());
		}


		private IEnumerator ConnectToLobbyIEnumerator(string lobbyName, Action<bool, string> callback = null)
		{
			currentWebSocket = new WebSocket(new Uri(address));
			yield return StartCoroutine(currentWebSocket.Connect());

			Debug.Log("CONNECTED TO WEBSOCKETS");
			if (currentWebSocket.error != null)
			{
				Debug.Log("ERROR CONNECTING TO WEBSOCKET: " + currentWebSocket.error);
				callback?.Invoke(false, "Error: Can't connect to Server!");
				yield break;
			}

			ServerRequest serverRequest = new ServerRequest
			{
				header = new Header
				{
					method = "CREATE_LOBBY",
					lobbyName = lobbyName
				}
			};

			currentWebSocket.SendString(JsonUtility.ToJson(serverRequest).ToString());

			// Wait until Server replies
			int counter = 100;
			while (true)
			{
				string mess = currentWebSocket.RecvString();
				Debug.Log("Server Reply " + mess);

				if (mess != null && mess.Contains("Success"))
				{
					this.lobbyName = lobbyName;
					Debug.Log("Created a Lobby");
					callback?.Invoke(true, "Success, created lobby!");
					break;
				}
				else if (mess != null && mess.Contains("Error"))
				{
					callback?.Invoke(false, mess);
					currentWebSocket.Close();
					yield break;
				}

				if (counter-- <= 0)
				{
					break;
				}

				yield return new WaitForSecondsRealtime(0.25f);
			}

		}

		private IEnumerator WaitForPlayersIEnumerator(Action<bool, string> callback = null)
		{
			Debug.Log("Waiting for other Players");
			if (currentWebSocket != null)
				while (true)
				{
					string message = currentWebSocket.RecvString();
					if (message != null)
					{
						try
						{
							Debug.Log("ServerMessage: " + message);
							ServerRequest clientMessage = JsonUtility.FromJson<ServerRequest>(message);
							if (clientMessage.body.data.clientID == -100)
							{
								callback?.Invoke(true, "clientConnected");
								yield break;
							}
						}
						catch
						{

						}
					}
					yield return new WaitForSecondsRealtime(0.25f);
				}
		}

		//Currently one big Loop that receives&sends data from clients/to clients 
		private IEnumerator TransmitData()
		{
			// Get all Gameobjects, so that we can order the Objects
			playerList = GameObject.Find("Players");
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemy");
			autoTowerList = GameObject.Find("AutoTower");

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
							method = "DATA_TRANSFER",
							lobbyName = lobbyName
						},

						body = new Body()
						{
							data = dataGroup
						}
					};

					// read messages
					string message = currentWebSocket.RecvString();
					if (message != null)
					{
						try
						{
							ServerRequest sr = JsonUtility.FromJson<ServerRequest>(message);
							DataGroup client = sr.body.data;
							foreach (DataPackage package in client.dataList)
							{
								switch (package.type)
								{
									case DataType.DataPing:
										dataGroup.dataList.Add(package);
										break;

									case DataType.DataClientInput:
										{
											DataClientInput dataClientInput = JsonUtility.FromJson<DataClientInput>(package.data);
											switch (dataClientInput.type)
											{
												case DataClientInputType.MoveTower:
													
													break;
												case DataClientInputType.ShootBullet:
													break;
											}

										}
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

					currentWebSocket.SendString(JsonUtility.ToJson(serverRequest).ToString());

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

		private void SendMessage(DataGroup dataGroup)
		{
			var serverRequest = new ServerRequest()
			{
				header = new Header()
				{
					method = "DATA_TRANSFER",
					lobbyName = lobbyName
				},
				body = new Body()
				{
					data = dataGroup
				}
			};
			currentWebSocket.SendString(JsonUtility.ToJson(serverRequest));
		}
	}
}