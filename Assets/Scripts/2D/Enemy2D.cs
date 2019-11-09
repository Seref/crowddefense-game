using UnityEngine;
using UnityEngine.AI;

public class Enemy2D : MonoBehaviour
{
	Vector3 start;
	NavMeshAgent agent;

	private GameObject paths;
	private Transform[] points;
	private int destPoint = 0;

	void Awake()
	{		
		agent = GetComponent<NavMeshAgent>();
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		agent.autoBraking = false;
		agent.autoRepath = true;
	}

	public void StartPath(GameObject paths)
	{
		start = transform.position;
		this.paths = paths;
		points = new Transform[paths.transform.childCount];
		destPoint = 0;

		int i = 0;
		foreach (Transform child in paths.transform)
		{
			points[i++] = child;
		}

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

	private Vector3 posBefore = Vector3.zero;
	void Update()
	{
		// Choose the next destination point when the agent gets
		// close to the current one.

		if (!agent.pathPending && agent.remainingDistance < 0.5f && points != null && destPoint != -1)
			GotoNextPoint();


		transform.rotation = HelperFunctions.LookAt2D(agent.velocity.normalized.normalized);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);

	}


	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			agent.destination = points[0].position;
			destPoint = -1;
			transform.position = start;
			transform.gameObject.SetActive(false);
		}
	}
}
