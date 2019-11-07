using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
	public enum bufferedInput { none, jump, dash };

	[SerializeField, Tooltip("Enable analog movement")]
	public bool analogMovement;

	[SerializeField, Tooltip("Time in seconds an undoable input will be buffered")]
	public float bufferTime;


	// Buffer for player input
	private bufferedInput buffer;
	public bufferedInput Buffer
	{
		get
		{
			return buffer;
		}
		set
		{
			buffer = value;
			if (value != bufferedInput.none)
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
			float time = Time.deltaTime * 1000f;
			timerBuffer -= time;
			if (timerBuffer <= 0)
			{
				Buffer = bufferedInput.none;
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

		// Jump
		bool jumped = false;
		if (Input.GetButtonDown("Jump"))
		{
			if (!unit.Jump())
			{
				Buffer = bufferedInput.jump;
			}
			else
				jumped = true;
		}
		else if (buffer == bufferedInput.jump)
		{
			if (unit.Jump())
			{
				Buffer = bufferedInput.none;
				if (!Input.GetButton("Jump"))
					unit.StopJump();

				jumped = true;
			}
		}

		// Hold Jump
		if (Input.GetButton("Jump"))
		{
			unit.HoldJump();
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
					Buffer = bufferedInput.dash;
				}
			}
			else if (buffer == bufferedInput.dash)
			{
				if (unit.Action(1))
				{
					Buffer = bufferedInput.none;
				}
			}
		}
	}
}
