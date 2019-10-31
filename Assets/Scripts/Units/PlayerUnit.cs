using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public float ratioStopJump;
	private bool isGroundJumping = false;

	public override bool Jump()
	{
		isGroundJumping = true;

		return base.Jump();
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
