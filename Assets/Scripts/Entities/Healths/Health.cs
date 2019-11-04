using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
	public enum DamageType { Physic, Energetic, SuperEnergetic }

	public TimedDurationObject deathEffect;

	public int maxHealth;
	public int currentHealth;

	public float ratioPhysic;
	public float ratioEnergetic;
	public float ratioSuperEnergetic;

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

		Kill(source);
	}

	private void Kill(GameObject source)
	{
		if (deathEffect)
		{
			Instantiate(deathEffect, transform.position, Quaternion.identity);
		}

		Destroy(this);
	}
}
