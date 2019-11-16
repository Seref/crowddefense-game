using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2D : MonoBehaviour
{
	Vector3 start;
	NavMeshAgent agent;

	private Path paths;
	private List<Vector2> points;
	private int destPoint = 0;
	private GameManager2D gameManager;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		agent.autoBraking = false;
		agent.autoRepath = true;
	}

	public void StartPath(Path paths, GameManager2D gameManager)
	{
		start = transform.position;
		this.paths = paths;
		destPoint = 0;

		this.gameManager = gameManager;

		points = paths.PointList;

		GotoNextPoint();
	}

	void GotoNextPoint()
	{
		// Returns if no points have been set up
		if (points.Capacity == 0)
			return;

		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint];

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Capacity;
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
			agent.destination = points[0];
			destPoint = -1;
			transform.position = start;
			transform.gameObject.SetActive(false);
			gameManager.Score += 1;
		}
	}
}
