using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	#region variables
	protected PlayerController playerController;
	protected EnterRoomController enterRoomController;

	public SpriteRenderer doubleJumpParticle;
	public SpriteRenderer dashParticle;

	public float doubleJumpSpeed;
	public float dashSpeed;
	public float dashDuration;
	public float groundedDashDelay;
	public float wallJumpSpeed;
	public float wallJumpSpeedX;
    public float slideJumpSpeed;
	public float wallSlideSpeed;
    public float groundSlideSpeed;
	public float bufferGroundedTime;

	// In which room the player is
	private Room room;
	public Room Room {
		get
		{
			return room;
		}
		private set
		{
			room = value;
			IsDashing = false;
			IsJumping = false;
			IsDoubleJumping = false;
			canDash = true;
			canDoubleJump = true;
			body.velocity = Vector2.zero;
		}
 }


	private float timerBufferGrounded;

	private bool canDoubleJump;
	private bool isDoubleJumping;
	protected bool IsDoubleJumping
	{
		get
		{
			return isDoubleJumping;
		}
		set
		{
			isDoubleJumping = value;

			if (value)
			{
				canDoubleJump = false;
				body.velocity = new Vector2(body.velocity.x, doubleJumpSpeed);

				if (animator != null)
					animator.SetTrigger("Jumped");

				// Generate particle
				if (doubleJumpParticle != null)
					Instantiate(doubleJumpParticle, body.position + (Vector2.down * (boxCollider.bounds.size.y / 2)), Quaternion.identity);
			}
		}
	}

	// in this timer, the player can't stick to a wall again
	private readonly float wallJumpTime = 0.2f;
	private float timerWallJump;
	private bool isWallJumping;
	protected bool IsWallJumping
	{
		get
		{
			return isWallJumping;
		}
		set
		{
			isWallJumping = value;
			if (value)
			{
				IsWallSliding = false;
				body.velocity = new Vector2(wallJumpSpeedX * GetDirection().x, wallJumpSpeed);
				timerWallJump = wallJumpTime;
			}
		}
	}

    // in this timer, the player can't stick to a slide again
    private readonly float slideJumpTime = 0.35f;
    private float timerSlideJump;
    private bool isSlideJumping;
    protected bool IsSlideJumping
    {
        get
        {
            return isSlideJumping;
        }
        set
        {
            isSlideJumping = value;
            if (value)
            {
                IsGroundSliding = false;
                body.velocity = new Vector2(body.velocity.x, slideJumpSpeed);
                timerSlideJump = slideJumpTime;
            }
        }
    }

    // Dash variables
    private bool canDash;
	private bool isDashing = false;
	private bool IsDashing
	{
		get
		{
			return isDashing;
		}
		set
		{
			isDashing = value;
			if (value)
			{
				if (IsWallSliding)
					IsWallSliding = false;

				// Disable collision
				gameObject.layer = LayerMask.NameToLayer("Energy Dash");
				body.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

				timerDash = dashDuration;
				timerDashParticles = 0;
				canDash = false;

				// If grounded, avoid spam dash
				if (IsGrounded)
					timerGroundedDash = groundedDashDelay;

				animator.SetTrigger("StartDash");
				animator.SetBool("Dashing", true);
			}
			else
			{
				gameObject.layer = LayerMask.NameToLayer("Player");
				body.constraints = RigidbodyConstraints2D.FreezeRotation;

				animator.SetTrigger("StopDash");
				animator.SetBool("Dashing", false);
			}
		}
	}
	private float timerDash;
	private float timerDashParticles;
	private float timerGroundedDash;

	// Wall sliding variables
	private bool isWallSliding;
	protected bool IsWallSliding
	{
		get
		{
			return isWallSliding;
		}
		set
		{
			isWallSliding = value;
			if (value)
			{
				body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
				body.gravityScale = 0;
				if (animator != null)
					animator.SetBool("Wall Slide", true);
			}
			else
			{
				body.constraints = RigidbodyConstraints2D.FreezeRotation;
				body.gravityScale = 1;
				if (animator != null)
					animator.SetBool("Wall Slide", false);
			}
		}
	}

    private bool isGroundSliding;
    protected bool IsGroundSliding
    {
        get
        {
            return isGroundSliding;
        }
        set
        {
            isGroundSliding = value;
            if (value)
            {
                body.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                if (animator != null)
                    animator.SetBool("Ground Slide", true);
            }
            else
            {
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (animator != null)
                    animator.SetBool("Ground Slide", false);
            }
        }
    }

    private float timerDropping;
	private bool isDropping;
	public bool IsDropping
	{
		get
		{
			return isDropping;// !Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("One Way Platform"));
		}
		set
		{
			if (value)
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("One Way Platform"), true);
			else if (isDropping)
			{
				timerDropping = 0.2f;
			}
			isDropping = value;
		}
	}

	public override bool IsGrounded
	{
		get
		{
			return isGrounded;
		}
		protected set
		{
			base.IsGrounded = value;
			if (value)
			{
				if (!IsDashing)
				{
					timerBufferGrounded = bufferGroundedTime;
					canDoubleJump = true;
				}
			}
		}
	}
	#endregion

	new void Awake()
	{
		base.Awake();

		playerController = GetComponent<PlayerController>();
		enterRoomController = GetComponent<EnterRoomController>();
	}

	new void Update()
    {
		base.Update();

		if (body.velocity.y <= 0)
			IsDoubleJumping = false;
		
		// Timer when dropping to avoid break dance
		if (timerDropping > 0)
		{
			timerDropping -= Time.deltaTime;
			if (timerDropping <= 0)
				Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("One Way Platform"), false);
		}

		if (IsGrounded)
		{
			if (timerGroundedDash > 0)
			{
				timerGroundedDash -= Time.deltaTime;
			}
			if (timerGroundedDash <= 0)
				canDash = true;
		}
		else
		{
			timerGroundedDash = 0;

			if (timerBufferGrounded > 0)
				timerBufferGrounded -= Time.deltaTime;
		}

		if (IsDashing)
		{
			body.velocity = dashSpeed * GetDirection();

			timerDash -= Time.deltaTime;
			timerDashParticles -= Time.deltaTime;
			if (timerDash <= 0)
			{
				IsDashing = false;
			}
			if (timerDashParticles <= 0)
			{
				// Generate particle every 30ms
				SpriteRenderer particle = Instantiate(dashParticle, transform.position, Quaternion.identity);
				particle.flipX = sprite.flipX;

				timerDashParticles = 0.03f;
			}

            // Draw Overlap with energy field to reset dash 
            if (isDashing)
            {
                int layerEnergy = (1 << LayerMask.NameToLayer("Energy Field"));
                bool inEnergyField = Physics2D.OverlapBox(Position, new Vector2(Size.x / 2, Size.y / 2), 0, layerEnergy);

                if (inEnergyField) // BUG : always true when dashing, idk why, Flouz help
                {
					ToucheEnergyField();
				}
                    
            }          
        }

        if (IsWallSliding)
		{
			int layerEnergy = (1 << LayerMask.NameToLayer("Energy Field")) + (1 << LayerMask.NameToLayer("Energy Ground"));
			bool touchEnergyField = Physics2D.OverlapCircle(body.position + (boxCollider.bounds.size.x / 2) * -GetDirection(), boxCollider.bounds.size.x / 2, layerEnergy);

			if (!touchEnergyField || isGrounded)
				IsWallSliding = false;
			else
				body.velocity = new Vector2(0, Mathf.MoveTowards(body.velocity.y, -wallSlideSpeed, acceleration.y * Time.deltaTime));
		}

        if (IsGroundSliding)
        {
            int layerEnergy = (1 << LayerMask.NameToLayer("Energy Field")) + (1 << LayerMask.NameToLayer("Energy Ground"));
            Vector2 v = new Vector2(body.position.x, body.position.y - boxCollider.bounds.size.y / 2);
            bool touchEnergyField = Physics2D.OverlapCircle(v, boxCollider.bounds.size.x / 2, layerEnergy);

            if (!touchEnergyField)
                IsGroundSliding = false;
            else
            {
                if(GetDirection() == Vector2.right)
                    body.velocity = new Vector2(groundSlideSpeed, 0);
                else
                    body.velocity = new Vector2(-groundSlideSpeed, 0);
            }
                
        }

        // Timers for energy slide
        if (IsWallJumping)
		{
            timerWallJump -= Time.deltaTime;
			if (timerWallJump <= 0)
				IsWallJumping = false;
		}

        if (IsSlideJumping)
        {
            timerSlideJump -= Time.deltaTime;
            if (timerSlideJump <= 0)
                IsSlideJumping = false;
        }
    }

    // Draw Gizmos
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(body.position, new Vector2(boxCollider.bounds.size.x / 2, boxCollider.bounds.size.y / 2));   
        */
    }

    protected new void OnCollisionStay2D(Collision2D collision)
	{
		base.OnCollisionStay2D(collision);

		if (!IsWallSliding && !IsGroundSliding && !IsWallJumping && !IsSlideJumping && !IsGrounded && (collision.gameObject.layer == LayerMask.NameToLayer("Energy Field") || collision.gameObject.layer == LayerMask.NameToLayer("Energy Ground")))
		{
			ColliderDistance2D colliderDistance = collision.collider.Distance(collision.otherCollider);
			float angle = Vector2.Angle(colliderDistance.normal, Vector2.up);

			// Reset Dash
			ToucheEnergyField();

            // Check if the collision is horizontal
            if (angle > 89 && angle < 91)
			{
				sprite.flipX = collision.GetContact(0).point.x - Position.x > 0;
				IsWallSliding = true;
			}

            // Check if the collision is vertical
            if (angle > -1 && angle < 1)
            {
                //sprite.flipX = collision.GetContact(0).point.x - Position.x > 0;
                IsGroundSliding = true;
            }
        }

    }

	public void SetVelocity(Vector2 velocity)
	{
		body.velocity = velocity;
	}

	public void EnterRoom(Room room, Vector2 direction)
	{
		Room = room;
		playerController.enabled = false;
		enterRoomController.enabled = true;
		enterRoomController.EntryDirection = direction;
	}

	public void EnterRoomOver()
	{
		enterRoomController.enabled = false;
		playerController.enabled = true;

		SetRespawnPoint(transform.position);
	}

	public void SetRespawnPoint(Vector3 position)
	{
		if (health != null)
		{
			PlayerHealth playerHealth = health as PlayerHealth;
			if (playerHealth != null)
			{
				playerHealth.RespawnPoint = position;
			}
		}
	}

	private void ToucheEnergyField()
	{
		canDash = true;
	}

	public override bool Move(Vector2 input)
	{
		if (IsDashing || IsWallSliding || IsGroundSliding)
			return false;

		if (!IsGrounded && isSlideJumping && ((input.x <= 0 && GetDirection().x < 0) || (input.x >= 0 && GetDirection().x > 0)))
			body.velocity = body.velocity;// new Vector2(Mathf.MoveTowards(body.velocity.x, speed.x * input.x, acceleration.x / 20 * Time.deltaTime), body.velocity.y);
		else
			body.velocity = new Vector2(Mathf.MoveTowards(body.velocity.x, speed.x * input.x, acceleration.x * Time.deltaTime), body.velocity.y);

		if (input.x != 0)
			sprite.flipX = input.x > 0 ? false : true;

		if (animator != null)
		{
			animator.SetBool("Moving X", input.x != 0);
			animator.SetBool("Moving Y", input.y != 0);
		}

		return true;
	}

	public override bool Jump()
	{
		if (IsWallSliding)
		{
			IsWallJumping = true;
			return true;
		}

        if (IsGroundSliding)
        {
            IsSlideJumping = true;
            return true;
        }

		// Check if we are grounded from the buffer
		if (timerBufferGrounded > 0 || IsGrounded)
		{
			IsJumping = true;
			return true;
		}

		else if (canDoubleJump && !IsDashing)
		{
			IsDoubleJumping = true;
			return true;
		}

		return false;
	}

	public override bool Action(int index)
	{
		switch (index)
		{
			case 1:
				return Dash();
			default:
				break;
		}

		return false;
	}

	private bool Dash()
	{
		if (canDash)
		{
			IsDashing = true;
			return true;
		}

		return false;
	}

	public override bool Drop(bool value)
	{
		IsDropping = value;
		return false;
	}
}
