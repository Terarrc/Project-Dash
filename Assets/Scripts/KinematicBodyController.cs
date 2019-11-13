﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class KinematicBodyController : MonoBehaviour
{
	protected BoxCollider2D boxCollider;
	protected Rigidbody2D body;

	public float gravityScale;

	public Vector2 Velocity { get; set; }
	public Vector2 Position
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = new Vector3(value.x, value.y, transform.position.z);
		}
	}
	public Vector2 Size
	{
		get
		{
			return boxCollider.bounds.size;
		}
	}

	public bool Grounded { get; private set; }
	public RigidbodyConstraints2D Constraints
	{
		get
		{
			return body.constraints;
		}
		set
		{
			body.constraints = Constraints;
		}
	}

	// Start is called before the first frame update
	private void Awake()
    {
		boxCollider = GetComponent<BoxCollider2D>();
		body = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		Velocity += Physics2D.gravity * gravityScale * Time.fixedDeltaTime;

		int layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

		if ((Constraints & RigidbodyConstraints2D.FreezePositionX) == 0)
		{
			transform.position += new Vector3(Velocity.x * Time.fixedDeltaTime, 0);

			Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0, layerMask);
			foreach (Collider2D hit in hits)
			{
				if (hit == boxCollider || hit.isTrigger)
					continue;

				ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
				if (colliderDistance.isOverlapped)
				{
					Vector2 translate = colliderDistance.pointA - colliderDistance.pointB;
					translate.y = 0;
					transform.Translate(translate);
				}
			}
		}

		Grounded = false;
		if ((Constraints & RigidbodyConstraints2D.FreezePositionY) == 0)
		{
			transform.position += new Vector3(0, Velocity.y * Time.fixedDeltaTime);

			Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0, layerMask);
			foreach (Collider2D hit in hits)
			{
				if (hit == boxCollider || hit.isTrigger)
					continue;

				ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
				if (colliderDistance.isOverlapped)
				{
					Vector2 translate = colliderDistance.pointA - colliderDistance.pointB;
					translate.x = 0;
					transform.Translate(translate);

					if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 1)
					{
						Grounded = true;
						Velocity = new Vector2(Velocity.x, 0);
					}
				}
			}
		}
	}

	public void Move(Vector2 input, Vector2 speed, float acceleration)
	{
		Velocity = Vector2.MoveTowards(Velocity, speed * input, acceleration * Time.deltaTime);
	}

	public void MoveX(float input, float speed, float acceleration)
	{
		float newVelocityX = Mathf.MoveTowards(Velocity.x, speed * input, acceleration * Time.deltaTime);

		if (Mathf.Abs(newVelocityX) < 0.1)
			newVelocityX = 0;

		Velocity = new Vector2(newVelocityX, Velocity.y);
	}

	public void MoveY(float input, float speed, float acceleration)
	{
		float newVelocityY = Mathf.MoveTowards(Velocity.y, speed * input, acceleration * Time.deltaTime);

		Velocity = new Vector2(Velocity.x, newVelocityY);
	}

	public void SetVelocity(Vector2 velocity)
	{
		Velocity = velocity;
	}

	public void SetVelocityX(float velocity)
	{
		Velocity = new Vector2(velocity, Velocity.y);
	}

	public void SetVelocityY(float velocity)
	{
		Velocity = new Vector2(Velocity.x, velocity);
	}
}
