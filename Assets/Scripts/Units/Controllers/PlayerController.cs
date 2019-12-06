using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
	public enum BufferedInput { none, jump, dash };

	[SerializeField, Tooltip("Enable analog movement")]
	public bool analogMovement;

	[SerializeField, Tooltip("Time in seconds an undoable input will be buffered")]
	public float bufferTime;


	// Buffer for player input
	private BufferedInput buffer;
	public BufferedInput Buffer
	{
		get
		{
			return buffer;
		}
		set
		{
			buffer = value;
			if (value != BufferedInput.none)
				timerBuffer = bufferTime;
		}
	}
	private float timerBuffer;



	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		// Decrease timer buffer
		if (timerBuffer > 0)
		{
			timerBuffer -= Time.deltaTime;
			if (timerBuffer <= 0)
			{
				Buffer = BufferedInput.none;
			}
		}

		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		// Move
		if (!analogMovement)
		{
			if (Mathf.Abs(horizontal) < 0.8)
				unit.Move(Vector2.zero);
			else
			{

				if (Mathf.Abs(horizontal) < 0.8)
					horizontal = 0;
				else if (horizontal < 0)
					horizontal = -1;
				else
					horizontal = 1;

				if (Mathf.Abs(vertical) < 0.8)
					vertical = 0;
				else if (vertical < 0)
					vertical = -1;
				else
					vertical = 1;
			}
		}
		unit.Move(new Vector2(horizontal, vertical));

		// Drop
		unit.Drop(vertical <= -0.9f);

		// Jump
		bool jumped = false;
	
		if (Input.GetButtonDown("Jump"))
		{
			if (!unit.Jump())
			{
				Buffer = BufferedInput.jump;
			}
			else
				jumped = true;
		}
		else if (buffer == BufferedInput.jump)
		{
			if (unit.Jump())
			{
				Buffer = BufferedInput.none;
				if (!Input.GetButton("Jump"))
					unit.StopJump();

				jumped = true;
			}
		}

		// Stop Jump
		if (Input.GetButtonUp("Jump"))
		{
			unit.StopJump();
		}

		// Dash
		if (!jumped)
		{
			if (Input.GetButtonDown("Action 1"))
			{
				if (!unit.Action(1))
				{
					Buffer = BufferedInput.dash;
				}
			}
			else if (buffer == BufferedInput.dash)
			{
				if (unit.Action(1))
				{
					Buffer = BufferedInput.none;
				}
			}
		}
	}
}
