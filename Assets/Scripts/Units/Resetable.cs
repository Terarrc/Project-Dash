using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetable : MonoBehaviour
{
	Vector3 initialPosition;
	Health health;
	Unit unit;
	MovingPlatform movingPlatform;
	Controller controller;

	private void Awake()
	{
		health = GetComponent<Health>();
		unit = GetComponent<Unit>();
		movingPlatform = GetComponent<MovingPlatform>();
		controller = GetComponent<Controller>();
	}

	// Start is called before the first frame update
	void Start()
    {
		initialPosition = transform.position;
	}

	virtual public void Reset()
	{
		transform.position = initialPosition;
		if (health != null)
			health.Reset();
		if (unit != null)
			unit.Reset();
		if (movingPlatform != null)
			movingPlatform.Reset();
		if (controller != null)
			controller.Reset();
	}
}
