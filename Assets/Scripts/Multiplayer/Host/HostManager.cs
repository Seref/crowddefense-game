using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Host
{
	public class HostManager : MonoBehaviour
	{
		[Header("ClientTower")]
		public TowerDummy clientTower;
		public HostPlayer hostTower;

		[Header("Relay Server Address")]
		public string address = "wss://localhost:8080/endpoint";

		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();
		private readonly List<DataPackage> actionDataList = new List<DataPackage>();

		private static WebSocket currentWebSocket;

		private string lobbyName;

		private GameObject bulletsList;
		private GameObject enemyList;
		private GameObject autoTowerList;	

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
			StartCoroutine(ProcessData());
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


		private IEnumerator ProcessData()
		{
			while (currentWebSocket != null && currentWebSocket.IsConnected())
			{
				// read messages
				string message = currentWebSocket.RecvString();
				if (message != null)
				{
					try
					{
						ServerRequest sr = JsonUtility.FromJson<ServerRequest>(message);
						DataGroup client = sr.body.data;
						try
						{
							var serverResponse = JsonUtility.FromJson<ServerRequest>(message);

							if (serverResponse != null)
							{
								if (serverResponse.header.method == "GUEST_LEFT")
								{
									GetComponent<HostGameManager>().BackToMainMenu();
								}
							}
						}
						catch
						{
						}

						foreach (DataPackage package in client.dataList)
						{
							switch (package.type)
							{
								case DataType.DataPing:
									actionDataList.Add(package);
									break;

								case DataType.DataTowerDummy:
									{
										DataTowerDummy dataTowerDummy = JsonUtility.FromJson<DataTowerDummy>(package.data);
										clientTower.UpdateData(dataTowerDummy.position, dataTowerDummy.rotation);
									}
									break;
								case DataType.DataClientInput:
									{
										DataClientInput dataClientInput = JsonUtility.FromJson<DataClientInput>(package.data);
										switch (dataClientInput.type)
										{
											case DataClientInputType.TowerShoot:
												clientTower.Fire();
												break;
										}
									}
									break;
								case DataType.DataAutoTower:
									{

									}
									break;
								default:
									break;
							}
						}
					}
					catch
					{
						Debug.Log("Error while deserializing the Data");
					}
				}

				if (currentWebSocket.error != null)
				{
					Debug.LogError("Error: " + currentWebSocket.error);
					break;
				}

				yield return 0;
			}
		}

		//Currently one big Loop that receives&sends data from clients/to clients 
		private IEnumerator TransmitData()
		{
			// Get all Gameobjects, so that we can order the Objects			
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemy");
			autoTowerList = GameObject.Find("AutoTower");

			

			while (currentWebSocket != null && currentWebSocket.IsConnected())
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

				if (currentWebSocket.error != null)
				{
					Debug.LogError("Error: " + currentWebSocket.error);
					break;
				}

				foreach (var datPack in actionDataList)
				{
					dataGroup.dataList.Add(datPack);
				}
				actionDataList.Clear();


				foreach (Transform t in bulletsList.transform)
					if (t.hasChanged)
						dataGroup.dataList.Add(MultiplayerHelper.CreateBulletData(t.gameObject));

				foreach (Transform t in enemyList.transform)
					if (t.hasChanged)
						dataGroup.dataList.Add(MultiplayerHelper.CreateEnemyData(t.gameObject));

				foreach (Transform t in autoTowerList.transform)
					if (t.hasChanged)
						dataGroup.dataList.Add(MultiplayerHelper.CreateAutoTowerData(t.gameObject));

				dataGroup.dataList.Add(MultiplayerHelper.CreateTowerDummyData(hostTower.gameObject));

				currentWebSocket.SendString(JsonUtility.ToJson(serverRequest).ToString());
				yield return new WaitForSecondsRealtime(0.02f);
			}
		}

		//The Client can AddEvents from outside, these will be sent to the Server
		public void EndGame(bool isWin)
		{
			DataPackage endData;
			if (isWin)
				endData = MultiplayerHelper.CreateGameState(DataGameStates.GameWin);
			else
				endData = MultiplayerHelper.CreateGameState(DataGameStates.GameOver);

			actionDataList.Add(endData);
		}

		//The Client can AddEvents from outside, these will be sent to the Server
		public void AddFireEvent(DataClientInputType dataInput, Vector3 position)
		{
			var dataPackage = new DataPackage()
			{
				type = DataType.DataClientInput,
				data = JsonUtility.ToJson(new DataClientInput()
				{
					type = dataInput,
					position = position

				})
			};
			actionDataList.Add(dataPackage);
		}

		private void OnDestroy()
		{
			if (currentWebSocket != null)
				currentWebSocket.Close();
		}

		private void OnApplicationQuit()
		{
			if (currentWebSocket != null)
				currentWebSocket.Close();
		}

	}
}