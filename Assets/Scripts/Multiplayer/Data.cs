using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	public enum DataType { DataPrefabPosition, DataPing, DataClientInput };


	[Serializable]
	public class DataPackage
	{
		public DataType type;
		public string data;
		public long serverTimeStamp;
	}

	[Serializable]
	public class DataGroup
	{
		public int clientID;
		public List<DataPackage> dataList;
	}

	public enum DataPrefabType { PLAYER, ENEMY, BULLETS, CLIENT, EXPLOSIONEFFECT, TIME }

	[Serializable]
	public class DataPrefabPosition
	{
		public int objectID;
		public DataPrefabType prefabType;
		public bool active;
		public Vector3 position;
		public Quaternion rotation;
		public string additional;
	}

	[Serializable]
	public class DataPing
	{
		public int id;
		public long time;
	}

	public enum DataClientInputType { MoveTower, ShootBullet, DragAutoTower, PlaceAutoTower }
	[Serializable]
	public class DataClientInput
	{
		public DataClientInputType type;
		public Vector3 position;
		public string additional;
	}

	[Serializable]
	public class DataClientMouseClick
	{
		public Vector3 mouseclick;
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
