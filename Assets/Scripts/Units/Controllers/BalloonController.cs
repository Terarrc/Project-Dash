using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : Controller
{
	// Public variables
	public List<Vector3> points;
	int idx = 0;
	bool reverse = false;

	override public void Reset()
	{
		idx = 0;
		reverse = false;
	}

	private void Update()
	{
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, points[idx], unit.speed.x * Time.deltaTime);

		if (transform.localPosition == points[idx])
		{
			if (reverse && idx > 0)
				idx--;
			else
				reverse = false;

			if (!reverse && idx < points.Count - 1)
				idx++;
			else
				reverse = true;
		}
	}
}
