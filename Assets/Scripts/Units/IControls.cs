using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControls
{
	bool Move(Vector2 direction);
	bool Jump();
	bool StopJump();
}
