﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour
{
	protected Rigidbody2D body;
	protected BoxCollider2D boxCollider;
	protected SpriteRenderer sprite;
	protected Animator animator;

	public Vector2 speed;
	public Vector2 acceleration;
	public float jumpSpeed;

	private int layerGround;

	protected bool isJumping;
	protected bool IsJumping 
	{ 
		get
		{
			return isJumping;
		}
		set
		{
			isJumping = value;

			if (value)
			{
				body.velocity = new Vector2(body.velocity.x, jumpSpeed);
				IsGrounded = false;

				if (animator != null)
					animator.SetTrigger("Jumped");
			}
			else if (body.velocity.y > 0)
			{
				body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);
			}

		}
	}

	protected bool isGrounded;
	protected bool IsGrounded
	{
		get
		{
			return isGrounded;
		}
		set
		{
			isGrounded = value;
			if (animator != null)
				animator.SetBool("Grounded", value);
		}
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();

		boxCollider = GetComponent<BoxCollider2D>();
		sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator != null)
			animator.logWarnings = false;

		layerGround = (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("Energy Ground")) + (1 << LayerMask.NameToLayer("Energy Field"));
	}

	protected void Update()
	{
		if (body.velocity.y <= 0)
			IsJumping = false;

		IsGrounded = Mathf.Approximately(body.velocity.y, 0) && Physics2D.OverlapCircle(body.position, boxCollider.bounds.size.x / 2, layerGround);

		animator.SetFloat("Speed X", body.velocity.x);
		animator.SetFloat("Speed Y", body.velocity.y);
	}

	public float GetDirectionX()
	{
		return sprite.flipX ? -1 : 1;
	}

	public virtual bool Move(Vector2 input)
	{
		body.velocity = new Vector2(Mathf.MoveTowards(body.velocity.x, speed.x * input.x, acceleration.x * Time.deltaTime), body.velocity.y);

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

		if (IsGrounded)
		{
			IsJumping = true;

			return true;
		}

		return false;
	}

	public virtual bool StopJump()
	{
		if(IsJumping)
		{
			IsJumping = false;		
		}

		return true;
	}

	public virtual bool Action(int index)
	{
		return false;
	}
}