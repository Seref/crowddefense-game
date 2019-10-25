using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{

	public bool lockCursor;
	public Transform target;

	public float smoothSpeed = 0.125f;

	public Vector2 cameraBounds = new Vector2(2, 2);

	private Bounds currentBoundingBox;
	private Vector3 desiredPosition;


	void Start()
	{
		currentBoundingBox = new Bounds(new Vector2(transform.position.x, transform.position.y), cameraBounds);
		desiredPosition = target.position;

		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void FixedUpdate()
	{
		if (!currentBoundingBox.Contains(new Vector2(target.position.x, target.position.y))) { 
			desiredPosition = target.position;
			currentBoundingBox = new Bounds(new Vector2(target.position.x, target.position.y), cameraBounds);
		}

		Vector3 smoothPosition = Vector2.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = new Vector3(smoothPosition.x, smoothPosition.y, transform.position.z);
	}

}