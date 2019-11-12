using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	#region variables
	public SpriteRenderer doubleJumpParticle;
	public SpriteRenderer dashParticle;

	public float doubleJumpHeight;
	public float dashSpeed;
	public float dashDuration;
	public float groundedDashDelay;
	public float wallJumpHeight;
	public float wallJumpSpeedX;
	public float wallSlideSpeed;
	public float bufferGroundedTime;

	// In which room the player is
	private Room room;
	public Room Room {
		get
		{
			return room;
		}
		set
		{
			room = value;
			IsDashing = false;
			IsJumping = false;
			IsDoubleJumping = false;
			body.SetVelocity(Vector2.zero);
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
				body.SetVelocityY(Mathf.Sqrt(2 * doubleJumpHeight * Mathf.Abs(Physics2D.gravity.y)));

				if (animator != null)
					animator.SetTrigger("Jumped");

				// Generate particle
				if (doubleJumpParticle != null)
					Instantiate(doubleJumpParticle, Position + (Vector2.down * (boxCollider.bounds.size.y / 2)), Quaternion.identity);
			}
		}
	}

	// in this timer, the player can't stuck to a wall again
	private float wallJumpTime = 0.2f;
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
				body.SetVelocityX(Mathf.Sqrt(wallJumpSpeedX));
				body.SetVelocityY(Mathf.Sqrt(2 * wallJumpHeight * Mathf.Abs(Physics2D.gravity.y)));
				timerWallJump = wallJumpTime;
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
				body.Constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

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
				body.Constraints = RigidbodyConstraints2D.FreezeRotation;

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
				body.Constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
				body.gravityScale = 0;
				if (animator != null)
					animator.SetBool("Wall Slide", true);
			}
			else
			{
				body.Constraints = RigidbodyConstraints2D.FreezeRotation;
				body.gravityScale = 1;
				if (animator != null)
					animator.SetBool("Wall Slide", false);
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

	void Start()
	{

	}

	new void Update()
    {
		base.Update();

		if (body.Velocity.y <= 0)
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
			canDash = false;
			body.SetVelocity(dashSpeed* GetDirection());

			timerDash -= Time.deltaTime;
			timerDashParticles -= Time.deltaTime;
			if (timerDash <= 0)
			{
				IsDashing = false;
			}
			if (timerDashParticles <= 0)
			{
				// Generate particle every 30ms
				SpriteRenderer particle = Instantiate(dashParticle, Position, Quaternion.identity);
				particle.flipX = sprite.flipX;

				timerDashParticles = 0.03f;
			}
		}

		if (IsWallSliding)
		{
			int layerEnergy = (1 << LayerMask.NameToLayer("Energy Field")) + (1 << LayerMask.NameToLayer("Energy Ground"));
			bool touchEnergyField = Physics2D.OverlapCircle(Position + (boxCollider.bounds.size.x / 2) * -GetDirection(), boxCollider.bounds.size.x / 2, layerEnergy);

			if (!touchEnergyField || isGrounded)
			{
				IsWallSliding = false;
			}
			else
				body.SetVelocity(Vector2.down * wallSlideSpeed);
		}

		if (IsWallJumping)
		{
			timerWallJump -= Time.deltaTime;
			if (timerWallJump <= 0)
				IsWallJumping = false;
		}
	}

	protected void OnCollisionStay2D(Collision2D collision)
	{
		if (!IsWallSliding && !IsWallJumping && !IsGrounded && (collision.gameObject.layer == LayerMask.NameToLayer("Energy Field") || collision.gameObject.layer == LayerMask.NameToLayer("Energy Ground")))
		{
			ColliderDistance2D colliderDistance = collision.collider.Distance(collision.otherCollider);

			float angle = Vector2.Angle(colliderDistance.normal, Vector2.up);

			// Check if the collision is horizontal
			if (angle > 89 && angle < 91)
			{
				sprite.flipX = collision.GetContact(0).point.x - Position.x > 0;
				IsWallSliding = true;
			}	
		} 
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

	public override bool Move(Vector2 input)
	{
		if (IsDashing || IsWallSliding)
			return false;

		return base.Move(input);
	}

	public override bool Jump()
	{
		if (IsWallSliding)
		{
			IsWallJumping = true;
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
