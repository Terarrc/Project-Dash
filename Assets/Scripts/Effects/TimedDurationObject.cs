using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDurationObject : MonoBehaviour
{
	private SpriteRenderer sprite;

	public float duration;
	public bool fade;
	private float timer;

    // Start is called before the first frame update
    void Awake()
    {
		timer = duration;
		sprite = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {
		if (fade)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, timer / duration);
		}

		float time = Time.deltaTime * 1000f;
		timer -= time;

		if (timer <= 0)
		{
			Destroy(gameObject);
		}
    }

	public void FlipX(bool value)
	{
		sprite.flipX = value;
	}
}
