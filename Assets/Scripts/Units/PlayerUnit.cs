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
	private float timerGroundedDash;
	private bool isGroundJumping = false;

	private bool canDoubleJump = true;
	private bool canDash = true;
	private bool canCreatePlatform = true;
	private bool canCreateWall = true;
    
	private float dashScale;
	private float preDashSpeed;
	private float timerDash;
	private float timerDashParticles;



	public override void Update()
	{
		// Decrease dash timer
		if (timerDash > 0)
		{
			float time = Time.deltaTime * 1000f;
			timerDash -= time;
			timerDashParticles -= time;
			if (timerDash <= 0)
			{
				animator.SetTrigger("StopDash");
				animator.SetBool("Dashing", false);
				currentSpeedX = preDashSpeed * 1.2f;
				body.velocity = new Vector2(body.velocity.x, 0);
			}
			else
			{
				currentSpeedX = dashSpeed * dashScale;
			}
			if (timerDashParticles <= 0)
			{ 
				// Generate particle every 30ms
				TimedDurationEntity particle = Instantiate(dashParticle, transform.position, Quaternion.identity);
				particle.FlipX(sprite.flipX);

				timerDashParticles = 30;
			}
		}

		// Decrease dash buffer
		if (timerGroundedDash > 0)
		{
			float time = Time.deltaTime * 1000f;
			timerGroundedDash -= time;
		}

		// Update movements
		bool wasGrounded = isGrounded;
		base.Update();

		// Reset double jump and dash
		if (isGrounded)
		{
			if (timerGroundedDash <= 0)
				canDash = true;
			canDoubleJump = true;
            canCreateWall = true;
		}
		else if (wasGrounded)
		{
			timerGroundedDash = 0;
			canDash = true;
		}
	}

	public override bool Jump()
	{
		if (timerDash > 0)
			return false;

		var jumped = base.Jump();
		if (jumped)
		{
			isGroundJumping = true;
			canDash = true;
		}

		if (!jumped && canDoubleJump && !verticalMoveEnabled)
		{
			body.velocity = new Vector2(body.velocity.x, doubleJumpSpeed);
			canDoubleJump = false;
			isGroundJumping = false;

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
		if (isGroundJumping && body.velocity.y > 0)
		{
			isGroundJumping = false;
			body.velocity = new Vector2(body.velocity.x, body.velocity.y * ratioStopJump);

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
