using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControls
{
	bool Move(Vector2 scale);
	bool Jump();
	bool StopJump();
	bool Dash(float scale);

	float GetDirection();
}
