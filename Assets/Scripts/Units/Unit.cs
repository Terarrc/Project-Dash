using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour
{
	public enum Faction { Robot, Demon };

	protected Rigidbody2D body;
	protected BoxCollider2D boxCollider;
	protected SpriteRenderer sprite;
	protected Animator animator;
	protected Health health;

	[SerializeField, Tooltip("Movement speed of the unit")]
	public Vector2 speed;
	[SerializeField, Tooltip("Acceleration of the unit")]
	public Vector2 acceleration;
	[SerializeField, Tooltip("Enable vertical movement input")]
	public bool verticalMovement;
	[SerializeField, Tooltip("Vertical speed given when the unit is jumping")]
	public float jumpSpeed;
	[SerializeField, Tooltip("Define who will be it's allies and who will be it's enemies")]
	public Faction faction;

	public Vector2 Size
	{
		get
		{
			return boxCollider.bounds.size;
		}
	}

	public Vector2 Position
	{
		get
		{
			return body.position;
		}
	}

	public float AttackRange { get; set; }



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

	public int LayerGround { get; set; }
	protected bool isGrounded;
	public bool IsGrounded
	{
		get
		{
			return isGrounded;
		}
		protected set
		{
			isGrounded = value;
			if (animator != null)
				animator.SetBool("Grounded", value);
		}
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();
		boxCollider = GetComponent<BoxCollider2D>();
		sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator != null)
			animator.logWarnings = false;

		LayerGround = (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("Energy Ground")) + (1 << LayerMask.NameToLayer("Energy Field")) + (1 << LayerMask.NameToLayer("One Way Platform"));
	}

	protected void Update()
	{
		if (body.velocity.y <= 0)
			IsJumping = false;

		if (animator != null)
		{
			animator.SetFloat("Speed X", body.velocity.x);
			animator.SetFloat("Speed Y", body.velocity.y);
		}
	}

	protected void FixedUpdate()
	{
		// Called before unity physic check, including collision
		IsGrounded = false;
	}

	protected void OnCollisionStay2D(Collision2D collision)
	{
		ColliderDistance2D colliderDistance = collision.collider.Distance(boxCollider);

		// Check if the collision is less than 50° with the vertical, and check if the collision is done from the bottom
		if ((Vector2.Angle(colliderDistance.normal, Vector2.up) < 50) && (collision.GetContact(0).point.y - body.position.y < 0))
			IsGrounded = true;
		else
		{
			// If we are stuck to a wall, the angle will be 90°
			// If the angle is not right, check the ground to be sure we are not stuck in a wall
			if (Physics2D.OverlapCircle(Position + (Vector2.down * (Size.y / 2)), (Size.x / 4), LayerGround))
				IsGrounded = true;
		}
	}

	public Vector2 GetDirection()
	{
		return sprite.flipX ? Vector2.left : Vector2.right;
	}

	public virtual bool SetDirectionX(float input)
	{
		sprite.flipX = input < 0;

		return true;
	}

	public void ApplyForce(Vector2 force)
	{
		body.AddForce(force, ForceMode2D.Impulse);
	}

	public float ApplyDamage(float amount, Health.DamageType damageType, GameObject source)
	{
		Health health = GetComponent<Health>();
		if (health != null)
			return health.ApplyDamage(amount, damageType, source);

		return 0;
	}

	public virtual bool Move(Vector2 input)
	{
		if (!verticalMovement)
			body.velocity = new Vector2(Mathf.MoveTowards(body.velocity.x, speed.x * input.x, acceleration.x * Time.deltaTime), body.velocity.y);
		else
			body.velocity = new Vector2(Mathf.MoveTowards(body.velocity.x, speed.x * input.x, acceleration.x * Time.deltaTime), Mathf.MoveTowards(body.velocity.y, speed.y * input.y, acceleration.y * Time.deltaTime));

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

	public virtual bool Drop(bool value)
	{
		return false;
	}
}