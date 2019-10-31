using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity DoubleJumpParticle;

	public float doubleJumpSpeed;
	public float ratioStopJump;
	private bool isGroundJumping = false;
	private bool hasDoubleJump = false;

	public override void Update()
	{
		base.Update();

		// Reset double jump when on the ground
		if (isGrounded)
		{
			hasDoubleJump = false;
		}
	}

	public override bool Jump()
	{
		isGroundJumping = true;

		var jumped = base.Jump();

		if (!jumped && !hasDoubleJump && !verticalMoveEnabled)
		{
			body.velocity = new Vector2(body.velocity.x, doubleJumpSpeed);
			hasDoubleJump = true;
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

}
