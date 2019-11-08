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

		LayerGround = (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("Energy Ground")) + (1 << LayerMask.NameToLayer("Energy Field")) + (1 << gameObject.layer);
	}

	protected void Update()
	{
		if (body.velocity.y <= 0)
			IsJumping = false;

		IsGrounded = Physics2D.OverlapCircle(body.position + (Vector2.down * (boxCollider.bounds.size.y / 2)), boxCollider.bounds.size.x / 2, LayerGround - (1 << gameObject.layer));

		animator.SetFloat("Speed X", body.velocity.x);
		animator.SetFloat("Speed Y", body.velocity.y);
	}

	public float GetDirectionX()
	{
		return sprite.flipX ? -1 : 1;
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
}