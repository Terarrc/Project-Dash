using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IControls
{
    // All of the Unit's component
    protected Rigidbody2D body;
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;

	// Physic variable
	public float moveSpeedX = 6, moveSpeedY = 6;
	public float jumpSpeed = 9;
	public float accelerationX = 100, accelerationY = 100;
	protected float currentSpeedX, currentSpeedY, wantedSpeedX, wantedSpeedY;
	protected bool isGrounded = true;

	// On wake, get Unit's components
	void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		float positionX = transform.position.x;
		float positionY = transform.position.y;

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
		if (!Mathf.Approximately(currentSpeedX, 0))
		{
			positionX += currentSpeedX * Time.deltaTime;
		}
		// Update the position Y
		if (!Mathf.Approximately(currentSpeedY, 0))
		{
			positionY += currentSpeedY * Time.deltaTime;
		}

		transform.position = new Vector3(positionX, positionY);
	}

	// ========================================================================
	// Controls
	// ========================================================================
	public virtual bool Move(Vector2 direction)
	{
		return false;
	}

	public virtual bool Jump()
	{
		return false;
	}

	public virtual bool StopJump()
	{
		return false;
	}

}
