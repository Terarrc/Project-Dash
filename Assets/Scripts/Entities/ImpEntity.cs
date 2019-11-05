using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpEntity : Entity
{
	public float meleeDamages;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		layerCollision += 1 << LayerMask.NameToLayer("Player");
		layerCollision += 1 << LayerMask.NameToLayer("Robot");
	}


	protected override float CollideX(Collider2D collider, float positionX)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player") || collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
			AttackEntity(collider.gameObject);

		return base.CollideX(collider, positionX);
	}

	protected override float CollideY(Collider2D collider, float positionY)
	{
		return base.CollideY(collider, positionY);
	}

	private void AttackEntity(GameObject target)
	{
		Health targetHealth = target.GetComponent<Health>();

		if (targetHealth)
			targetHealth.ApplyDamage(meleeDamages, Health.DamageType.Physic, gameObject);
	}
}
