using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
	public float damage;

	private void OnCollisionStay2D(Collision2D collision)
	{
		Health health = collision.gameObject.GetComponent<Health>();
		if (health != null)
			health.Kill(gameObject);
	}
}
