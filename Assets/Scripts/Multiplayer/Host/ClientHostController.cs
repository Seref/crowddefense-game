using Assets.Scripts.Multiplayer.Client;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Multiplayer.Host
{
	public class ClientHostController : MonoBehaviour
	{
		public FloatingCounter floatCounter;

		private AudioSource audioSource;
		private new Rigidbody2D rigidbody2D;

		public HostManager hostManager;

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

		void Update()
		{
			if (Input.GetKey(KeyCode.LeftControl) && !EventSystem.current.IsPointerOverGameObject())
			{
				var mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos);
				rigidBody2D.rotation = HelperFunctions.LookAt2D(transform.position, mousePos).eulerAngles.z + 90;

				if (Input.GetButtonDown("Fire1"))
					Fire();
			}
		}

		private void Fire()
		{
			GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
			if (bullet != null && !coolDown)
			{
				coolDown = true;
				audioSource.Play();
				bullet.transform.position = transform.position + transform.up * 1.25f;
				bullet.transform.rotation = transform.rotation;
				bullet.SetActive(true);
				var bulletScript = bullet.GetComponent<Bullet>();
				bulletScript.Speed = 20;
				bulletScript.Fire();

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

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Enemy")
			{
				transform.gameObject.SetActive(false);
				gameManager.GameOver();
			}
		}
	}
}
