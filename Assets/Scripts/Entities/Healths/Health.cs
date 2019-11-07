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

	private float redTime = 200;
	private float timerRed;

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
		if (timerRed > 0)
		{
			timerRed -= Time.deltaTime * 1000;
			if (timerRed < 0)
				timerRed = 0;
			sprite.color = new Color(1, 1 - (timerRed / redTime), 1 - (timerRed / redTime));
		}
	}

	public void ApplyDamage(float amount, DamageType damageType, GameObject source)
	{
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

		Entity entity = GetComponent<Entity>();
		if (entity != null)
		{
			if (transform.position.x < source.transform.position.x)
				entity.ApplySpeed(new Vector2(-10, 5));

			else
				entity.ApplySpeed(new Vector2(10, 5));
		}

		// If a damage is taken, the entity is briefly invulnerable
		if (damage >= 1)
		{
			timerRed = redTime;
			sprite.color = new Color(1, 0, 0);
		}


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
