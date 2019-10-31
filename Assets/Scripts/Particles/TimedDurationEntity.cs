using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDurationEntity : MonoBehaviour
{
	public float duration;
	private float timer;

    // Start is called before the first frame update
    void Start()
    {
		timer = duration;
    }

    // Update is called once per frame
    void Update()
    {
		float time = Time.deltaTime * 1000f;
		timer -= time;
		if (timer <= 0)
		{
			Destroy(gameObject);
		}
    }
}
