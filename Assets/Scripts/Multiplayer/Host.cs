using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DataType { Enemy, Player, Bullet };

// define classed needed to deserialize recieved data
[Serializable]
public class Data
{
	public int playerID;
	public bool host;
	public int type;
	public string data;
	//public long playerTimeStamp;
	//public long serverTimeStamp;
}

[Serializable]
public class PositionData
{
	public int id;
	public Vector3 position;
	public Quaternion rotation;
}

[Serializable]
public class ClientData
{
	public int type;
	public int id;
	public Vector3 position;
	public Quaternion rotation;	
}

[Serializable]
public class Players
{
	public List<Data> players;
}

[Serializable]
public class EnemyList
{
	public List<Data> enemyobje;
}

public class Host : MonoBehaviour
{

	// define public game object used to visualize other players
	public GameObject otherPlayerObject;
	public float smoothUpdates = 10f;
	private Vector3 prevPosition;
	private Quaternion prevRotation;
	private List<GameObject> otherPlayers = new List<GameObject>();
	private int playerID = 0;

	public GameObject player;
	public GameObject enemies;
	public GameObject bullets;

	public string address = "localhost:8000";

	IEnumerator Start()
	{
		// get player
		GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
		bullets = GameObject.Find("Bullet");
		enemies = GameObject.Find("Enemies");

		// connect to server
		Debug.Log("Trying " + address);
		if (address == "") {
			address = "localhost:8000";
		}
		address = "ws://" + address.ToString();
		WebSocket w = new WebSocket(new Uri(address));
		yield return StartCoroutine(w.Connect());
		Debug.Log("CONNECTED TO WEBSOCKETS");

		// generate random ID to have idea for each client (feels unsecure)
		System.Guid myGUID = System.Guid.NewGuid();
		playerID = myGUID.GetHashCode();

		// wait for messages

		while (true)
		{
			// read message
			string message = w.RecvString();
			// check if message is not empty			
			if (message != null)
			{
				//Debug.Log("RECEIVED FROM WEBSOCKETS: " + message);

				/*
				// deserialize recieved data
				Players data = JsonUtility.FromJson<Players>(message);
				*/

				Data data = JsonUtility.FromJson<Data>(message);
				switch (data.type)
				{
					case (int)DataType.Bullet:
						//TODO: implement Other Player Bullets
						break;
					case (int)DataType.Player:
						//TODO: implement Other Player Position
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

			int id = 0;
			// check if player moved
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
					foreach (Transform enemy in enemies.transform) {
						var e = GetGameObjectJsonString(enemy.gameObject, DataType.Enemy, id++);						
						w.SendString(e);
					}

			}

			yield return 0;
		}

		// if error, close connection
		w.Close();
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
