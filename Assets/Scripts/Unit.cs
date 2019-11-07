using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(ControllableRigidBody))]
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour
{
	protected ControllableRigidBody body;
	protected BoxCollider2D boxCollider;
	protected SpriteRenderer sprite;
	protected Animator animator;

	public Vector2 speed;
	public Vector2 acceleration;
	public float jumpSpeed;

	private int layerGround;
	protected bool isJumping;

	protected void Awake()
	{
		body = GetComponent<ControllableRigidBody>();

		layerGround += 1 << LayerMask.NameToLayer("Ground");
		layerGround += 1 << LayerMask.NameToLayer("Vertical Energy Ground");
		layerGround += 1 << LayerMask.NameToLayer("Horizontal Energy Ground");
		body.LayerGround = layerGround;

		boxCollider = GetComponent<BoxCollider2D>();
		sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator != null)
			animator.logWarnings = false;
	}

	protected void Update()
	{
		if (body.Velocity.y <= 0)
		{
			isJumping = false;
		}

		animator.SetFloat("Speed X", body.Velocity.x);
		animator.SetFloat("Speed Y", body.Velocity.y);
		animator.SetBool("Grounded", body.Grounded);
	}

	public virtual bool Move(Vector2 input)
	{
		body.MoveX(input.x, speed.x, acceleration.x);
		//body.Velocity = new Vector2(Mathf.MoveTowards(body.Velocity.x, speed.x * input.x, acceleration.x * Time.deltaTime), body.velocity.y);

		if (input.x != 0)
			sprite.flipX = input.x > 0 ? false : true;

		if (animator != null)
		{
			animator.SetBool("Moving X", input.x != 0);
			animator.SetBool("Moving Y", input.y != 0);
		}

		return true;
	}

	public virtual bool Jump()
	{

		if (body.Grounded)//body.Grounded)
		{
			body.SetVelocityY(jumpSpeed);
			//body.velocity = new Vector2(body.velocity.x, Mathf.Sqrt(2 * minJumpHeight * Mathf.Abs(Physics2D.gravity.y)));
			isJumping = true;

			if (animator != null)
				animator.SetTrigger("Jumped");

			return true;
		}

		return false;
	}

	public virtual bool HoldJump()
	{
		/*
		if (isJumping)
		{
			if (timerJump > 0)
			{
				body.SetVelocityY(Mathf.Sqrt(2 * minJumpHeight * Mathf.Abs(Physics2D.gravity.y)));
				timerJump -= Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}
		*/
		return true;
	}

	public virtual bool StopJump()
	{
		if(isJumping)
		{
			isJumping = false;
			body.SetVelocityY(body.Velocity.y / 2);
		}

		return true;
	}

	public virtual bool Action(int index)
	{
		return false;
	}
}