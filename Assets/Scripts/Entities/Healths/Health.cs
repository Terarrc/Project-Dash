using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
	public enum DamageType { Physic, Energetic, SuperEnergetic }

	protected SpriteRenderer sprite;

	public TimedDurationObject deathEffect;

	public int maxHealth;
	protected int currentHealth;

	public float ratioPhysic;
	public float ratioEnergetic;
	public float ratioSuperEnergetic;

	protected float timerInvulnerable;
	protected float invulnerableTime = 0;
	protected float timerBlink;
	protected float blinkTime = 100;

	public void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	public void Start()
	{
		currentHealth = maxHealth;
	}

	public void Update()
	{
		float time = Time.deltaTime * 1000;

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
	}

	public void ApplyDamage(float amount, DamageType damageType, GameObject source)
	{
		if (timerInvulnerable > 0)
			return;

		int damage = 0;
		switch (damageType)
		{
			case DamageType.Physic:
				damage = Mathf.RoundToInt(amount * ratioPhysic);
				break;
			case DamageType.Energetic:
				damage = Mathf.RoundToInt(amount * ratioEnergetic);
				break;
			case DamageType.SuperEnergetic:
				damage = Mathf.RoundToInt(amount * ratioSuperEnergetic);
				break;
		}
		currentHealth -= damage;

		// If a damage is taken, the entity is briefly invulnerable
		if (damage >= 1)
			timerInvulnerable = invulnerableTime;

		if (currentHealth <= 0)
			Kill(source);
	}

	protected virtual void Kill(GameObject source)
	{
		if (deathEffect)
		{
			Instantiate(deathEffect, transform.position, Quaternion.identity);
		}

		Destroy(gameObject);
	}
}
