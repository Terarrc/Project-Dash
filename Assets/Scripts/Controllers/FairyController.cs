using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : Controller
{
    public Unit following;
	public float ratioSpeedBinded;
	public Rect wanderingRect = new Rect();

	// Variable offset so the fairy doesn't stay in place
	private Vector3 offset;
	private bool binded = false;

    // Start is called before the first frame update
    void Start() 
	{
		offset = new Vector3
		{
			x = Random.Range(wanderingRect.xMin, wanderingRect.xMax),
			y = Random.Range(wanderingRect.yMin, wanderingRect.yMax)
		};
	}

    // Update is called once per frame
    void Update() {
		if (following) 
		{
			Vector3 distance;
			if (binded)
				distance = (following.transform.position + offset) - transform.position;
			else
				distance = (following.transform.position + new Vector3(0, 1)) - transform.position;

			// Check if we're at the right place
			if (distance.magnitude < 0.2)
			{
				offset.x = Random.Range(wanderingRect.xMin, wanderingRect.xMax);
				offset.y = Random.Range(wanderingRect.yMin, wanderingRect.yMax);
			}
			else
			{
				// The fairy move slowly if close to the unit she follows
				if (binded)
				{
					controls.Move(new Vector2(distance.x, distance.y).normalized * ratioSpeedBinded);
				}
				else
				{
					controls.Move(new Vector2(distance.x, distance.y).normalized);
				}

			}

			// Check if she's bind or unbind
			if (!binded && distance.magnitude <= 1) 
			{
				binded = true;
			}
			else if (binded && distance.magnitude > 3)
			{
				binded = false;
			}
        }
    }
}
