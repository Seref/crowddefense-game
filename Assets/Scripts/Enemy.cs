using Assets.Scripts.Path;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	Vector3 start;
	NavMeshAgent agent;

	private Path paths;
	private List<Vector2> points;
	private int destPoint = 0;
	private StatsManager statsManager;
    public AudioClip clip;
    private AudioSource audioSource;

	void Awake()
	{
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		agent.autoBraking = false;
		agent.autoRepath = true;
	}

	public void StartPath(Path paths, StatsManager statsManager)
	{
		start = transform.position;
		this.paths = paths;
		destPoint = 0;

		this.statsManager = statsManager;

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
            AudioSource.PlayClipAtPoint(clip, gameObject.transform.position);
            agent.destination = points[0];
			destPoint = -1;
			transform.position = start;
			transform.gameObject.SetActive(false);
			statsManager.Score += 1;
		}
	}
}
