using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerController : Controller
{
	private BoxCollider2D boxCollider;

	public bool fallCliff;
	public bool startLeft;

	private new void Awake()
	{
		base.Awake();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		if (startLeft)
			controls.Move(new Vector2(-1, 0));
		else
			controls.Move(new Vector2(1, 0));
	}

    // Update is called once per frame
    void Update()
    {
        if (controls.GetDirectionX() < 0)
		{
			float height = boxCollider.bounds.size.y;
			float halfWidth = (boxCollider.bounds.size.x / 2);

			Vector2 pointA = new Vector2(transform.position.x - halfWidth - 0.5f, transform.position.y + (height * 0.8f));
			Vector2 pointB = new Vector2(transform.position.x, transform.position.y + (height * 0.2f));

			if (Physics2D.OverlapArea(pointA, pointB, controls.GetLayerBlock()))
				controls.Move(new Vector2(1, 0));

			if (!fallCliff)
			{
				pointA = new Vector2(transform.position.x - halfWidth - 0.5f, transform.position.y);
				pointB = new Vector2(transform.position.x - halfWidth - 0.3f, transform.position.y - 0.1f);

				if (!Physics2D.OverlapArea(pointA, pointB, controls.GetLayerBlock()))
					controls.Move(new Vector2(1, 0));
			}
		}

		else if (controls.GetDirectionX() > 0)
		{
			float height = boxCollider.bounds.size.y;
			float halfWidth = (boxCollider.bounds.size.x / 2);

			Vector2 pointA = new Vector2(transform.position.x + halfWidth + 0.5f, transform.position.y +(height * 0.8f));
			Vector2 pointB = new Vector2(transform.position.x, transform.position.y + (height * 0.2f));

			if (Physics2D.OverlapArea(pointA, pointB, controls.GetLayerBlock()))
				controls.Move(new Vector2(-1, 0));

			if (!fallCliff)
			{
				pointA = new Vector2(transform.position.x + halfWidth + 0.5f, transform.position.y);
				pointB = new Vector2(transform.position.x + halfWidth + 0.3f, transform.position.y - 0.1f);

				if (!Physics2D.OverlapArea(pointA, pointB, controls.GetLayerBlock()))
					controls.Move(new Vector2(-1, 0));
			}
		}
	}
}
