using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunbot : Unit
{
	public Projectile projectile;
	public float fireRate;
	public float burstRate;

	private float timerFire;
	private int fireCount;
	private float timerReload;

	private void Start()
	{
		AttackRange = 8;
	}

	new void Update()
	{
		base.Update();

		if (timerFire > 0)
		{
			float time = Time.deltaTime;
			timerFire -= time;
		}

		if (timerReload > 0)
		{
			float time = Time.deltaTime;
			timerReload -= time;

			if (timerReload <= 0)
				fireCount = 0;
		}
	}
	public override bool Move(Vector2 input)
	{
		if (timerFire > 0)
			return base.Move(Vector2.zero);

		return base.Move(input);
	}

	public override bool Action(int index)
	{
		switch (index)
		{
			case 1:
				return Fire();
			default:
				break;
		}
		return false;
	}

	private bool Fire()
	{
		if (timerFire > 0)
			return false;
		else
		{
			Projectile createdProjectile = Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + (6f / 16f), 0), Quaternion.identity);
			createdProjectile.faction = Faction.Robot;
			createdProjectile.SetVelocity((GetDirection() * 20) + (Vector2.up * Random.Range(-0.1f, 0.1f)));

			if (fireCount == 2)
			{
				timerFire = fireRate;
				timerReload = fireRate;
				fireCount = 0;
			}
			else
			{
				timerFire = burstRate;
				timerReload = fireRate;
				fireCount++;
			}

			animator.SetTrigger("Fire");
			return true;
		}
	}
}
