using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Client
{
	//a hybrid authorative server approach
	public class ClientManager : MonoBehaviour
	{
		[Header("Prefabs")]
		public GameObject enemyObject;
		public GameObject bulletObject;
		public GameObject autoTowerObject;

		[Header("Tower")]
		public TowerDummy hostDummy;		
		public GameObject clientTower;


		[Header("Creation GameObject")]
		public GameObject serverObjects;

		[Header("Ping display for UI")]
		public TMPro.TextMeshProUGUI Ping;

		[Header("Relay Server Address")]
		public string address = "wss://localhost:8080/endpoint";

		private WebSocket currentWebSocket;
		private string lobbyName;

		private ClientGameManager gameManager;
		private bool waitingForPing = false;
		
		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

		//List which will be filled with Useractions and then added to the current DataList
		private readonly List<DataPackage> actionDataList = new List<DataPackage>();

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

		private IEnumerator ReceiveDataIEnumerator()
		{
			if (currentWebSocket != null)
			{
				while (true)
				{
					var currentDataList = new List<DataPackage>();

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
									gameManager.BackToMainMenu();
								}
								else
								{
									ProcessServerData(serverResponse.body.data);
								}
							}
						}
						catch (Exception e)
						{
							Debug.LogError(e.Message);
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

					if (!waitingForPing)
					{
						waitingForPing = true;

						var ping = new DataPing
						{
							time = DateTime.UtcNow.Ticks
						};

						dataPackage.data = JsonUtility.ToJson(ping).ToString();

						currentDataList.Add(dataPackage);
					}

					currentDataList.Add(MultiplayerHelper.CreateTowerDummyData(clientTower));					

					foreach (var datPack in actionDataList)
					{
						currentDataList.Add(datPack);
					}
					actionDataList.Clear();					

					currentWebSocket.SendString(JsonUtility.ToJson(serverRequest));
					yield return new WaitForFixedUpdate();
				}
			}
		}

		//Big method that processes the DataBatch (needs to be splitted into smaller methods later on)
		private void ProcessServerData(DataGroup data)
		{
			if (data != null)
			{
				foreach (DataPackage package in data.dataList)
				{
					switch (package.type)
					{
						case DataType.DataPing:
							{
								waitingForPing = false;
								DataPing s = JsonUtility.FromJson<DataPing>(package.data);
								Ping.text = ((DateTime.UtcNow.Ticks - s.time) / TimeSpan.TicksPerMillisecond) + "ms";
							}
							break;

						case DataType.DataBullet:
							{
								DataBullet deserializedData = JsonUtility.FromJson<DataBullet>(package.data);

								//check if GameObject already exists otherwise create it!
								var isGameObjectinList = gameObjectList.ContainsKey(deserializedData.objectID);

								GameObject gameObject = null;
								if (isGameObjectinList)
									gameObject = gameObjectList[deserializedData.objectID];

								if (isGameObjectinList)
								{
									gameObject.transform.position = deserializedData.position;
									gameObject.transform.rotation = deserializedData.rotation;
									gameObject.SetActive(deserializedData.active);
								}
								else
								{
									gameObject = Instantiate(bulletObject, deserializedData.position, deserializedData.rotation);
									gameObject.transform.SetParent(serverObjects.transform);
									gameObject.SetActive(deserializedData.active);

									gameObjectList.Add(deserializedData.objectID, gameObject);
								}
							}
							break;

						case DataType.DataEnemy:
							{								
								DataEnemy deserializedData = JsonUtility.FromJson<DataEnemy>(package.data);
								//check if GameObject already exists otherwise create it!
								var isGameObjectinList = gameObjectList.ContainsKey(deserializedData.objectID);

								GameObject gameObject = null;
								if (isGameObjectinList)
									gameObject = gameObjectList[deserializedData.objectID];

								//Either apply the update, or create the Gameobject + add it to the hashmap
								if (isGameObjectinList)
								{
									var Enemy = gameObject.GetComponent<ClientEnemy>();
									Enemy.UpdateData(deserializedData.position, deserializedData.rotation, deserializedData.active);
								}
								else
								{
									gameObject = Instantiate(enemyObject, deserializedData.position, deserializedData.rotation);
									gameObject.transform.SetParent(serverObjects.transform);
									gameObject.SetActive(deserializedData.active);

									gameObjectList.Add(deserializedData.objectID, gameObject);
								}
							}
							break;

						case DataType.DataClientInput:
							{
								DataClientInput dataClientInput = JsonUtility.FromJson<DataClientInput>(package.data);
								switch (dataClientInput.type)
								{
									case DataClientInputType.TowerShoot:
										hostDummy.ShootBullet(1);
										break;
								}
							}
							break;
						case DataType.DataAutoTower:
							{
								DataAutoTower deserializedData = JsonUtility.FromJson<DataAutoTower>(package.data);
								//check if GameObject already exists otherwise create it!
								var isGameObjectinList = gameObjectList.ContainsKey(deserializedData.objectID);

								GameObject gameObject = null;
								if (isGameObjectinList)
									gameObject = gameObjectList[deserializedData.objectID];

								//Either apply the update, or create the Gameobject + add it to the hashmap
								if (isGameObjectinList)
								{
									DataAutoTower dataTowerDummy = JsonUtility.FromJson<DataAutoTower>(package.data);
									var towerDummy = gameObject.GetComponent<TowerDummy>();
									towerDummy.UpdateData(dataTowerDummy.position, dataTowerDummy.rotation.eulerAngles.z);
									gameObject.SetActive(deserializedData.active);
								}
								else
								{
									gameObject = Instantiate(autoTowerObject, deserializedData.position, deserializedData.rotation);
									gameObject.transform.SetParent(serverObjects.transform);
									gameObject.SetActive(deserializedData.active);
									gameObjectList.Add(deserializedData.objectID, gameObject);
								}
							}
							break;

						case DataType.DataTowerDummy:
							{
								DataTowerDummy dataTowerDummy = JsonUtility.FromJson<DataTowerDummy>(package.data);
								hostDummy.UpdateData(dataTowerDummy.position, dataTowerDummy.rotation);								
							}
							break;
					}
				}
			}
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

	}
}