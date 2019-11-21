using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Public variables
    public float speed;
    public List<Vector3> points;
    int idx = 0;
    bool reverse = false;

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, points[idx], speed * Time.fixedDeltaTime);

        if(transform.localPosition == points[idx])
        {
            if (reverse && idx > 0)
                idx--;
            else
                reverse = false;

            if(!reverse && idx < points.Count - 1)
                idx++;
            else
                reverse = true;           
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        col.collider.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D col)
    {
		if (gameObject.activeSelf)
		{
			if (col.collider.GetComponent<Player>() != null)
				col.collider.transform.SetParent(null);
			else
				col.collider.transform.SetParent(transform.parent);
		}
    }
}
