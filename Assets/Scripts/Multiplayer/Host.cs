using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> Datatypes that can be used (as of now)
/// </summary>
public enum DataType { DataPosition, DataStatus, DataClient };

/// <summary> DataPackage structure each information is packed inside one "package"
///    with an additional space for various payloads
/// </summary>
/// <param name="objectID">unique ID of every Gameobject that needs to be updated on the server</param>
/// <param name="sender">to define the direction Host -> Client or Client -> Host </param>
/// <param name="type">define a Type to allow for multiple different JSON structures </param>
/// <param name="data">data that needs to be transitted, the server doesn't have to parse it</param>
/// <param name="serverTimeStamp">Not used right now, The server needs to insert it's own Timestamp into it (useful synchronization methods)</param>
[Serializable]
public class DataPackage
{
    public int objectID;
    public int sender;
    public DataType type;
    public string data;
    //public long serverTimeStamp;
}

/// <summary> DataGroup structure this is for batching multiple pieces of Data, to reduce the amount of connections
/// </summary>
/// <param name="dataLlist">A simple list that contains multiple DataWrapper Packets</param>
[Serializable]
public class DataGroup
{
    public List<DataPackage> dataList;
}

public enum DataPrefabType { PLAYER, ENEMY, BULLETS }
/// <summary> Example Payload, this can be replaced with something even smaller that doesn't need to be a Json string,
/// since the Server doesn't really need to work with it
/// </summary>
/// <param name="position">position coordinates</param>
[Serializable]
public class DataPosition
{
    public DataPrefabType prefabType;
    public Vector3 position;
    public Quaternion rotation;
}

/// <summary> Example Payload, this can be replaced with something even smaller that doesn't need to be a Json string,
/// since the Server doesn't really need to work with it
/// </summary>
/// <param name="clientID">position coordinates</param>
[Serializable]
public class DataClient
{
    public int clientID;
}

/// <summary> Change the GameObject ID with the one from the Host 
/// </summary>
[Serializable]
public class DataChangeID
{
    public int originalID;
    public int newID;
}


public enum DataState { ACTIVE, INACTIVE, REMOVED }
// <summary> Change the current state of the Client
/// </summary>
/// <param name="position">position coordinates</param>
[Serializable]
public class DataStatus
{
    public DataState dataState;
}


public class Host : MonoBehaviour
{
   
    public GameObject playersList;
    public GameObject enemiesList;
    public GameObject bulletsList;

    public string address = "localhost:8000";

    public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

    public readonly Dictionary<int, int> clientToHostID = new Dictionary<int, int>();

    IEnumerator Start()
    {
        // Get all Gameobjects, so that we can order the Objects
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        playersList = GameObject.Find("Players");
        bulletsList = GameObject.Find("Bullet");
        enemiesList = GameObject.Find("Enemies");

        // connects to server
        Debug.Log("Trying " + address);
        if (address == "") {
            address = "localhost:8000";
        }
        address = "ws://" + address.ToString();
        WebSocket w = new WebSocket(new Uri(address));
        yield return StartCoroutine(w.Connect());
        Debug.Log("CONNECTED TO WEBSOCKETS");

        // wait for messages
        // When ever a message get's received that updates the position (which are inside of the gameObject list, they will be updated)
        while (true) {
            // read message
            string message = w.RecvString();
            // check if message is not empty			
            if (message != null) {
                DataGroup data = JsonUtility.FromJson<DataGroup>(message);
                foreach (DataPackage package in data.dataList) {
                    if (package.sender == 0)
                        continue;

                    GameObject gameObject = null;

                    //Idea: create a new ID for something that was created by the player and tell them to adapt to it
                    //Will probably be replaced by something a bit more soficiticated

                    if (gameObjectList.ContainsKey(package.objectID)) {
                        gameObject = gameObjectList[package.objectID];

                        if (package.type.Equals(DataType.DataPosition)) {
                            DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
                            switch (dataPosition.prefabType) {
                                case DataPrefabType.ENEMY:
                                    gameObject = Instantiate(enemyObject, dataPosition.position, dataPosition.rotation);
                                    enemies.transform.parent = gameObject.transform.parent;
                                    break;
                                case DataPrefabType.PLAYER:
                                    bullets.transform.parent = gameObject.transform.parent;
                                    player = Instantiate(playerObject, dataPosition.position, dataPosition.rotation);
                                    break;
                                case DataPrefabType.BULLETS:
                                    gameObject = Instantiate(bulletObject, dataPosition.position, dataPosition.rotation);
                                    bullets.transform.parent = gameObject.transform.parent;
                                    break;
                            }

                            // package.objectID
                            // gameObjectList.Add(, gameObject);

                            continue;
                        }
                        else {
                            continue;
                        }
                    }

                    switch (package.type) {
                        case DataType.DataPosition:
                            DataPosition dataPosition = JsonUtility.FromJson<DataPosition>(package.data);
                            gameObject.transform.position = dataPosition.position;
                            gameObject.transform.rotation = dataPosition.rotation;

                            break;
                        case DataType.DataStatus:
                            DataStatus dataStatus = JsonUtility.FromJson<DataStatus>(package.data);
                            switch (dataStatus.dataState) {
                                case DataState.ACTIVE:
                                    gameObject.SetActive(true);
                                    break;
                                case DataState.INACTIVE:
                                    gameObject.SetActive(false);
                                    break;
                                case DataState.REMOVED:
                                    Destroy(gameObject);
                                    gameObjectList[package.objectID] = null;
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                }
            }

            // if connection error, break the loop
            if (w.error != null) {
                Debug.LogError("Error: " + w.error);
                break;
            }

            int id = 0;
            // check if player moved
            if (prevPosition != player.transform.position || prevRotation != player.transform.rotation) {
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

    private DataPackage ChangeID(int originalID, int newID, int sender)
    {
        
        var e = new DataPackage();
        e.objectID = 0000;
        e.sender = sender;
        e.type = DataType.DataPosition;

        var b = new DataPosition();
        b.position = gameObject.transform.position;
        b.rotation = gameObject.transform.rotation;

        e.data = JsonUtility.ToJson(b);
        
        return e;
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
