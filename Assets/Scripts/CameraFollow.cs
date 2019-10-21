using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

	public bool lockCursor;	
	public Transform target;
	public float dstFromTarget = 2;	
	
	void Start()
	{
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void LateUpdate()
	{		
		transform.position = target.position - transform.forward * dstFromTarget;
	}

}