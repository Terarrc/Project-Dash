using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	protected IControls controls;

	public void Awake()
	{
		controls = gameObject.GetComponent<IControls>();
	}
}
