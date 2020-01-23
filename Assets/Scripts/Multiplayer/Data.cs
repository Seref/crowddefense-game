using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{

	//Enum of possible DataTypes that can be sent
	public enum DataType {DataPing, DataBullet, DataClientInput, DataTowerDummy, DataEnemy, DataAutoTower, DataGameState };

	//A single Datapackage with some custom payload

	[Serializable]
	public class DataPackage
	{
		public DataType type;
		public string data;
	}	

	//Batching multiple packages to allow for better performance
	[Serializable]
	public class DataGroup
	{
		public int clientID;
		public long serverTimeStamp;
		public List<DataPackage> dataList;
	}

	//Here are some DataPoints that get sent from the Client to the Server or vice versa, objectID is the "Unity" generated ID and should be unique!		
	//The approach before used one general type for multiple prefabs, though it's probably better to seperate them like this for easier changes
	[Serializable]
	public class DataBullet
	{
		public int objectID;
		public bool active;
		public Vector3 position;
		public Quaternion rotation;
	}


	public enum DataGameStates { GameOver, GameWin};

	[Serializable]
	public class DataGameState
	{
		public DataGameStates state;
	}

	[Serializable]
	public class DataCurrentStats
	{
		public string stats;
	}	

	[Serializable]
	public class DataAutoTower
	{
		public int objectID;
		public bool active;
		public Vector3 position;
		public Quaternion rotation;
	}

	[Serializable]
	public class DataEnemy
	{
		public int objectID;
		public bool active;
		public Vector3 position;
		public Quaternion rotation;
	}	

	[Serializable]
	public class DataTowerDummy
	{
		public int objectID;
		public bool active;
		public bool shooting;
		public int duration;
		public Vector3 position;
		public float rotation;
	}	

	[Serializable]
	public class DataPing
	{
		public int id;
		public long time;
	}

	public enum DataClientInputType { TowerShoot, AutoTowerDrag, AutoTowerPlace }
	[Serializable]
	public class DataClientInput
	{
		public DataClientInputType type;
		public Vector3 position;		
		public string additional;
	}




	//RELAY SERVER COMMUNICATION Data formats!

	[Serializable]
	public class Header
	{
		public string method;
		public string lobbyName;
	}

	[Serializable]
	public class Body
	{
		public DataGroup data;
	}


	[Serializable]
	public class ServerRequest
	{
		public Header header;
		public Body body;
	}


}
