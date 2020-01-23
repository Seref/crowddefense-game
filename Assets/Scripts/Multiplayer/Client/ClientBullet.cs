using UnityEngine;

namespace Assets.Scripts.Multiplayer.Client
{
	public class ClientBullet : MonoBehaviour
	{
		public float Speed = 10;

		private Rigidbody2D rigidBody2D;
		private SpriteRenderer sr;
		private bool wasVisible = false;

		void OnEnable()
		{
			rigidBody2D = GetComponent<Rigidbody2D>();
			sr = GetComponent<SpriteRenderer>();
			wasVisible = false;
			rigidBody2D.velocity = transform.up * Speed;
		}

		private void Update()
		{
			if (sr.isVisible)
			{
				wasVisible = true;
			}
			if (!sr.isVisible && wasVisible)
			{
				wasVisible = false;
				rigidBody2D.velocity = new Vector3(0, 0, 0);
				transform.gameObject.SetActive(false);
			}
		}
	}
}
