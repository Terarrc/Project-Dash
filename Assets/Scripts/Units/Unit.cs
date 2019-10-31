using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IControls
{
    // All of the Unit's component
    protected Rigidbody2D body;
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;
	protected Animator animator;

	// Physic variable
	public float moveSpeedX, moveSpeedY;
	public float jumpSpeed;
	public float accelerationX, accelerationY;
	public bool verticalMoveEnabled;
	protected float currentSpeedX, currentSpeedY, wantedSpeedX, wantedSpeedY;
	protected float currentAccelerationX, currentAccelerationY;
	protected bool isGrounded = true;

	// On wake, get Unit's components
	void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	public virtual void Update()
	{
		// Less maneouvrability in the air
		if (isGrounded || verticalMoveEnabled)
		{
			currentAccelerationX = accelerationX;
			currentAccelerationY = accelerationY;
		}
		else
		{
			currentAccelerationX = accelerationX / 2;
			currentAccelerationY = accelerationY / 2;
		}

		float positionX = transform.position.x;
		float positionY = transform.position.y;

		// Update the speed X
		if (currentSpeedX < wantedSpeedX)
		{
			currentSpeedX = Mathf.Min(wantedSpeedX, currentSpeedX + (accelerationX * Time.deltaTime));
		}
		else if (currentSpeedX > wantedSpeedX)
		{
			currentSpeedX = Mathf.Max(wantedSpeedX, currentSpeedX - (accelerationX * Time.deltaTime));
		}

		// Update the speed Y
		if (verticalMoveEnabled)
		{
			if (currentSpeedY < wantedSpeedY)
			{
				currentSpeedY = Mathf.Min(wantedSpeedY, currentSpeedY + (accelerationY * Time.deltaTime));
			}
			else if (currentSpeedY > wantedSpeedY)
			{
				currentSpeedY = Mathf.Max(wantedSpeedY, currentSpeedY - (accelerationY * Time.deltaTime));
			}
		}
	

		// Update the position X
		if (!Mathf.Approximately(currentSpeedX, 0))
		{
			positionX += currentSpeedX * Time.deltaTime;
		}
		// Update the position Y
		if (verticalMoveEnabled)
		{
			if (!Mathf.Approximately(currentSpeedY, 0))
			{
				positionY += currentSpeedY * Time.deltaTime;
			}
		}

		transform.position = new Vector3(positionX, positionY);

		// Overlap ground check
		// Make a rectangle out of two edges (Vector 2) and if that rectangle overlaps with layer (8) it returns true
		if (boxCollider)
		{
			var hitboxHalf = boxCollider.bounds.size.x / 2;
			isGrounded = Physics2D.OverlapArea(
				new Vector2(transform.position.x - hitboxHalf, transform.position.y),
				new Vector2(transform.position.x + hitboxHalf, transform.position.y - 0.02f), 1 << 8);

			animator.SetBool("Grounded", isGrounded);
		}
		// For animation in the air
		if (body)
		{
			animator.SetFloat("SpeedY", body.velocity.y);
		}
    }

    // ========================================================================
    // Controls
    // ========================================================================
    public virtual bool Move(Vector2 direction)
	{
		if (!verticalMoveEnabled)
		{
			direction.y = 0;
		}

		// Check if move or stop
		if (direction == Vector2.zero)
		{
			wantedSpeedX = 0;
			wantedSpeedY = 0;

			animator.SetBool("Moving", false);

			return true;
		}
		else
		{
			wantedSpeedX = direction.x * moveSpeedX;
			wantedSpeedY = direction.y * moveSpeedY;

			sprite.flipX = direction.x < 0;
			animator.SetBool("Moving", true);

			return true;
		}
	}

	public virtual bool Jump()
	{
		if (isGrounded && !verticalMoveEnabled)
		{
			isGrounded = false;

			body.velocity = new Vector2(body.velocity.x, jumpSpeed);

			// Send event
			var hasJumped = new UnitHasJumpedEvent(this);
			hasJumped.execute();

			animator.SetTrigger("Jumped");

			return true;
		}

		return false;
	}
	
	public virtual bool StopJump()
	{
		return false;
	}

}
