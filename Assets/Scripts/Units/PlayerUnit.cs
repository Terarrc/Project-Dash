﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
	public TimedDurationEntity DoubleJumpParticle;
    public TimedDurationEntity createdField;

	public float doubleJumpSpeed;
	public float ratioStopJump;
	public float dashSpeed;
	public float dashDuration;
	private bool isGroundJumping = false;

	private bool canDoubleJump = false;
	private bool canDash = false;
	private bool canCreatePlatform = false;
	private bool canCreateWall = false;
    
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

		base.Update();

		// Reset bonus movement options when on the ground
		if (isGrounded)
		{
			canDoubleJump = true;
			canDash = true;
            canCreatePlatform = true;
            canCreateWall = true;
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
			dashScale = GetDirection();
			canDash = false;

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
