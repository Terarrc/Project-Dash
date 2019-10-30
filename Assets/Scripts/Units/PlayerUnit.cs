using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public override bool Move(Vector2 direction)
	{
		// No vertical movement
		direction.y = 0;
		direction.Normalize();

		return base.Move(direction);
	}

	public override bool Jump()
	{
		if (isGrounded)
		{
			body.velocity = new Vector2(body.velocity.x, jumpSpeed);

			// Send event
			var hasJumped = new UnitHasJumpedEvent(this);
			hasJumped.execute();

			return true;
		}

		return false;
	}
}
