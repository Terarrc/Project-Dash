using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHasJumpedEvent : IEvent
{
	// The unit who jumped
	private Entity unit;
	
	public UnitHasJumpedEvent(Entity unit)
	{
		this.unit = unit;
	}

	public void execute()
	{
		// Do things
	}
}
