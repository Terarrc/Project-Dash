using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KinematicBodyController))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour
{
	public enum Faction { Robot, Demon };

	protected KinematicBodyController body;
	protected BoxCollider2D boxCollider;
	protected SpriteRenderer sprite;
	protected Animator animator;
	protected Health health;

	[SerializeField, Tooltip("Movement speed of the unit")]
	public Vector2 speed;
	[SerializeField, Tooltip("Acceleration of the unit")]
	public float acceleration;
	[SerializeField, Tooltip("Enable vertical movement input")]
	public bool verticalMovement;
	[SerializeField, Tooltip("Vertical speed given when the unit is jumping")]
	public float jumpHeight;
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
			return transform.position;
		}
		set
		{
			transform.position = new Vector3(value.x, value.y, transform.position.z);
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
				body.SetVelocityY(Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y)));
				IsGrounded = false;

				if (animator != null)
					animator.SetTrigger("Jumped");
			}
			else if (body.Velocity.y > 0)
			{
				body.SetVelocityY(body.Velocity.y / 2);
			}

		}
	}

	public int LayerGround { get; set; }
	protected bool isGrounded;
	public virtual bool IsGrounded
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
		body = GetComponent<KinematicBodyController>();
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
		IsGrounded = body.Grounded;

		if (body.Velocity.y <= 0)
			IsJumping = false;

		if (animator != null)
		{
			animator.SetFloat("Speed X", body.Velocity.x);
			animator.SetFloat("Speed Y", body.Velocity.y);
		}
	}

	public void AddVelocity(Vector2 velocity)
	{
		body.SetVelocityX(body.Velocity.x + velocity.x);
		body.SetVelocityY(body.Velocity.y + velocity.y);
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
			body.MoveX(input.x, speed.x, acceleration);
		else
			body.Move(input, speed, acceleration);

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