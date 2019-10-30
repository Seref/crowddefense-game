using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{

	// define public game object used to visualize other players
	public GameObject enemyObject;
	public GameObject playerObject;
	public GameObject bulletObject;
	public float smoothUpdates = 10f;
	private Vector3 prevPosition;
	private Quaternion prevRotation;

	public GameObject bullets;
	public GameObject player;
	public GameObject enemies;

	public EnemyList en;
	private Dictionary<int, GameObject> objectlist;
	IEnumerator Start()
	{		
		//TextMeshProUGUI t = GameObject.Find("Text_Address").GetComponent<TextMeshProUGUI>();
		// connect to server
		WebSocket w = new WebSocket(new Uri("ws://localhost;"));//+t.text));
		yield return StartCoroutine(w.Connect());
		Debug.Log("CONNECTED TO WEBSOCKETS");

		// generate random ID to have idea for each client (feels unsecure)
		System.Guid myGUID = System.Guid.NewGuid();
		int playerID = myGUID.GetHashCode();

		objectlist = new Dictionary<int, GameObject>();
		
		// wait for messages
		while (true)
		{
			// read message
			string message = w.RecvString();
			// check if message is not empty
			if (message != null)
			{				

				ClientData data = JsonUtility.FromJson<ClientData>(message);				

				switch (data.type)
				{
					case (int)DataType.Enemy:
						//TODO: implement Other Player Bullets

						ClientData enemyData = data;
						if (!objectlist.ContainsKey(enemyData.id))
						{
							GameObject enemy = Instantiate(enemyObject, enemyData.position, enemyData.rotation);
							enemies.transform.parent = enemy.transform.parent;
							objectlist.Add(enemyData.id, enemy);
						}
						else {
							GameObject enemy = objectlist[enemyData.id];
							enemy.transform.position = enemyData.position;
							enemy.transform.rotation = enemyData.rotation;
						}
						break;
					case (int)DataType.Bullet:
						//TODO: implement Other Player Bullets
						ClientData bulletData = data;
						if (!objectlist.ContainsKey(bulletData.id))
						{
							GameObject bullet = Instantiate(bulletObject, bulletData.position, bulletData.rotation);
							bullets.transform.parent = bullet.transform.parent;
							objectlist.Add(bulletData.id, bullet);
						}
						else
						{
							GameObject bullet = objectlist[bulletData.id];
							bullet.transform.position = bulletData.position;
							bullet.transform.rotation = bulletData.rotation;
						}
						break;
					case (int)DataType.Player:
						ClientData playerData = data;
						if (!objectlist.ContainsKey(playerData.id))
						{
							GameObject playertemp = Instantiate(playerObject, playerData.position, playerData.rotation);
							player.transform.parent = playertemp.transform.parent;
							objectlist.Add(playerData.id, playertemp);
						}
						else
						{
							GameObject playertemp = objectlist[playerData.id];
							playertemp.transform.position = playerData.position;
							playertemp.transform.rotation = playerData.rotation;
						}
						break;
					default:
						break;
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
