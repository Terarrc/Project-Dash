using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity doubleJumpParticle;
	public TimedDurationEntity dashParticle;
	public TimedDurationEntity createdField;

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
				collideWithEnergy = false;
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
				collideWithEnergy = true;
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
				Debug.Log("Stop wall sliding");
			}
			else
			{
				lockAxisX = false;
				wantedSpeedY = -100;
				animator.SetBool("Wall Slide", false);
			}
		}
	}

	// Energy field creation variable
	private bool canCreateEnergyField = true;
    

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
				TimedDurationEntity particle = Instantiate(dashParticle, transform.position, Quaternion.identity);
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
				if (!Physics2D.OverlapArea(pointA, pointB, (1 << LayerMask.NameToLayer("Vertical Energy Fields")) + (1 << LayerMask.NameToLayer("Vertical Energy Ground"))))
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
			canCreateEnergyField = true;
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
			case 2:
				return CreateEnergyField();
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

	protected override void TouchEnergyVertical(float direction)
	{
		if (!isGrounded)
			IsWallSliding = true;
	}
	protected override void TouchEnergyHorizontal(float direction)
	{

	}

	protected override void TouchEnergyField()
	{
		canDash = true;
	}

	private bool CreateEnergyField()
    {
        return false;
    }

}
