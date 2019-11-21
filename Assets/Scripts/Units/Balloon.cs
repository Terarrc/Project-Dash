using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    new void Update()
    {
		base.Update();

		Vector2 pointA = Position + (Vector2.up * ((Size.y / 2) + 0.1f)) + (Vector2.left * (Size.x / 2));
		Vector2 pointB = Position + (Vector2.up * ((Size.y / 2) - 0.5f)) + (Vector2.right * (Size.x / 2));
		int layerMask = (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Demon")) + (1 << LayerMask.NameToLayer("Robot"));

		Collider2D[] hits = Physics2D.OverlapAreaAll(pointA, pointB, layerMask);
		foreach (Collider2D hit in hits)
		{
			if (hit.attachedRigidbody.velocity.y < 0)
			{
				Unit unit = hit.GetComponent<Unit>();
				if (unit != null && unit.Position.y - (unit.Size.y / 2) > Position.y + (Size.y / 4))
				{
					unit.body.velocity = new Vector2(unit.body.velocity.x, 20);
				}
			}
		}
	}
}
