using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : Controller
{
    public Unit following;

    // Start is called before the first frame update
    void Start() 
	{

	}

    // Update is called once per frame
    void Update() {
		if (following) 
		{
			Vector3 distance;

			if (following.GetDirectionX() < 0)
				distance = (following.transform.position + new Vector3(1.5f, 2)) - transform.position;
			else
				distance = (following.transform.position + new Vector3(-1.5f, 2)) - transform.position;

			// Check if we're at the right place
			if (distance.magnitude > 1)
				controls.Move(new Vector2(distance.x, distance.y).normalized * Mathf.Min(1, distance.magnitude));
			else
			{
				controls.Move(Vector2.zero);
				controls.Turn(following.transform.position.x - transform.position.x);
			}

		}
    }
}
