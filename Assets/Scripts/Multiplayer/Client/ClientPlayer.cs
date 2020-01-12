using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Multiplayer.Client
{
	public class ClientPlayer : MonoBehaviour
	{
		public FloatingCounter floatCounter;

		private AudioSource audioSource;
		private new Rigidbody2D rigidbody2D;

		private bool coolDown = false;
		private GameObject additionalUI;

		private ClientManager clientManager;
		public float CoolDownTime = 20.0f;

		void Start()
		{
			audioSource = GetComponent<AudioSource>();
			rigidbody2D = GetComponent<Rigidbody2D>();
			additionalUI = GameObject.FindWithTag("AdditionalUI");
			clientManager = FindObjectOfType<ClientManager>();
		}		

		public void ShowCountsDown(int duration)
		{
			var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("AdditionalUI").transform);
			Text.Show(duration, transform, GetComponent<SpriteRenderer>().bounds.size.y);
		}

		private Vector2 mousePos;
		void Update()
		{
			if (Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
			{
				mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos);
				rigidbody2D.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z + 90;

				if (Input.GetButtonDown("Fire1"))
				{					
					Fire();
				}				
			}
		}

		private void Fire()
		{			
			if (!coolDown)
			{
				coolDown = true;
				audioSource.Play();
				clientManager.AddFireEvent(DataClientInputType.TowerShoot, mousePos);				

				var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, additionalUI.transform);
				Text.Show(CoolDownTime, transform, GetComponent<SpriteRenderer>().bounds.size.y);
				StartCoroutine(CoolDownCounter());
			}
		}

		private IEnumerator CoolDownCounter()
		{
			yield return new WaitForSeconds(CoolDownTime);
			coolDown = false;
		}

	}
}
