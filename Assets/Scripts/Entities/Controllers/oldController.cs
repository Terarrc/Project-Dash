using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldController : MonoBehaviour
{
	protected IControls controls;

	public void Awake()
	{
		controls = gameObject.GetComponent<IControls>();
	}
}
