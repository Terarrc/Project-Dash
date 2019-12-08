using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Controller : MonoBehaviour
{
	protected Unit unit;

	public void Awake()
	{
		unit = gameObject.GetComponent<Unit>();
	}

	virtual public void Reset()
	{

	}
}
