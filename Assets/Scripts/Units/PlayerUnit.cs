using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    // Start is called before the first frame update
    public void Start()
    {
        
    }

	// Update is called once per frame
	public void Update()
    {
		
    }

	public new bool Move(Vector2 direction, float ratio)
	{
		// No analog control
		ratio = 1f;

		// No vertical movement
		direction.x = 0;

		// Check if move or stop
		if (direction == Vector2.zero)
		{
			// stop

			return true;
		}
		else
		{
			direction.Normalize();

			// gameobjectspeed = movespeed * direction * ratio

			return true;
		}
	}

	public new bool Jump()
	{
		if (true)
		{
			// Send event
			var hasJumped = new HasJumpedEvent(this);
			hasJumped.execute();
		}

		return false;
	}
}
