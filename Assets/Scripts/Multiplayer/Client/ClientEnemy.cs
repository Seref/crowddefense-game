using UnityEngine;

public class ClientEnemy : MonoBehaviour
{
	public AudioClip clip;

	private AudioSource audioSource;


	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private bool wasActive = false;

	public void UpdateData(Vector3 position, Quaternion rotation, bool isActive)
	{
		transform.position = position;
		transform.rotation = rotation;

		if (!isActive)
		{
			if (wasActive)
			{
				wasActive = false;
				PlayAudio();
			}
		}
		else
		{
			wasActive = true;
		}

		gameObject.SetActive(isActive);
	}


	public void PlayAudio()
	{
		AudioSource.PlayClipAtPoint(clip, transform.position);
	}

}
