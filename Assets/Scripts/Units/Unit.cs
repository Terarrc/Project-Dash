using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IControls
{
    // All of the Unit's component
    protected Rigidbody2D body;
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;
	protected Animator animator;

	// Physic variable
	public float moveSpeedX, moveSpeedY;
	public float jumpSpeed;
	public float accelerationX, accelerationY;
	public bool affectedByGravity;
	protected float currentSpeedX, currentSpeedY, wantedSpeedX, wantedSpeedY;
	protected float currentAccelerationX, currentAccelerationY;
	protected bool isGrounded = false;
    protected bool isOnHorizontalEnergyField = false;
    protected bool isOnVerticalEnergyField_Left = false;
    protected bool isOnVerticalEnergyField_Right = false;

	// Disable physic update on a specific axis
	protected bool lockAxisX = false;
	protected bool lockAxisY = false;

	// On wake, get Unit's components
	void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
	{
		// We don't care if a parameter doesn't exists
		animator.logWarnings = false;

		if (affectedByGravity)
			wantedSpeedY = -100;
	}

	// Update is called once per frame
	public virtual void Update()
	{
		float time = Time.deltaTime;

		// Less maneouvrability in the air
		if (affectedByGravity)
		{
			if (isGrounded)
				currentAccelerationX = accelerationX;
			else if (currentSpeedY > 0)
				currentAccelerationX = accelerationX / 2;
			else
				currentAccelerationX = accelerationX / 2;

			currentAccelerationY = WorldSettings.gravity;
		}
		else
		{
			currentAccelerationX = accelerationX;
			currentAccelerationY = accelerationY;
		}

		float positionX;
		float positionY;

		// Get the current position
		positionX = transform.position.x;
		positionY = transform.position.y;

		// Update the speed X
		if (!lockAxisX)
		{
			if (currentSpeedX < wantedSpeedX)
			{
				currentSpeedX = Mathf.Min(wantedSpeedX, currentSpeedX + (currentAccelerationX * time));
			}
			else if (currentSpeedX > wantedSpeedX)
			{
				currentSpeedX = Mathf.Max(wantedSpeedX, currentSpeedX - (currentAccelerationX * time));
			}
		}

		// Update the speed Y
		if (!lockAxisY)
		{ 
			if (currentSpeedY < wantedSpeedY)
			{
				currentSpeedY = Mathf.Min(wantedSpeedY, currentSpeedY + (currentAccelerationY * time));
			}
			else if (currentSpeedY > wantedSpeedY)
			{
				currentSpeedY = Mathf.Max(wantedSpeedY, currentSpeedY - (currentAccelerationY * time));
			}
		}

        #region Calculate position x

        // Calculate the position X
        float offset = 0;
		if (!lockAxisX)
		{
			float deltaPositionX = currentSpeedX * time;
			float oldPosX = positionX;
			positionX += deltaPositionX;

			if (body)
			{
				float height = boxCollider.bounds.size.y;
				float halfWidth = (boxCollider.bounds.size.x / 2);

				// Check if there is something between the old and the new position
				Vector2 pointA, pointB;
				if (currentSpeedX < 0)
				{
					pointA = new Vector2(oldPosX - halfWidth - offset, positionY + (height * 0.8f));
					pointB = new Vector2(positionX - halfWidth - offset, positionY + (height * 0.2f));
				}
				else
				{
					pointA = new Vector2(oldPosX + halfWidth + offset, positionY + (height * 0.8f));
					pointB = new Vector2(positionX + halfWidth + offset, positionY + (height * 0.2f));
				}

				Collider2D[] collidersEnergy = Physics2D.OverlapAreaAll(pointA, pointB, 1 << LayerMask.NameToLayer("Vertical Energy Fields"));

				// Move the body the furthest possible without collision
				foreach (Collider2D collider in collidersEnergy)
				{
					if (currentSpeedX < 0 && collider.bounds.max.x + halfWidth > positionX)
					{
						positionX = collider.bounds.max.x + halfWidth + (2 * offset);
						TouchEnergyFieldVertical(-1);
					}
					if (currentSpeedX > 0 && collider.bounds.min.x - halfWidth - (2 * offset) < positionX)
					{
						positionX = collider.bounds.min.x - halfWidth - (2 * offset);
						TouchEnergyFieldVertical(1);
					}
				}

				Collider2D[] collidersGround = Physics2D.OverlapAreaAll(pointA, pointB, 1 << LayerMask.NameToLayer("Ground"));

				// Move the body the furthest possible without collision
				foreach (Collider2D collider in collidersGround)
				{
					if (currentSpeedX < 0 && collider.bounds.max.x + halfWidth > positionX)
					{
						positionX = collider.bounds.max.x + halfWidth + (2 * offset);
					}
					if (currentSpeedX > 0 && collider.bounds.min.x - halfWidth - (2 * offset) < positionX)
					{
						positionX = collider.bounds.min.x - halfWidth - (2 * offset);
					}
				}
			}
		}
		animator.SetFloat("Speed X", currentSpeedX);

		#endregion

		#region Calculate position y

		// Calculate the position Y
		isGrounded = false;
		if (!lockAxisY)
		{
			float deltaPositionY = currentSpeedY * time;
			float oldPosY = positionY;
			positionY += deltaPositionY;

			if (body)
			{
				float height = boxCollider.bounds.size.y;
				float halfWidth = (boxCollider.bounds.size.x / 2);

				// Check if there is something between the old and the new position
				Vector2 pointA, pointB;
				if (currentSpeedY < 0)
				{
					pointA = new Vector2(positionX + (halfWidth * 0.6f), oldPosY - offset);
					pointB = new Vector2(positionX - (halfWidth * 0.6f), positionY - offset);
				}
				else
				{
					pointA = new Vector2(positionX + (halfWidth * 0.6f), oldPosY + height + offset);
					pointB = new Vector2(positionX - (halfWidth * 0.6f), positionY + height + offset);
				}

				Collider2D[] collidersEnergy = Physics2D.OverlapAreaAll(pointA, pointB, 1 << LayerMask.NameToLayer("Horizontal Energy Fields"));

				// Move the body the furthest possible without collision
				foreach (Collider2D collider in collidersEnergy)
				{
					if (currentSpeedY < 0 && collider.bounds.max.y + (2 * offset) > positionY)
					{
						positionY = collider.bounds.max.y + (2 * offset);
						TouchEnergyFieldHorizontal(-1);
					}
					if (currentSpeedY > 0 && collider.bounds.min.y - height - (2 * offset) < positionY)
					{
						positionY = collider.bounds.min.y - height - (2 * offset);
						TouchEnergyFieldHorizontal(1);
					}
				}

				Collider2D[] collidersGround = Physics2D.OverlapAreaAll(pointA, pointB, 1 << LayerMask.NameToLayer("Ground"));

				// Move the body the furthest possible without collision
				foreach (Collider2D collider in collidersGround)
				{
					if (currentSpeedY < 0 && collider.bounds.max.y + (2 * offset) > positionY)
					{
						positionY = collider.bounds.max.y + (2 * offset);
						// A vertical collision while falling mean landing
						isGrounded = true;
						currentSpeedY = 0;
					}
					if (currentSpeedY > 0 && collider.bounds.min.y - height - (2 * offset) < positionY)
					{
						positionY = collider.bounds.min.y - height - (2 * offset);
						currentSpeedY = 0;
					}
				}
				animator.SetBool("Grounded", isGrounded);
			}
		}
		animator.SetFloat("Speed Y", currentSpeedY);

        #endregion

        // Update the position
        transform.position = new Vector3(positionX, positionY);
    }

	protected virtual void TouchEnergyFieldVertical(float direction)
	{

	}
	protected virtual void TouchEnergyFieldHorizontal(float direction)
	{

	}

	// ========================================================================
	// Controls
	// ========================================================================
	public virtual bool Move(Vector2 scale)
	{
		if (affectedByGravity)
			scale.y = 0;

		if (scale.x != 0)
			sprite.flipX = scale.x < 0;

		wantedSpeedX = scale.x * moveSpeedX;
		if (!affectedByGravity)
			wantedSpeedY = scale.y * moveSpeedY;

		animator.SetBool("Moving X", scale.x != 0);
		animator.SetBool("Moving Y", scale.y != 0);

		return true;
	}

	public bool Turn(float direction)
	{
		sprite.flipX = (direction < 0);
		return true;
	}

	public virtual bool Jump()
	{
		if (isGrounded && affectedByGravity)
		{
			isGrounded = false;
			currentSpeedY = jumpSpeed;

			// Send event
			var hasJumped = new UnitHasJumpedEvent(this);
			hasJumped.execute();

			animator.SetTrigger("Jumped");

			return true;
		}

		return false;
	}
	
	public virtual bool StopJump()
	{
		return true;
	}

	public virtual bool Action(int index)
	{
		return true;
	}

	public float GetDirectionX()
	{
		if (sprite.flipX)
			return -1;
		else 
			return 1;
	}


}
