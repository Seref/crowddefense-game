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

		public GameObject serverObjects;		

		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();


		IEnumerator Start()
		{
			//TextMeshProUGUI t = GameObject.Find("Text_Address").GetComponent<TextMeshProUGUI>();

			// connect to server
			WebSocket w = new WebSocket(new Uri("ws://localhost:8000/"));//+t.text));//localhost"));//
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");

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
					if (data != null)
						foreach (DataPackage package in data.dataList)
						{
							if (package.type != DataType.DataPrefabPosition)
								continue;

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
				}


				// if connection error, break the loop
				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
					break;
				}
				yield return 0;
			}

			// if error, close connection
			w.Close();
		}
	}
}