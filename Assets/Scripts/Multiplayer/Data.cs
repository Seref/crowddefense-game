﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	/// <summary> Datatypes that can be used (as of now)
	/// </summary>
	public enum DataType { DataPrefabPosition, DataStatus, DataPing, DataClientServerAction, DataClientMouseClick };

	/// <summary> DataPackage structure each information is packed inside one "package"
	///    with an additional space for various payloads
	/// </summary>	
	/// <param name="sender">to define the direction Host -> Client or Client -> Host </param>
	/// <param name="type">define a Type to allow for multiple different JSON structures </param>
	/// <param name="data">data that needs to be transitted, the server doesn't have to parse it</param>
	/// <param name="serverTimeStamp">Not used right now, The server needs to insert it's own Timestamp into it (useful synchronization methods)</param>
	[Serializable]
	public class DataPackage
	{
		public DataType type;
		public string data;
		public long serverTimeStamp;
	}


	public enum ServerRequestType { CREATE_LOBBY, JOIN_LOBBY };
	[Serializable]
	public class DataSeverRequest
	{
		public int request;
		public string lobby;
		public string optional;
	}

	/// <summary> DataGroup structure this is for batching multiple pieces of Data, to reduce the amount of connections
	/// </summary>
	/// <param name="dataLlist">A simple list that contains multiple DataWrapper Packets</param>
	[Serializable]
	public class DataGroup
	{		
		public int clientID;
		public List<DataPackage> dataList;
	}

	public enum DataPrefabType { PLAYER, ENEMY, BULLETS }
	/// <summary> Example Payload, this can be replaced with something even smaller that doesn't need to be a Json string,
	/// since the Server doesn't really need to work with it
	/// </summary>
	/// <param name="position">position coordinates</param>
	[Serializable]
	public class DataPrefabPosition
	{
		public int objectID;
		public DataPrefabType prefabType;
		public bool active;
		public Vector3 position;
		public Quaternion rotation;
	}


	[Serializable]
	public class DataPing
	{
		public int id;
		public long time;
	}

	/// <summary> Example Payload, this can be replaced with something even smaller that doesn't need to be a Json string,
	/// since the Server doesn't really need to work with it
	/// </summary>
	/// <param name="clientID">position coordinates</param>
	[Serializable]
	public class DataClientMouseClick
	{
		public Vector3 mouseclick;
	}

	public enum DataClientServerType { JOIN, LEAVE }
	/// <summary> Example Payload, this can be replaced with something even smaller that doesn't need to be a Json string,
	/// since the Server doesn't really need to work with it
	/// </summary>
	/// <param name="clientID">position coordinates</param>
	[Serializable]
	public class DataClientServerAction
	{
		public DataClientServerType action;
	}

	//Server Data
	[Serializable]
	public class Header
	{
		public string Method;
		public string LobbyName;
	}

	[Serializable]
	public class Body
	{
		public DataGroup Data;
	}

	[Serializable]
	public class ServerRequest
	{
		public Header header;
		public Body body;
	}


}
