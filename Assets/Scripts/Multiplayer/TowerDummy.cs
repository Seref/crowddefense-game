using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Multiplayer
{
	public class TowerDummy : MonoBehaviour
	{
		public FloatingCounter floatCounter;
		public int CoolDownTime = 20;

		private AudioSource audioSource;
		private new Rigidbody2D rigidbody2D;
		private GameObject additionalUI;

		void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			rigidbody2D = GetComponent<Rigidbody2D>();
			additionalUI = GameObject.FindGameObjectWithTag("AdditionalUI");
		}		

		public void UpdateData(Vector3 position, float rotation)
		{
			transform.position = position;
			rigidbody2D.rotation = rotation;
		}


		public void ShootBullet(int duration)
		{
			PlayAudio();
			ShowCountDown(duration);
		}

		public void PlayAudio()
		{
			audioSource.Play();
		}

		public void Fire()
		{
			GameObject bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
			if (bullet != null)
			{
				audioSource.Play();
				bullet.transform.position = transform.position + transform.up * 1.25f;
				bullet.transform.rotation = transform.rotation;
				bullet.SetActive(true);

				var bulletScript = bullet.GetComponent<Bullet>();
				bulletScript.Speed = 20;
				bulletScript.Fire();

				ShowCountDown(CoolDownTime);
			}
		}

		public void ShowCountDown(int duration)
		{
			var Text = Instantiate(floatCounter, new Vector3(-1000, -1000, 0), Quaternion.identity, additionalUI.transform);
			Text.Show(duration, transform, GetComponent<SpriteRenderer>().bounds.size.y);
		}
	}
}
