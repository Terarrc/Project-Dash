using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity DoubleJumpParticle;

	public float doubleJumpSpeed;
	public float ratioStopJump;
	public float dashSpeed;
	public float dashDuration;
	private bool isGroundJumping = false;
	private bool hasDoubleJumped = false;

	private bool hasDashed = false;
	private float dashScale;
	private float preDashSpeed;
	private float timerDash;

	public override void Update()
	{
		// Decrease dash buffer
		if (timerDash > 0)
		{
			float time = Time.deltaTime * 1000f;
			timerDash -= time;
			if (timerDash <= 0)
			{
				animator.SetTrigger("DashStop");
				currentSpeedX = preDashSpeed * 1.2f;
				body.velocity = new Vector2(body.velocity.x, 0);
			}
			else
			{
				currentSpeedX = dashSpeed * dashScale;
			}
		}

		base.Update();

		// Reset double jump when on the ground
		if (isGrounded)
		{
			hasDoubleJumped = false;
			hasDashed = false;
		}
	}

	public override bool Jump()
	{
		isGroundJumping = true;

		var jumped = base.Jump();

		if (!jumped && !hasDoubleJumped && !verticalMoveEnabled)
		{
			body.velocity = new Vector2(body.velocity.x, doubleJumpSpeed);
			hasDoubleJumped = true;
			isGroundJumping = false;

			// Send event
			var hasJumped = new UnitHasJumpedEvent(this);
			hasJumped.execute();

			// Generate particle
			Instantiate(DoubleJumpParticle, transform.position, Quaternion.identity);

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

	public override bool Dash(float scale)
	{
		if (!hasDashed)
		{
			preDashSpeed = currentSpeedX;
			timerDash = dashDuration;
			dashScale = scale;
			hasDashed = true;

			animator.SetTrigger("DashStart");

			return true;
		}

		return false;
	}
}
