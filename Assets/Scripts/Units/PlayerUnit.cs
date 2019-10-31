using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity DoubleJumpParticle;
    public TimedDurationEntity CreatedPlatform;

	public float doubleJumpSpeed;
	public float ratioStopJump;
	private bool isGroundJumping = false;
	private bool canDoubleJump = true;
    private bool canCreatePlatform = true;
    private bool canCreateWall = true;

	public override void Update()
	{
		base.Update();

		// Reset double jump when on the ground
		if (isGrounded)
		{
			canDoubleJump = true;
		}
	}

	public override bool Jump()
	{
		isGroundJumping = true;

		var jumped = base.Jump();

		if (!jumped && canDoubleJump && !verticalMoveEnabled)
		{
			body.velocity = new Vector2(body.velocity.x, doubleJumpSpeed);
			canDoubleJump = false;
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

    public bool CreatePlatform()
    {
        if (!isGrounded && canCreatePlatform)
        {
            Instantiate(CreatedPlatform, transform.position, Quaternion.identity);
            return true;
        }

        return false;
    }

}
