using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunbotEntity : Entity
{
	public Entity projectile;
	public float fireRate;
	public float burstRate;

	private float timerFire;
	private int fireCount;
	private float timerReload;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		layerCollision += 1 << LayerMask.NameToLayer("Robot");
		layerCollision += 1 << LayerMask.NameToLayer("Demon");
		layerBlock += 1 << LayerMask.NameToLayer("Robot");
		layerBlock += 1 << LayerMask.NameToLayer("Demon");
	}

	new void Update()
	{
		base.Update();

		if (timerFire > 0)
		{
			float time = Time.deltaTime * 1000;
			timerFire -= time;
		}

		if (timerReload > 0)
		{
			float time = Time.deltaTime * 1000;
			timerReload -= time;

			if (timerReload <= 0)
				fireCount = 0;
		}
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
			if (GetDirectionX() < 0)
			{
				Entity createdProjectile = Instantiate(projectile, new Vector3(transform.position.x - 0.5f, transform.position.y + 1.125f, 0), Quaternion.identity);
				createdProjectile.Move(new Vector2(-1, Random.Range(-1, 1)));
			}
			else
			{
				Entity createdProjectile = Instantiate(projectile, new Vector3(transform.position.x + 0.5f, transform.position.y + 1.125f, 0), Quaternion.identity);
				createdProjectile.Move(new Vector2(1, Random.Range(-1, 1)));
			}

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
