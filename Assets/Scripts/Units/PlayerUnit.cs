using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity DoubleJumpParticle;
    public TimedDurationEntity CreatedPlatform;

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



	public override void Update()
	{
		// Decrease dash timer
		if (timerDash > 0)
		{
			float time = Time.deltaTime * 1000f;
			timerDash -= time;
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
		}

		// Decrease dash buffer
		if (timerGroundedDash > 0)
		{
			float time = Time.deltaTime * 1000f;
			timerGroundedDash -= time;
		}

		// Update movements
		float oldPosX = body.position.x;
		base.Update();

		// For dashes, check if we don't pass throught walls
		if (true)//timerDash > 0)
		{ /*
			Vector2 pointA, pointB;
			pointA = new Vector2(oldPosX, body.position.y + (boxCollider.bounds.size.y * 0.6f));
			pointB = new Vector2(body.position.x, body.position.y + (boxCollider.bounds.size.y * 0.4f));


			Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, 1 << 8);

			float positionX = body.position.x;

			foreach (Collider2D collider in colliders)
			{
				if (currentSpeedX < 0 && collider.bounds.max.x > positionX)
				{
					positionX = collider.bounds.max.x + (boxCollider.bounds.size.x / 2) + 0.02f;
				}
				if (currentSpeedX > 0 && collider.bounds.min.x < positionX)
				{
					positionX = collider.bounds.min.x - (boxCollider.bounds.size.x / 2) - 0.02f;
				}
			}

			transform.position = new Vector2(positionX, body.position.y);*/
		}

		// Reset double jump and dash
		if (isGrounded)
		{
			if (timerGroundedDash <= 0)
				canDash = true;
			canDoubleJump = true;
		}
		else
			timerGroundedDash = 0;
	}

	public override bool Jump()
	{
		isGroundJumping = true;

		var jumped = base.Jump();

		if (!jumped && canDoubleJump && !verticalMoveEnabled && timerDash <= 0)
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

	public override bool Action(int index)
	{
		switch (index)
		{
			case 1:
				return Dash();
			case 2:
				return CreatePlatform();
		}
		return false;
	}

	private bool Dash()
	{
		if (canDash)
		{
			preDashSpeed = currentSpeedX;
			timerDash = dashDuration;
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

	private bool CreatePlatform()
    {
        if (!isGrounded && canCreatePlatform)
        {
            Instantiate(CreatedPlatform, transform.position, Quaternion.identity);
            return true;
        }

        return false;
    }

}
