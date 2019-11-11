using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
	public Vector3 RespawnPoint { get; set; }

	public override void Kill(GameObject source)
	{
		currentHealth = maxHealth;
		gameObject.transform.position = RespawnPoint;
	}

	public void SetRespawnPoint(Vector3 position)
	{
		RespawnPoint = position;
	}

	/*
	 * 
	protected float timerInvulnerable;
	protected float invulnerableTime = 0;
	protected float timerBlink;
	protected float blinkTime = 100;

	 * 		float time = Time.deltaTime * 1000;

		if (timerInvulnerable > 0)
		{
			timerInvulnerable -= time;
			if (timerInvulnerable <= 0)
			{
				if (sprite)
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
				timerBlink = 100;
			}
			else
			{
				timerBlink -= time;
				if (timerBlink <= -blinkTime)
					timerBlink = blinkTime;
				if (sprite)
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Abs(timerBlink) / blinkTime);
			}
		}
	 */
}
