using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public float crumblingTime;
    private float timerCrumble;
    private bool crumbling;

    // Start is called before the first frame update
    void Start()
    {
        crumbling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(crumbling)
        {
            timerCrumble -= Time.deltaTime;
            if(timerCrumble <= 0)
            {
                // Break Platform 
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Start Timer
        crumbling = true;
        timerCrumble = crumblingTime;

        // Set Crumbling Animation

    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Stop Crumbling
        crumbling = false;

        // Stop Crumbling Animation
    }
}
