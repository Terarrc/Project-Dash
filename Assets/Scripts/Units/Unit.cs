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
	public bool verticalMoveEnabled;
	protected float currentSpeedX, currentSpeedY, wantedSpeedX, wantedSpeedY;
	protected float currentAccelerationX, currentAccelerationY;
	protected bool isGrounded = false;
    protected bool isOnHorizontalEnergyField = false;
    protected bool isOnVerticalEnergyField_Left = false;
    protected bool isOnVerticalEnergyField_Right = false;

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
		if (!verticalMoveEnabled)
			wantedSpeedY = -10;
	}

	// Update is called once per frame
	public virtual void Update()
	{
		// Less maneouvrability in the air
		if (isGrounded || verticalMoveEnabled)
		{
			currentAccelerationX = accelerationX;
			currentAccelerationY = accelerationY;
		}
		else
		{
			currentAccelerationX = accelerationX / 2;
			currentAccelerationY = accelerationY / 2;
		}

		float positionX;
		float positionY;

		// Use the rigid body if exists
		if (body)
		{
			positionX = body.position.x;
			positionY = body.position.y;
		}
		else
		{
			positionX = transform.position.x;
			positionY = transform.position.y;
		}

		// Update the speed X
		if (currentSpeedX < wantedSpeedX)
		{
			currentSpeedX = Mathf.Min(wantedSpeedX, currentSpeedX + (accelerationX * Time.deltaTime));
		}
		else if (currentSpeedX > wantedSpeedX)
		{
			currentSpeedX = Mathf.Max(wantedSpeedX, currentSpeedX - (accelerationX * Time.deltaTime));
		}

		// Update the speed Y
		if (currentSpeedY < wantedSpeedY)
		{
			currentSpeedY = Mathf.Min(wantedSpeedY, currentSpeedY + (accelerationY * Time.deltaTime));
		}
		else if (currentSpeedY > wantedSpeedY)
		{
			currentSpeedY = Mathf.Max(wantedSpeedY, currentSpeedY - (accelerationY * Time.deltaTime));
		}

		// Update the position X

		float deltaPositionX = currentSpeedX * Time.deltaTime;
		float oldPosX = positionX;
		positionX += deltaPositionX;

		if (body)
		{
			float height = boxCollider.bounds.size.y;
			float halfWidth = (boxCollider.bounds.size.x / 2);

			Vector2 pointA, pointB;
			if (currentSpeedX < 0)
			{
				pointA = new Vector2(oldPosX - halfWidth - 0.02f, positionY + (height * 0.8f));
				pointB = new Vector2(positionX - halfWidth - 0.02f, positionY + (height * 0.2f));
			}
			else
			{
				pointA = new Vector2(oldPosX + halfWidth + 0.02f, positionY + (height * 0.8f));
				pointB = new Vector2(positionX + halfWidth + 0.02f, positionY + (height * 0.2f));
			}

			Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, 1 << 8);

			foreach (Collider2D collider in colliders)
			{
				if (currentSpeedX < 0 && collider.bounds.max.x + halfWidth + 0.04f > positionX)
				{
					positionX = collider.bounds.max.x + (boxCollider.bounds.size.x / 2) + 0.04f;
				}
				if (currentSpeedX > 0 && collider.bounds.min.x - halfWidth - 0.04f < positionX)
				{
					positionX = collider.bounds.min.x - (boxCollider.bounds.size.x / 2) - 0.04f;
				}
			}
		}

		// Update the position Y
	
		float deltaPositionY = currentSpeedY * Time.deltaTime;
		float oldPosY = positionY;
		positionY += deltaPositionY;

		if (body)
		{
			float height = boxCollider.bounds.size.y;
			float halfWidth = (boxCollider.bounds.size.x / 2);

			Vector2 pointA, pointB;
			if (currentSpeedY < 0)
			{
				pointA = new Vector2(positionX + (halfWidth * 0.6f), oldPosY - 0.01f);
				pointB = new Vector2(positionX - (halfWidth * 0.6f), positionY - 0.01f);
			}
			else
			{
				pointA = new Vector2(positionX + (halfWidth * 0.6f), oldPosY + height + 0.01f);
				pointB = new Vector2(positionX - (halfWidth * 0.6f), positionY + height + 0.01f);
			}

			Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB, 1 << 8);

			isGrounded = false;
			foreach (Collider2D collider in colliders)
			{
				if (currentSpeedY < 0 && collider.bounds.max.y + 0.02f > positionY)
				{
					positionY = collider.bounds.max.y + 0.02f;
					isGrounded = true;
					currentSpeedY = 0;
				}
				if (currentSpeedY > 0 && collider.bounds.min.y - height - 0.02f < positionY)
				{
					positionY = collider.bounds.min.y - height - 0.02f;
					currentSpeedY = 0;
				}
			}
			animator.SetBool("Grounded", isGrounded);
		}

		animator.SetFloat("SpeedY", currentSpeedY);

		// Use the rigid body if exists
		if (body)
		{
			body.position = new Vector3(positionX, positionY);
		}
		else
		{
			transform.position = new Vector3(positionX, positionY);
		}

		// Overlap ground check
        // Energy Field ground check
		// Make a rectangle out of two edges (Vector 2) and if that rectangle overlaps with layer (8) it returns true
		if (boxCollider)
		{
            var horizontalHitboxHalf = boxCollider.bounds.size.x / 2; 
            var verticalHitboxQuarter = boxCollider.bounds.size.y / 4;

			/*
            // Ground Check
            isGrounded = Physics2D.OverlapArea(
				new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y),
				new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y), 1 << 8);

			animator.SetBool("Grounded", isGrounded);
			*/
            // Horizontal Energy Field Ground Check
            isOnHorizontalEnergyField = Physics2D.OverlapArea(
                new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y),
                new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y), 1 << 9);

            Debug.Log("Horizontal  : " + isOnHorizontalEnergyField);

            if (!isGrounded)
            {
                // Vertical Energy Field Check Right Side
                isOnVerticalEnergyField_Right = Physics2D.OverlapArea(
                   new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
                   new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3), 1 << 10);

                // Vertical Energy Field Check Left Side
                isOnVerticalEnergyField_Left = Physics2D.OverlapArea(
                   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
                   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3), 1 << 10);               

                Debug.Log("Vertical Right : " + isOnVerticalEnergyField_Right);
                Debug.Log("Vertical Left : " + isOnVerticalEnergyField_Left);
            }          

        }
    }

    /* 
     * Display wall checkers as black lines
     * 
    private void OnDrawGizmos()
    {
        if (boxCollider)
        {
            var horizontalHitboxHalf = boxCollider.bounds.size.x / 2;
            var verticalHitboxQuarter = boxCollider.bounds.size.y / 4;

            Gizmos.color = Color.black;
            Gizmos.DrawLine(
                new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
                new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3));
            Gizmos.DrawLine(
               new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
               new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3));
        }
       
    }
    */

    // ========================================================================
    // Controls
    // ========================================================================
    public virtual bool Move(Vector2 scale)
	{
		if (!verticalMoveEnabled)
		{
			scale.y = 0;
		}

		// Check if move or stop
		if (scale == Vector2.zero)
		{
			wantedSpeedX = 0;
			if (verticalMoveEnabled)
				wantedSpeedY = 0;

			animator.SetBool("Moving", false);

			return true;
		}
		else
		{
			wantedSpeedX = scale.x * moveSpeedX;
			if (verticalMoveEnabled)
				wantedSpeedY = scale.y * moveSpeedY;

			sprite.flipX = scale.x < 0;
			animator.SetBool("Moving", true);

			return true;
		}
	}

	public virtual bool Jump()
	{
		if (isGrounded && !verticalMoveEnabled)
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

	public float GetDirection()
	{
		if (sprite.flipX)
			return -1;
		else 
			return 1;
	}
}
