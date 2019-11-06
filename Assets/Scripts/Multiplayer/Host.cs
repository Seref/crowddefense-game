﻿using System;
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

		public string address = "localhost:8000";

		//Dictonary that gives every multiplayer gameobject an id (for faster access time)
		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();


		IEnumerator Start()
		{
			// Get all Gameobjects, so that we can order the Objects
			playerList = GameObject.Find("Players");
			bulletsList = GameObject.Find("Bullet");
			enemyList = GameObject.Find("Enemies");

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
					try
					{
						DataGroup data = JsonUtility.FromJson<DataGroup>(message);


						foreach (DataPackage package in data.dataList)
						{
							if (package.sender == 0)
								continue;

							/*
							if (package.type.Equals(DataType.DataClientMouseClick))
							{							
							}
							else
							{
								continue;
							}
							*/
						}

					}
					catch{
					}
				}


				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
					Debug.Log(message);
					break;
				}

				DataGroup dataGroup = new DataGroup();
				dataGroup.dataList = new List<DataPackage>();

				foreach (Transform t in playerList.transform)
					dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.PLAYER));

				foreach (Transform t in bulletsList.transform)
					dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.BULLETS));

				foreach (Transform t in enemyList.transform)
					dataGroup.dataList.Add(CreatePositionDataPackage(t, DataPrefabType.ENEMY));

				var e = JsonUtility.ToJson(dataGroup).ToString();
				Debug.Log(e);
				w.SendString(e);
				yield return null; //new WaitForSeconds(0.033f);
			}

			// if error, close connection
			w.Close();
		}

		private DataPackage CreatePositionDataPackage(Transform gameObject, DataPrefabType prefabType)
		{
			var dataPackage = new DataPackage();
			dataPackage.sender = 0;
			dataPackage.type = (int)DataType.DataPrefabPosition;

			var positionData = new DataPrefabPosition();
			positionData.objectID = gameObject.GetInstanceID();
			positionData.prefabType = prefabType;
			positionData.position = gameObject.transform.position;
			positionData.rotation = gameObject.transform.rotation;
			dataPackage.data = JsonUtility.ToJson(positionData).ToString();

			return dataPackage;
		}
	}
}