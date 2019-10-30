using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IControls
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	// ========================================================================
	// Controls
	// ========================================================================
	public bool Move(Vector3 direction, float ratio = 1)
	{
		return false;
	}

	public bool Jump()
	{
		return false;
	}

	public bool StopJump()
	{
		return false;
	}

}
