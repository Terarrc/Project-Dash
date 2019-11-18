using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pine_Cone : Unit
{
	private SpriteRenderer thread;

	// Start is called before the first frame update
	new void Awake()
    {
		base.Awake();
		Transform child = transform.GetChild(0);
		thread = child.GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	new void Update()
	{
		base.Update();
		if (thread != null)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 100, 1 << LayerMask.NameToLayer("Ground"));

			if (float.IsNaN(hit.distance))
				hit.distance = 0;

			thread.size = new Vector2(thread.size.x, hit.distance);
			thread.transform.localPosition = new Vector3(thread.transform.localPosition.x, hit.distance / 2, thread.transform.localPosition.z);
		}

		Collider2D[] colliders = Physics2D.OverlapCircleAll(Position, 0.5f, (1 << LayerMask.NameToLayer("Robot")) + (1 << LayerMask.NameToLayer("Player")));

		foreach (Collider2D collider in colliders)
		{
			Unit unit = collider.gameObject.GetComponent<Unit>();
			if (unit != null && unit.faction != this.faction)
				unit.ApplyDamage(5, Health.DamageType.Physic, gameObject);
		}
	}
}
