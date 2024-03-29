﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
	public enum bufferedInput { none, jump, dash };

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
	// Time in milliseconds the buffer will keep it's input
	public float bufferTime;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// Make sure we have controls to uses
		if (controls == null)
		{
			return;
		}

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

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// Move
		if (Mathf.Abs(horizontal) < 0.8 && Mathf.Abs(vertical) < 0.8)
			controls.Move(Vector2.zero);
		else
			controls.Move(new Vector2(horizontal, vertical));

		// Jump
		if (Input.GetButtonDown("Jump"))
		{
			if (!controls.Jump())
			{
				Buffer = bufferedInput.jump;
			}
		} 
		else if (buffer == bufferedInput.jump)
		{
			if (controls.Jump())
			{
				Buffer = bufferedInput.none;
                if (!Input.GetButton("Jump"))
                    controls.StopJump();
            }
		}

        if(Input.GetButtonUp("Jump"))
        {
            controls.StopJump();
        }

	}
}
