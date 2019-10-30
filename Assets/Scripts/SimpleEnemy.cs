using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : MonoBehaviour
{
	Vector3 start;
	NavMeshAgent navagent;
	void Start()
	{
		start = transform.position;
		navagent = GetComponent<NavMeshAgent>();
		navagent.destination = new Vector3(0, 0, 0);
	}

	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			transform.position = start;			
		}
	}
}
