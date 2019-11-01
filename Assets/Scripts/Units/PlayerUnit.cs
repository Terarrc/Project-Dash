using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity doubleJumpParticle;
	public TimedDurationEntity dashParticle;
	public TimedDurationEntity createdField;

	public float doubleJumpSpeed;
	public float ratioStopJump;
	public float dashSpeed;
	public float dashDuration;
	public float groundedDashDelay;
	public float bufferGroundedTime;
	private float timerGroundedDash;
	private bool isGroundJumping = false;

	private bool canDoubleJump = true;
	private bool canDash = true;
	private bool isDashing = false;
	private bool canCreatePlatform = true;
	private bool canCreateWall = true;
    
	private float dashScale;
	private float preDashSpeed;
	private float timerDash;
	private float timerDashParticles;
	private float timerBufferGrounded;


	public override void Update()
	{
		float time = Time.deltaTime * 1000f;
		isDashing = timerDash > 0;

		// Decrease dash timer
		if (isDashing)
		{
			lockAxisY = true;

			
			timerDash -= time;
			timerDashParticles -= time;
			if (timerDash <= 0)
			{
				animator.SetTrigger("StopDash");
				animator.SetBool("Dashing", false);
				currentSpeedX = preDashSpeed + (currentSpeedX / 2);
				currentSpeedY = 0;
				lockAxisY = false;
			}
			if (timerDashParticles <= 0)
			{ 
				// Generate particle every 30ms
				TimedDurationEntity particle = Instantiate(dashParticle, transform.position, Quaternion.identity);
				particle.FlipX(sprite.flipX);

				timerDashParticles = 30;
			}
		}

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
			canCreateWall = true;
		}
		else
		{
			timerGroundedDash = 0;
		}
	}

	public override bool Move(Vector2 scale)
	{
		if (isDashing)
			return false;

		return base.Move(scale);
	}

	public override bool Jump()
	{
		if (isDashing)
			return false;

		// Check if we are grounded from the buffer
		if (timerBufferGrounded > 0 && !isGrounded)
		{
			isGrounded = true;
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
				return CreateHorizontalField();
		}
		return false;
	}

	private bool Dash()
	{
		if (canDash)
		{
			preDashSpeed = currentSpeedX;
			timerDash = dashDuration;
			timerDashParticles = 0;
			dashScale = GetDirection();
			canDash = false;
			wantedSpeedX = dashSpeed * dashScale;
			currentSpeedX = wantedSpeedX;

			// If grounded, avoid spam dash
			if (isGrounded)
				timerGroundedDash = groundedDashDelay;


			animator.SetTrigger("StartDash");
			animator.SetBool("Dashing", true);

			return true;
		}

		return false;
	}

	private bool CreateHorizontalField()
    {
        if (!isGrounded && canCreatePlatform)
        {
            canCreatePlatform = false;
            Instantiate(createdField, transform.position, Quaternion.identity);
            return true;
        }

        return false;
    }

    private bool CreateVerticalField()
    {
        return false;
    }

}
