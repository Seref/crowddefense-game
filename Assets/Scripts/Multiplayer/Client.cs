using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	public class Client : MonoBehaviour
	{

		// define public game object used to visualize other players
		public GameObject enemyObject;
		public GameObject playerObject;
		public GameObject bulletObject;

		public GameObject clientObjects;

		public GameObject bullets;
		public GameObject player;
		public GameObject enemies;

		private int clientID = 99;
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();
		private Dictionary<int, GameObject> objectlist;


		IEnumerator Start()
		{
			//TextMeshProUGUI t = GameObject.Find("Text_Address").GetComponent<TextMeshProUGUI>();
			// connect to server
			WebSocket w = new WebSocket(new Uri("ws://localhost;"));//+t.text));
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");

			objectlist = new Dictionary<int, GameObject>();

			// wait for messages
			while (true)
			{
				// read message
				string message = w.RecvString();
				// check if message is not empty
				if (message != null)
				{
					//create DataPackage List
					DataGroup data = JsonUtility.FromJson<DataGroup>(message);
					foreach (DataPackage package in data.dataList)
					{
						//check if GameObject already exists
						GameObject gameObject = null;
						var isGameObjectinList = gameObjectList.ContainsKey(package.objectID);

						if (isGameObjectinList)
							gameObject = gameObjectList[package.objectID];

						//go to the designated package type
						switch (package.type)
						{
							//if it's a Command directly to the Client itself, run that
							case DataType.DataClient:
								var clientData = JsonUtility.FromJson<DataClient>(package.data);
								clientID = clientData.clientID;
								break;

							//if it's a Position update, check if the element exists, if not initialize it.
							case DataType.DataPosition:
								if (isGameObjectinList)
								{
									DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
									gameObject.transform.position = dataPosition.position;
									gameObject.transform.rotation = dataPosition.rotation;
								}
								else
								{
									DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
									switch (dataPosition.prefabType)
									{
										case DataPrefabType.ENEMY:
											gameObject = Instantiate(enemyObject, dataPosition.position, dataPosition.rotation);
											enemies.transform.parent = gameObject.transform.parent;
											break;
										case DataPrefabType.PLAYER:
											gameObject = Instantiate(playerObject, dataPosition.position, dataPosition.rotation);
											player.transform.parent = gameObject.transform.parent;
											break;
										case DataPrefabType.BULLETS:
											gameObject = Instantiate(bulletObject, dataPosition.position, dataPosition.rotation);
											bullets.transform.parent = gameObject.transform.parent;
											break;
									}
									gameObjectList.Add(package.objectID, gameObject);
								}
								break;

							//update gameObject Status if it already exists
							case DataType.DataStatus:
								if (isGameObjectinList)
								{
									DataStatus dataStatus = JsonUtility.FromJson<DataStatus>(package.data);
									switch (dataStatus.dataState)
									{
										case DataState.ACTIVE:
											gameObject.SetActive(true);
											break;
										case DataState.INACTIVE:
											gameObject.SetActive(false);
											break;
										case DataState.REMOVED:
											Destroy(gameObject);
											gameObjectList[package.objectID] = null;
											break;
									}
								}
								break;

						}
					}
				}

				// if connection error, break the loop
				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
					break;
				}

				if (prevPosition != player.transform.position || prevRotation != player.transform.rotation)
				{
					// send update if position had changed
					var data = new Data();
					data.host = true;
					data.playerID = playerID;
					data.type = (int)DataType.Player;

					var positionData = new PositionData();
					positionData.id = id++;
					positionData.position = player.transform.position;
					positionData.rotation = player.transform.rotation;
					data.data = JsonUtility.ToJson(positionData).ToString();



					w.SendString(JsonUtility.ToJson(data).ToString());

					prevPosition = player.transform.position;
					prevRotation = player.transform.rotation;

					id += 1000;
					if (bullets != null)
						foreach (Transform bullet in bullets.transform)
							w.SendString(GetGameObjectJsonString(bullet.gameObject, DataType.Bullet, id++));
					else
						bullets = GameObject.Find("Bullet");

					id += 1000;
					if (enemies != null)
						foreach (Transform enemy in enemies.transform)
						{
							var e = GetGameObjectJsonString(enemy.gameObject, DataType.Enemy, id++);
							w.SendString(e);
						}
				}
				yield return 0;
			}

			// if error, close connection
			w.Close();
		}

		//TODO: Check difference between Positions to save some Transmissions
		private bool checkDiffPositions(float diff, GameObject gameObject, int gameObjectID)
		{
			if (!gameObjectList.ContainsKey(gameObjectID))
				return true;
			return true;
		}

		private DataPackage GameObjectPosition(int senderID, GameObject gameObject)
		{
			var e = new DataPackage();
			e.objectID = 0000;
			e.sender = senderID;
			e.type = DataType.DataPosition;

			var b = new DataPosition();
			b.position = gameObject.transform.position;
			b.rotation = gameObject.transform.rotation;

			e.data = JsonUtility.ToJson(b);

			return e;
		}

	}
}