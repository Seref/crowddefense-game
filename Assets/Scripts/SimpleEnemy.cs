using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : MonoBehaviour
{
    NavMeshAgent navagent;
    // Start is called before the first frame update
    void Start()
    {
        navagent = GetComponent<NavMeshAgent>();
        navagent.destination = new Vector3(0, 0,0 );
    }

}
