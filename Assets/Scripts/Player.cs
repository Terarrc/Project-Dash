using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	#region variables
	public SpriteRenderer doubleJumpParticle;
	public SpriteRenderer dashParticle;

	public float doubleJumpSpeed;
	public float dashSpeed;
	public float dashDuration;
	public float groundedDashDelay;
	public float wallJumpSpeed;
	public float wallJumpSpeedX;
	public float wallSlideSpeed;
	public float bufferGroundedTime;

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
				body.velocity = new Vector2(wallJumpSpeedX * GetDirectionX(), wallJumpSpeed);
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
				gameObject.layer = LayerMask.NameToLayer("Energy Projectile");
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
	#endregion
	
	void Start()
	{

	}

	new void Update()
    {
		base.Update();

		if (body.velocity.y <= 0)
			IsDoubleJumping = false;

		if (isGrounded)
		{
			if (!IsDashing)
			{
				timerBufferGrounded = bufferGroundedTime;
				canDoubleJump = true;
			}

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
			body.velocity = new Vector2(dashSpeed * GetDirectionX(), 0);

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
		}

		if (IsWallSliding)
		{
			int layerEnergy = (1 << LayerMask.NameToLayer("Energy Field")) + (1 << LayerMask.NameToLayer("Energy Ground"));
			bool touchEnergyField = Physics2D.OverlapCircle(body.position + new Vector2(boxCollider.bounds.size.x / 2 * -GetDirectionX(), boxCollider.bounds.size.y / 2), boxCollider.bounds.size.x / 2, layerEnergy);

			if (!touchEnergyField || isGrounded)
			{
				IsWallSliding = false;
			}
			else
				body.velocity = new Vector2(0, -wallSlideSpeed);
		}

		if (IsWallJumping)
		{
			timerWallJump -= Time.deltaTime;
			if (timerWallJump <= 0)
				IsWallJumping = false;
		}
	}

	public void OnCollisionStay2D(Collision2D collision)
	{
		if (!IsWallSliding && !IsWallJumping && !isGrounded && (collision.gameObject.layer == LayerMask.NameToLayer("Energy Field") || collision.gameObject.layer == LayerMask.NameToLayer("Energy Ground")))
		{
			ColliderDistance2D colliderDistance = collision.collider.Distance(collision.otherCollider);

			// Check if the collision is horizontal
			if (Mathf.Approximately(Vector2.Angle(colliderDistance.normal, Vector2.up), 90))
			{
				sprite.flipX = collision.GetContact(0).point.x - transform.position.x > 0;
				IsWallSliding = true;
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

		else if (canDoubleJump)
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
}
