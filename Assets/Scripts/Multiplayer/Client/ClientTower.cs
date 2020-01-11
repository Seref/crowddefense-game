using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Multiplayer.Client
{
	public class ClientController : MonoBehaviour
	{
		public FloatingCounter floatCounter;

		private AudioSource audioSource;
		private new Rigidbody2D rigidbody2D;

		public ClientManager clientManager;

		void Start()
		{
			audioSource = GetComponent<AudioSource>();
			rigidbody2D = GetComponent<Rigidbody2D>();
		}

		public void PlayAudio()
		{
			audioSource.Play();
		}

		public void ShowCountDown(int duration)
		{
			var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, GameObject.FindWithTag("AdditionalUI").transform);
			Text.Show(duration, transform, GetComponent<SpriteRenderer>().bounds.size.y);
		}

		void LateUpdate()
		{
			if (Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
			{
				var mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos);
				rigidbody2D.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z + 90;
				clientManager.AddEvent(DataClientInputType.MoveTower, mousePos);
				if (Input.GetButtonDown("Fire1"))
					clientManager.AddEvent(DataClientInputType.ShootBullet, mousePos);
			}
		}

	}
}
