using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : MonoBehaviour
{
	Vector3 start;
	NavMeshAgent agent;

	public GameObject paths;

	private Transform[] points;
	private int destPoint = 0;

	void Start()
	{
		start = transform.position;
		agent = GetComponent<NavMeshAgent>();

		points = new Transform[paths.transform.childCount];
		int i = 0;
		foreach (Transform child in paths.transform)
		{
			points[i++] = child;
		}

		agent.autoBraking = false;
		agent.autoRepath = true;

		GotoNextPoint();
	}

	void GotoNextPoint()
	{
		// Returns if no points have been set up
		if (points.Length == 0)
			return;

		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Length;
	}


	void Update()
	{
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!agent.pathPending && agent.remainingDistance < 0.5f)
			GotoNextPoint();
	}


	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			agent.destination = points[0].position;
			destPoint = 0;
			transform.position = start;
			
		}
	}
}
