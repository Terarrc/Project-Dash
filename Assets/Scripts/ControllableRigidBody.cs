using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ControllableRigidBody : MonoBehaviour
{
	public Vector2 Velocity { 
		get
		{
			return body.velocity;
		}
		set
		{
			body.velocity = value;
		}
	}

	private BoxCollider2D boxCollider;
	protected Rigidbody2D body;

	public bool Grounded { get; private set; }
	public int LayerGround { get; set; }

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		body = GetComponent<Rigidbody2D>();
	}


    void FixedUpdate()
    {
		Debug.Log(Velocity);
		if (Physics2D.OverlapCircle(body.position, boxCollider.bounds.size.x / 2, LayerGround))
			Grounded = true;
		else
			Grounded = false;
	}

	public void MoveX(float moveInput, float speed, float acceleration)
	{
		Velocity = new Vector2(Mathf.MoveTowards(Velocity.x, speed * moveInput, acceleration * Time.deltaTime), Velocity.y);
	}

	public void MoveY(float moveInput, float speed, float acceleration)
	{
		Velocity = new Vector2(Velocity.x, Mathf.MoveTowards(Velocity.y, speed * moveInput, acceleration * Time.deltaTime));
	}

	public void SetVelocityY(float value)
	{
		Velocity = new Vector2(Velocity.x, value);
	}
}
