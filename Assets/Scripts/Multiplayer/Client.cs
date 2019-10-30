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

    public readonly Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();
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
        while (true) {
            // read message
            string message = w.RecvString();
            // check if message is not empty
            if (message != null) {

                DataGroup data = JsonUtility.FromJson<DataGroup>(message);
                foreach (DataWrapper package in data.dataList) {

                    GameObject gameObject = null;
                    if (gameObjectList.ContainsKey(package.objectID)) {
                        gameObject = gameObjectList[package.objectID];
                    }
                    if (gameObject == null) {
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

                            gameObjectList.Add(package.objectID, gameObject);

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

            yield return 0;
        }

        // if error, close connection
        w.Close();
    }
}
