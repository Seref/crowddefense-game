using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	//a hybrid authorative server approach
	public class ClientManager : MonoBehaviour
	{
		public GameObject enemyObject;
		public GameObject playerObject;
		public GameObject bulletObject;

		public GameObject serverObjects;

		public string address = "wss://localhost:8080/endpoint";

		private WebSocket currentWebSocket;
		public string lobbyName;

		private ClientGameManager gameManager;


		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();


		void Start()
		{
			gameManager = GetComponent<ClientGameManager>();
		}

		public void ConnectToLobby(string lobbyName, Action<bool, string> callback = null)
		{
			StartCoroutine(ConnectToLobbyIEnumerator(lobbyName, callback));
		}

		public void ReceiveData()
		{
			StartCoroutine(ReceiveDataIEnumerator());
		}

		private IEnumerator ConnectToLobbyIEnumerator(string lobName, Action<bool, string> callback = null)
		{
			currentWebSocket = new WebSocket(new Uri(address));
			yield return StartCoroutine(currentWebSocket.Connect());

			Debug.Log("CONNECTED TO WEBSOCKETS");

			if (currentWebSocket.error != null)
			{
				Debug.Log("ERROR CONNECTING TO WEBSOCKET: " + currentWebSocket.error);
				callback(false, "Error: Can't connect to Server!");
				yield break;
			}

			ServerRequest serverRequest = new ServerRequest
			{
				header = new Header
				{
					method = "JOIN_LOBBY",
					lobbyName = lobName
				}
			};

			currentWebSocket.SendString(JsonUtility.ToJson(serverRequest).ToString());

			// Wait until Server replies
			int counter = 50 * 2;
			while (true)
			{
				string mess = currentWebSocket.RecvString();
				Debug.Log("Server Reply " + mess);

				if (mess != null && mess.Contains("Success"))
				{
					break;
				}
				else if (mess != null && mess.Contains("Error"))
				{
					callback(false, mess);
					this.currentWebSocket.Close();
					yield break;
				}

				if (counter-- <= 0)
				{
					break;
				}

				yield return new WaitForSecondsRealtime(0.25f);
			}
			lobbyName = lobName;
			Debug.Log("Joined a Lobby");

			serverRequest = new ServerRequest
			{
				header = new Header
				{
					method = "DATA_TRANSFER",
					lobbyName = lobbyName
				},
				body = new Body
				{
					data = new DataGroup()
					{
						clientID = -100
					}
				}
			};

			currentWebSocket.SendString(JsonUtility.ToJson(serverRequest).ToString());

			callback?.Invoke(true, "Success, joined lobby!");
		}

		private List<DataPackage> currentDataList;
		private readonly List<DataPackage> actionDataList = new List<DataPackage>();

		private IEnumerator ReceiveDataIEnumerator()
		{
			if (currentWebSocket != null)
			{
				while (true)
				{
					currentDataList = new List<DataPackage>();

					DataGroup dataGroup = new DataGroup
					{
						clientID = -1,
						dataList = currentDataList
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

					string message = currentWebSocket.RecvString();
					if (message != null)
					{
						try
						{
							var serverResponse = JsonUtility.FromJson<ServerRequest>(message);

							if (serverResponse != null)

							{
								if (serverResponse.header.method == "HOST_LEFT")
								{
									Debug.Log("Host Left!!");
									gameManager.BackToMainMenu();
								}
								else
								{
									ProcessData(serverResponse.body.data);
								}
							}
						}
						catch
						{
							Debug.Log("Error in parsing Response");
						}
					}
					// if connection error, break the loop
					if (currentWebSocket.error != null)
					{
						Debug.LogError("Error: " + currentWebSocket.error);
						break;
					}

					var dataPackage = new DataPackage
					{
						type = DataType.DataPing
					};

					if (!waitingForAnswer)
					{
						waitingForAnswer = true;

						var pi = new DataPing
						{
							id = pingCounter++,
							time = DateTime.UtcNow.Ticks
						};
						dataPackage.data = JsonUtility.ToJson(pi).ToString();

						currentDataList.Add(dataPackage);
					}

					foreach (var datPack in actionDataList)
					{
						currentDataList.Add(datPack);
					}
					currentDataList.Clear();

					currentWebSocket.SendString(JsonUtility.ToJson(serverRequest));
					yield return 0;
				}
			}
		}

		private bool waitingForAnswer = false;
		public TMPro.TextMeshProUGUI Ping;
		private int pingCounter;

		private void ProcessData(DataGroup data)
		{
			if (data != null)
			{
				foreach (DataPackage package in data.dataList)
				{
					switch (package.type)
					{
						case DataType.DataPing:
							{
								waitingForAnswer = false;
								DataPing s = JsonUtility.FromJson<DataPing>(package.data);
								//Debug.Log(s.id + " diff: " + ((DateTime.UtcNow.Ticks - s.time) / TimeSpan.TicksPerMillisecond + "ms\n"));
								Ping.text = ((DateTime.UtcNow.Ticks - s.time) / TimeSpan.TicksPerMillisecond) + "ms";
							}
							break;
						case DataType.DataPrefabPosition:
							{
								DataPrefabPosition prefabPosition = JsonUtility.FromJson<DataPrefabPosition>(package.data);

								//check if GameObject already exists
								GameObject gameObject = null;
								var isGameObjectinList = gameObjectList.ContainsKey(prefabPosition.objectID);

								if (isGameObjectinList)
									gameObject = gameObjectList[prefabPosition.objectID];

								if (isGameObjectinList)
								{
									gameObject.transform.position = prefabPosition.position;
									gameObject.transform.rotation = prefabPosition.rotation;
									gameObject.SetActive(prefabPosition.active);
								}
								else
								{
									switch (prefabPosition.prefabType)
									{
										case DataPrefabType.ENEMY:
											gameObject = Instantiate(enemyObject, prefabPosition.position, prefabPosition.rotation);
											gameObject.transform.SetParent(serverObjects.transform);
											gameObject.SetActive(prefabPosition.active);
											break;
										case DataPrefabType.PLAYER:
											gameObject = Instantiate(playerObject, prefabPosition.position, prefabPosition.rotation);
											gameObject.transform.SetParent(serverObjects.transform);
											gameObject.SetActive(prefabPosition.active);
											break;
										case DataPrefabType.BULLETS:
											gameObject = Instantiate(bulletObject, prefabPosition.position, prefabPosition.rotation);
											gameObject.transform.SetParent(serverObjects.transform);
											gameObject.SetActive(prefabPosition.active);
											break;
									}

									gameObjectList.Add(prefabPosition.objectID, gameObject);
								}
							}
							break;
					}
				}
			}
		}

		public void AddEvent(DataClientInputType dataInput, Vector3 position)
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

		private void SendMessage(DataGroup dataGroup)
		{
			var serverRequest = new ServerRequest()
			{
				header = new Header()
				{
					lobbyName = this.lobbyName,
					method = "DATA_TRANSFER"
				},
				body = new Body()
				{
					data = dataGroup
				}
			};

			currentWebSocket.SendString(JsonUtility.ToJson(serverRequest));
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
