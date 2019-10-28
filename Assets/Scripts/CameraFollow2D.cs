using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{

	public bool lockCursor;
	public Transform target;

	public float smoothSpeed = 0.125f;
	public float smoothPanSpeed = 0.25f;

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
			var currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			var offset = currentPosition - target.position;
			desiredPosition = currentPosition - (offset* smoothPanSpeed);
			currentBoundingBox = new Bounds(desiredPosition, cameraBounds);
		}

		Vector3 smoothPosition = Vector2.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = new Vector3(smoothPosition.x, smoothPosition.y, transform.position.z);
	}

}