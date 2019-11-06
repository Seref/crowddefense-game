using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	//a hybrid authorative server approach
	public class Host : MonoBehaviour
	{

		//Lists of GameObjects that will appear on the scene
		private GameObject playerList;
		private GameObject bulletsList;
		private GameObject enemyList;
		private GameObject clientObjectsList;

		public string address = "localhost:8000";

		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();
		

		public readonly Dictionary<int, int> playerIdentifier = new Dictionary<int, int>();
		IEnumerator Start()
		{
			// Get all Gameobjects, so that we can order the Objects
			GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
			playerList = GameObject.Find("Players");
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemies");
			clientObjectsList = GameObject.Find("Enemies");

			// connects to server
			Debug.Log("Trying " + address);
			if (address == "")
			{
				address = "localhost:8000";
			}
			address = "ws://" + address.ToString();
			WebSocket w = new WebSocket(new Uri(address));
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");

			// wait for messages
			// When ever a message get's received that updates the position (which are inside of the gameObject list, they will be updated)
			while (true)
			{
				// read message
				string message = w.RecvString();
				// check if message is not empty			
				if (message != null)
				{
					DataGroup data = JsonUtility.FromJson<DataGroup>(message);
					foreach (DataPackage package in data.dataList)
					{
						if (package.sender == 0)
							continue;

						GameObject gameObject = null;
						

						if (gameObjectList.ContainsKey(package.objectID))
						{
							gameObject = gameObjectList[package.objectID];

							if (package.type.Equals(DataType.DataPosition))
							{
								DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
								switch (dataPosition.prefabType)
								{
									case DataPrefabType.ENEMY:
										gameObject = Instantiate(enemyObject, dataPosition.position, dataPosition.rotation);
										enemies.transform.parent = gameObject.transform.parent;
										break;
									case DataPrefabType.PLAYER:
										bullets.transform.parent = gameObject.transform.parent;
										player = Instantiate(playerObject, dataPosition.position, dataPosition.rotation);
										break;
									case DataPrefabType.BULLETS:
										gameObject = Instantiate(bulletObject, dataPosition.position, dataPosition.rotation);
										bullets.transform.parent = gameObject.transform.parent;
										break;
								}

								// package.objectID
								// gameObjectList.Add(, gameObject);

								continue;
							}
							else
							{
								continue;
							}
						}

						switch (package.type)
						{
							case DataType.DataPosition:
								DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
								gameObject.transform.position = dataPosition.position;
								gameObject.transform.rotation = dataPosition.rotation;

								break;
							case DataType.DataStatus:
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
								break;
							default:
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

				int id = 0;

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

		private DataPackage ChangeID(int originalID, int newID, int sender)
		{

			var e = new DataPackage();
			e.objectID = 0000;
			e.sender = sender;
			e.type = DataType.DataPosition;

			var b = new DataPosition();
			b.position = gameObject.transform.position;
			b.rotation = gameObject.transform.rotation;

			e.data = JsonUtility.ToJson(b);

			return e;
		}

		public string GetGameObjectJsonString(GameObject gameObject, DataType dataType, int ID)
		{
			var data = new Data();
			data.type = (int)dataType;
			data.host = true;
			data.playerID = playerID;

			var positionData = new PositionData();
			positionData.id = ID;
			positionData.position = gameObject.transform.position;
			positionData.rotation = gameObject.transform.rotation;
			data.data = JsonUtility.ToJson(positionData).ToString();

			return JsonUtility.ToJson(data).ToString();
		}

	}
}