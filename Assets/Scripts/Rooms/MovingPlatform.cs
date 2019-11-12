using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    GameObject target;
    Vector2 offset;

    // Public variables
    public float speed;
    public List<Vector3> points;
    int idx = 0;
    bool reverse = false;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, points[idx], speed * Time.deltaTime);

        if(transform.position == points[idx])
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

    void OnCollisionStay2D(Collision2D col)
    {
        target = col.gameObject;
        offset = target.transform.position - transform.position;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        target = null;
    }
}
