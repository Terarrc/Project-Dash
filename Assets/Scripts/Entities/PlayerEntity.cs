using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
	public TimedDurationObject doubleJumpParticle;
	public TimedDurationObject dashParticle;

	// Inspector variables
	public float doubleJumpSpeed;
	public float ratioStopJump;
	public float dashSpeed;
	public float dashDuration;
	public float groundedDashDelay;
	public float wallJumpSpeed;
	public float wallJumpSpeedX;
	public float bufferGroundedTime;

	// Grounded 
	private float timerBufferGrounded;

	// Jump variables
	private bool isGroundJumping = false;
	private bool canDoubleJump = true;

	// Dash variables
	private bool canDash = true;
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
				// Put 0 at the energy layer to disable blockage
				layerBlock &= (((1 << LayerMask.NameToLayer("Vertical Energy Field")) + (1 << LayerMask.NameToLayer("Horizontal Energy Field"))) ^ int.MaxValue);
				timerDash = dashDuration;
				timerDashParticles = 0;
				canDash = false;
				wantedSpeedX = dashSpeed * GetDirectionX();
				currentSpeedX = wantedSpeedX;

				// If grounded, avoid spam dash
				if (isGrounded)
					timerGroundedDash = groundedDashDelay;

				animator.SetTrigger("StartDash");
				animator.SetBool("Dashing", true);
			}
			else
			{
				// Put 1 at the energy layer to enable blockage
				layerBlock |= ((1 << LayerMask.NameToLayer("Vertical Energy Field")) + (1 << LayerMask.NameToLayer("Horizontal Energy Field")));
				animator.SetTrigger("StopDash");
				animator.SetBool("Dashing", false);
				currentSpeedY = 0;
				lockAxisY = false;
			}
		}
	}
	private float timerDash;
	private float timerDashParticles;
	private float timerGroundedDash;

	// Wall sliding variables
	private bool isWallSliding = false;
	private bool IsWallSliding
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
				sprite.flipX = currentSpeedX > 0;
				lockAxisX = true;
				wantedSpeedY = -1f;
				currentSpeedX = 0;
				animator.SetBool("Wall Slide", true);
			}
			else
			{
				lockAxisX = false;
				wantedSpeedY = -100;
				animator.SetBool("Wall Slide", false);
			}
		}
	}
    
	public new void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		float time = Time.deltaTime * 1000f;

		// Decrease dash timer
		if (IsDashing)
		{
			lockAxisY = true;

			timerDash -= time;
			timerDashParticles -= time;
			if (timerDash <= 0)
			{
				IsDashing = false;
			}
			if (timerDashParticles <= 0)
			{ 
				// Generate particle every 30ms
				TimedDurationObject particle = Instantiate(dashParticle, transform.position, Quaternion.identity);
				particle.FlipX(sprite.flipX);

				timerDashParticles = 30;
			}
		}

		// Check if the conditions for wall sliding are still ok
		if (IsWallSliding)
		{
			if (isGrounded)
				IsWallSliding = false;
			else
			{
				float height = boxCollider.bounds.size.y;
				float halfWidth = (boxCollider.bounds.size.x / 2);

				// Get the current position
				float positionX = transform.position.x;
				float positionY = transform.position.y;

				// Check if there is still a energy field
				Vector2 pointA, pointB;
				if (GetDirectionX() > 0)
				{
					pointA = new Vector2(positionX - halfWidth - 0.1f, positionY + height);
					pointB = new Vector2(positionX - halfWidth, positionY);
				}
				else
				{
					pointA = new Vector2(positionX + halfWidth + 0.1f, positionY + height);
					pointB = new Vector2(positionX + halfWidth, positionY);
				}

				// If no energy field is here, unstuck the player
				if (!Physics2D.OverlapArea(pointA, pointB, (1 << LayerMask.NameToLayer("Vertical Energy Field")) + (1 << LayerMask.NameToLayer("Vertical Energy Ground"))))
					IsWallSliding = false;
			}
		};

		// Descrease fall 
		if (!isGrounded && timerBufferGrounded > 0)
			timerBufferGrounded -= time;

		// Decrease dash buffer
		if (timerGroundedDash > 0)
			timerGroundedDash -= time;

		// Update movements
		base.Update();

		// Disable ground jumping if falling
		if (currentSpeedY <= 0)
			isGroundJumping = false;

		// Reset double jump and dash
		if (isGrounded)
		{
			timerBufferGrounded = bufferGroundedTime;
			if (timerGroundedDash <= 0)
				canDash = true;
			canDoubleJump = true;
		}
		else if (currentSpeedY != 0)
		{
			timerGroundedDash = 0;
		}
	}

	public override bool Move(Vector2 scale)
	{
		if (IsDashing)
			return false;

		return base.Move(scale);
	}

	public override bool Jump()
	{
		if (IsDashing)
			return false;

		// Check if we are grounded from the buffer
		if (timerBufferGrounded > 0 && !isGrounded)
		{
			isGrounded = true;
		}

		// Check if we are wall sliding for wall jump
		if (IsWallSliding)
		{
			IsWallSliding = false;
			currentSpeedY = wallJumpSpeed;

			currentSpeedX = wallJumpSpeedX * GetDirectionX();

			return true;
		}
			
		var jumped = base.Jump();
		if (jumped)
		{
			isGroundJumping = true;
			canDash = true;
			timerBufferGrounded = 0;
		}

		// Check for double jump
		if (!jumped && canDoubleJump && affectedByGravity)
		{
			currentSpeedY = doubleJumpSpeed;

			if ((wantedSpeedX < 0 && currentSpeedX > 0) && (wantedSpeedX > 0 && currentSpeedX < 0))
				currentSpeedX *= -1;

			canDoubleJump = false;
			isGroundJumping = false;
			timerBufferGrounded = 0;

			// Send event
			var hasJumped = new UnitHasJumpedEvent(this);
			hasJumped.execute();

			// Generate particle
			Instantiate(doubleJumpParticle, transform.position, Quaternion.identity);

			animator.SetTrigger("Jumped");

			return true;
		}

		return jumped;
	}

    public override bool StopJump()
    {
		if (isGroundJumping)
		{
			isGroundJumping = false;
			currentSpeedY  *= ratioStopJump;

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
		}
		return false;
	}

	private bool Dash()
	{
		if (canDash)
		{
			if (IsWallSliding)
				IsWallSliding = false;

			IsDashing = true;
			return true;
		}

		return false;
	}

	protected override float CollideX(Collider2D collider, float positionX)
	{
		positionX = base.CollideX(collider, positionX);

		if ((collider.gameObject.layer == LayerMask.NameToLayer("Vertical Energy Field") && ((layerBlock & (1 << LayerMask.NameToLayer("Vertical Energy Field"))) != 0)) || 
			collider.gameObject.layer == LayerMask.NameToLayer("Vertical Energy Ground"))
		{
			if (currentSpeedX < 0)
				TouchEnergyVertical(-1);
			if (currentSpeedX > 0)
				TouchEnergyVertical(1);
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Vertical Energy Field"))
			TouchEnergyField();

		return positionX;
	}

	protected override float CollideY(Collider2D collider, float positionY)
	{
		float oldSpeedY = currentSpeedY;

		positionY = base.CollideY(collider, positionY);

		if ((collider.gameObject.layer == LayerMask.NameToLayer("Horizontal Energy Field") && ((layerBlock & LayerMask.NameToLayer("Horizontal Energy Field")) != 0)) ||
			collider.gameObject.layer == LayerMask.NameToLayer("Horizontal Energy Ground"))
		{
			if (oldSpeedY < 0)
				TouchEnergyHorizontal(-1);
			if (oldSpeedY > 0)
				TouchEnergyHorizontal(1);
		}

		if (collider.gameObject.layer == LayerMask.NameToLayer("Horizontal Energy Field"))
			TouchEnergyField();

		return positionY;
	}

	protected void TouchEnergyVertical(float direction)
	{
		if (!isGrounded)
			IsWallSliding = true;
	}
	protected void TouchEnergyHorizontal(float direction)
	{

	}

	protected void TouchEnergyField()
	{
		canDash = true;
		canDoubleJump = true;
	}
}
