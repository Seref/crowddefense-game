using UnityEngine;

public class PlayerController2D : MonoBehaviour
{

	public float walkSpeed = 2;
	public float runSpeed = 6;
	private Animator animator;
	private Rigidbody2D rb;
	private SpriteRenderer sr;

	private int height = 0;
	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
				
		height = Mathf.RoundToInt(sr.size.y * 10f)/3;
	}

	void LateUpdate()
	{		

		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
		bool running = false; //Input.GetKey(KeyCode.LeftShift);		
		Move(input, running);
	}

	void Move(Vector3 input, bool running)
	{
		animator.SetBool("up", false);
		animator.SetBool("down", false);
		animator.SetBool("left", false);
		animator.SetBool("right", false);

		if (input.x == 1)
		{
			animator.SetBool("right", true);
		}
		else if (input.x == -1)
		{
			animator.SetBool("left", true);
		}
		else if (input.y == 1)
		{
			animator.SetBool("up", true);
		}
		else if (input.y == -1)
		{
			animator.SetBool("down", true);
		}

		float targetSpeed = ((running) ? runSpeed : walkSpeed) * input.magnitude;
		Vector2 direction = input.normalized * targetSpeed;
		
		rb.velocity = direction;

	}	
}