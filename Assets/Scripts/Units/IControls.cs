using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControls
{
	bool Move(Vector2 direction, float ratio = 1);
	bool Jump();
	bool StopJump();
}
