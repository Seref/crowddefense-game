using Assets.Scripts.Multiplayer;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClientController : MonoBehaviour
{
	public float CoolDownTime = 20.0f;
	public FloatingCounter floatCounter;
	public ClientManager clientManager;

	private Rigidbody2D rigidBody;
	private GameManager gameManager;
    private AudioSource audioSource;

    private bool coolDown = false;

	void Start()
	{
        audioSource = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody2D>();
		gameManager = FindObjectOfType<GameManager>();
	}

	void LateUpdate()
	{
		if (Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
		{
			var mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);
			rigidBody.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z + 90;
			clientManager.AddEvent(DataClientInputType.MoveTower, mousePos);
			if (Input.GetButtonDown("Fire1"))
				clientManager.AddEvent(DataClientInputType.ShootBullet, mousePos);
		}
	}	

	private IEnumerator CoolDownCounter()
	{
		yield return new WaitForSeconds(CoolDownTime);
		coolDown = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			transform.gameObject.SetActive(false);
			gameManager.GameOver();
		}
	}

}
