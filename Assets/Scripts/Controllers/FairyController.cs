using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : Controller
{
    public GameObject following;
	public float offsetX = 0;
	public float offsetY = 0;
	public float timerOffset = 3000;
	private float minSpeed = 0.8f;
	private float maxSpeed = 3f;

	// Variable offset so the fairy doesn't stay in place
	private float secondaryOffsetX = 0;
	private float secondaryOffsetY = 0;
	private bool binded = false;

    // Start is called before the first frame update
    void Start() 
	{
        
    }

    // Update is called once per frame
    void Update() {
		minSpeed = Mathf.Clamp(1, minSpeed, maxSpeed);

		var offset = new Vector3 
		{
			x = offsetX + secondaryOffsetX,
			y = offsetY + secondaryOffsetY
		};

		if (following) 
		{
			Vector3 distance = transform.position - (following.transform.position + offset);
			if (!binded && distance.magnitude <= 1) 
			{
				// TODO Event 
			}
			else if (binded && distance.magnitude > 2)
			{

			}
        }
    }

	private void Move(Vector3 position, float speed) 
	{
		
	}
}
