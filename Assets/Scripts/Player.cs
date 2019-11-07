using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	public GameObject doubleJumpParticle;
	public float doubleJumpSpeed;

	private bool canDoubleJump = true;

    new void Update()
    {
		base.Update();

		if (body.Grounded)
			canDoubleJump = true;

	}

	public override bool Jump()
	{
		if (base.Jump())
			return true;

		else if (canDoubleJump)
		{
			canDoubleJump = false;
			body.SetVelocityY(doubleJumpSpeed);

			if (animator != null)
				animator.SetTrigger("Jumped");

			// Generate particle
			if (doubleJumpParticle != null)
				Instantiate(doubleJumpParticle, transform.position, Quaternion.identity);

			return true;
		}

		return false;
	}

}
