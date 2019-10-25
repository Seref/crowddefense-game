using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// define classed needed to deserialize recieved data
[Serializable]
public class Data
{
	public int playerID;
	public Vector3 position;
	public Quaternion rotation;
	public long playerTimeStamp;
	public long serverTimeStamp;
}
[Serializable]
public class Players
{
	public List<Data> players;
}

public class Multiplayer : MonoBehaviour
{

	// define public game object used to visualize other players
	public GameObject otherPlayerObject;
	public float smoothUpdates = 10f;
	private Vector3 prevPosition;
	private Quaternion prevRotation;
	private List<GameObject> otherPlayers = new List<GameObject>();

	IEnumerator Start()
	{
		// get player
		GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];

		// connect to server
		WebSocket w = new WebSocket(new Uri("ws://localhost:8000"));
		yield return StartCoroutine(w.Connect());
		Debug.Log("CONNECTED TO WEBSOCKETS");

		// generate random ID to have idea for each client (feels unsecure)
		System.Guid myGUID = System.Guid.NewGuid();
		int playerID = myGUID.GetHashCode();

		// wait for messages
		while (true)
		{
			// read message
			string message = w.RecvString();
			// check if message is not empty
			if (message != null)
			{
				Debug.Log("RECEIVED FROM WEBSOCKETS: " + message);

				// deserialize recieved data
				Players data = JsonUtility.FromJson<Players>(message);

				// if number of players is not enough, create new ones
				if (data.players.Count > otherPlayers.Count)
				{
					for (int i = 0; i < data.players.Count - otherPlayers.Count; i++)
					{
						otherPlayers.Add(Instantiate(otherPlayerObject, data.players[otherPlayers.Count + i].position, data.players[otherPlayers.Count + i].rotation));
					}
				}

				// update players positions
				for (int i = 0; i < otherPlayers.Count; i++)
				{
					// using animation
					if (smoothUpdates > 0)
					{
						otherPlayers[i].transform.position = Vector3.Lerp(otherPlayers[i].transform.position, data.players[i].position, Time.deltaTime * smoothUpdates);
						otherPlayers[i].transform.rotation = Quaternion.Lerp(otherPlayers[i].transform.rotation, data.players[i].rotation, Time.deltaTime * smoothUpdates);
						// or without animation
					}
					else
					{
						otherPlayers[i].transform.position = data.players[i].position;
						otherPlayers[i].transform.rotation = data.players[i].rotation;
					}
				}
			}

			// if connection error, break the loop
			if (w.error != null)
			{
				Debug.LogError("Error: " + w.error);
				break;
			}

			// check if player moved
			if (prevPosition != player.transform.position || prevRotation != player.transform.rotation)
			{
				// send update if position had changed
				var data = new Data();
				data.playerID = playerID;
				data.rotation = player.transform.rotation;
				data.position = player.transform.position;
				data.playerTimeStamp = DateTime.UtcNow.Ticks;
				Debug.Log(JsonUtility.ToJson(data).ToString());
				w.SendString(playerID.ToString() + "\t" + JsonUtility.ToJson(data).ToString());

				prevPosition = player.transform.position;
				prevRotation = player.transform.rotation;
			}

			yield return 0;
		}

		// if error, close connection
		w.Close();
	}
}
