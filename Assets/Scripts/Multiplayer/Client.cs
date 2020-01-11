using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
		public TextMeshProUGUI Ping;

		public string lobby = "Test_Server!";

		public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

		IEnumerator Start()
		{
			string address = "ws://192.168.0.103:8000/";
			GameObject text_address = GameObject.Find("Text_Address");
			if (text_address != null)
			{
				TextMeshProUGUI t = text_address.GetComponent<TextMeshProUGUI>();
				if (t.text.Length > 5)
				{
					address = "ws://" + t.text;
				}
			}

			// connect to server
			// connect to server
			// connect to server
			WebSocket w = new WebSocket(new Uri(address));
			yield return StartCoroutine(w.Connect());
			Debug.Log("CONNECTED TO WEBSOCKETS");



			// Wait until Server replies
			int counter = 10 * 2;
			bool success = false;
			while (true)
			{
				string mess = w.RecvString();
				if (mess == "ok")
				{
					success = true;
					break;
				}
				if (counter-- <= 0)
				{
					break;
				}
				yield return new WaitForSecondsRealtime(0.5f);
			}

			if (!success)
			{
				Debug.Log("Failed to Join");
				yield break;
			}
			else
				Debug.Log("Successfully Joined");


			DataGroup dataGroup = new DataGroup();
			dataGroup.clientID = -1;
			var e = JsonUtility.ToJson(dataGroup).ToString();
			w.SendString(e);

			int pingCounter = 0;
			bool waitingForAnswer = false;
			// wait for messages
			while (true)
			{
				string message = w.RecvString();
				// check if message is not empty				
				if (message != null)
				{
					//create DataPackage List

					DataGroup data = JsonUtility.FromJson<DataGroup>(message);
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

				// if connection error, break the loop
				if (w.error != null)
				{
					Debug.LogError("Error: " + w.error);
					break;
				}


				if (!waitingForAnswer)
				{
					waitingForAnswer = true;
					var pi = new DataPing();
					pi.id = pingCounter++;
					pi.time = DateTime.UtcNow.Ticks;

					var dP = new DataPackage();
					dP.type = DataType.DataPing;
					dP.data = JsonUtility.ToJson(pi).ToString();


					var el = new List<DataPackage>();
					el.Add(dP);

					DataGroup dG = new DataGroup();
					dG.clientID = -1;
					dG.dataList = el;
					var goop = JsonUtility.ToJson(dG).ToString();

					w.SendString(goop);
				}


				yield return 0;// new WaitForSecondsRealtime(0.010f);
			}

			// if error, close connection
			w.Close();
		}
	}
}