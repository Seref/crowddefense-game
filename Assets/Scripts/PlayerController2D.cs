using UnityEngine;

public class PlayerController2D : MonoBehaviour
{

	public float walkSpeed = 2;
	public float runSpeed = 6;		
	
	void LateUpdate()
	{
		// input
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);		
		bool running = Input.GetKey(KeyCode.LeftShift);

		Move(input, running);		
	}

	void Move(Vector3 input, bool running)
	{								
		float targetSpeed = ((running) ? runSpeed : walkSpeed) * input.magnitude;
		Vector2 direction = input.normalized;

		transform.position = new Vector3(transform.position.x + direction.x * targetSpeed, transform.position.y + direction.y * targetSpeed, transform.position.z);
				
	}	
	
}