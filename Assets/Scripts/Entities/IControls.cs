using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControls
{
	bool Move(Vector2 scale);
	bool Turn(float direction);
	bool Jump();
	bool StopJump();
	bool Action(int index);

	float GetDirectionX();
	int GetLayerCollision();
	int GetLayerBlock();

	float GetDamageDirection();
}
