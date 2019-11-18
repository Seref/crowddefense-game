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

		public string address = "localhost:8000/";
		public string lobby = "Test_Server!";

		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

		IEnumerator Start()
		{
			// Get all Gameobjects, so that we can order the Objects
			playerList = GameObject.Find("Players");
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemy");

			// connects to server			
			if (address.Length <= 5)
			{
				address = "localhost:8000/";
			}

			address = "ws://" + address.ToString();
			Debug.Log("Connecting to: " + address);
			WebSocket w = new WebSocket(new Uri(address));
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");


			// Create Lobby
			DataSeverRequest serverRequest = new DataSeverRequest();
			serverRequest.lobby = "SERVER_COMMAND";
			serverRequest.request = (int)ServerRequestType.CREATE_LOBBY;
			serverRequest.optional = lobby;
			w.SendString(JsonUtility.ToJson(serverRequest).ToString());

			// Wait until Server replies

			int counter = 10 * 2;
			bool success = false;
			while (true)
			{
				string mess = w.RecvString();
				Debug.Log("Server Reply " + mess);
				if (mess == "ok")
				{
					success = true;
					break;
				}
				if (counter-- <= 0)
				{
					break;
				}
				yield return new WaitForSecondsRealtime(0.016f);
			}

			if (!success)
			{
				Debug.Log("Failed to Host");
				yield break;
			}
			else
				Debug.Log("Created a Lobby");
			
			// wait for messages
			while (true)
			{
				// read message
				string message = w.RecvString();
				// check if message is not empty			
				Debug.Log(message);
				DataGroup dataGroup = new DataGroup
				{
					lobby = lobby,
					dataList = new List<DataPackage>()
				};

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

				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
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

				w.SendString(JsonUtility.ToJson(dataGroup).ToString());

				yield return new WaitForSecondsRealtime(0.017f);
			}

			// if error, close connection
			w.Close();
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