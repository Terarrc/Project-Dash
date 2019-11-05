using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
	public Vector3 respawnPoint;

	protected override void Kill(GameObject source)
	{
		gameObject.transform.position = respawnPoint;
	}
}
