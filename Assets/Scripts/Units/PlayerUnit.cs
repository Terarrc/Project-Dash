using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public override bool Move(Vector2 direction)
	{
		// No vertical movement
		direction.y = 0;

		// Check if move or stop
		if (direction == Vector2.zero)
		{
			wantedSpeedX = 0;

			animator.SetBool("Walking", false);

			return true;
		}
		else
		{
			direction.Normalize();

			wantedSpeedX = direction.x * moveSpeedX;

			sprite.flipX = direction.x < 0;
			animator.SetBool("Walking", true);

			return true;
		}
	}

	public override bool Jump()
	{
		if (isGrounded)
		{
			body.velocity = new Vector2(body.velocity.x, jumpSpeed);

			// Send event
			var hasJumped = new HasJumpedEvent(this);
			hasJumped.execute();
		}

		return false;
	}
}
