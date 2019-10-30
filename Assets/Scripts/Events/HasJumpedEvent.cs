using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasJumpedEvent : IEvent
{
	// The unit who jumped
	private Unit unit;
	
	public HasJumpedEvent(Unit unit)
	{
		this.unit = unit;
	}

	public void execute()
	{
		// Do things
	}
}
