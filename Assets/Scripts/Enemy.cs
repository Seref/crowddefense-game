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
	public float Health = 1f;
	private int destPoint = 0;
	private StatsManager statsManager;
	public AudioClip clip;
	public AudioClip deathclip;
	public GameObject Explosion;

	private GameObject additionalLayer;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		agent.autoBraking = false;
		agent.autoRepath = true;
		additionalLayer = GameObject.FindGameObjectWithTag("Additional");
	}

	void OnEnable()
	{
		isDead = false;
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

	public virtual void SetHealth(float health)
	{
		GetComponent<SpriteRenderer>().color = new Color(0.2f + 0.8f * ((10 - health) / 10), 0f + 1.0f * ((10 - health) / 10), 0.3f + 0.7f * ((10 - health) / 10), 1f);
		Health = health;
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
			Health -= collision.gameObject.GetComponent<Bullet>().Damage;			
			SetHealth(Health);

			if (Health <= 0.0f)
				Die();
		}
	}


	private bool isDead = false;

	public void Die()
	{
		if (!isDead)
		{
			var position = transform.position;
			isDead = true;
			AudioSource.PlayClipAtPoint(clip, gameObject.transform.position, (SettingsManager.Instance.GetCurrentSettings().MasterSound / 100.0f)*0.25f);
			AudioSource.PlayClipAtPoint(deathclip, gameObject.transform.position, (SettingsManager.Instance.GetCurrentSettings().MasterSound / 100.0f)*1.25f);
			Health = 0.3f;
			transform.position = start;
			agent.destination = points[0];
			destPoint = -1;
			statsManager.Score += 1;
			transform.gameObject.SetActive(false);

			
			var e = Instantiate(Explosion, position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), additionalLayer.transform);
			var scale = Random.Range(0.1f, 0.5f);
			e.transform.localScale = new Vector2(scale, scale);
		}
	}
}
