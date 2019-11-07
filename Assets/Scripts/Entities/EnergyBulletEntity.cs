using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBulletEntity : Entity
{
	public float damage;
	public GameObject Creator { get; set; }

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		layerCollision += 1 << LayerMask.NameToLayer("Demon");
	}

	private void Collide(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Demon"))
		{
			Health health = collider.gameObject.GetComponent<Health>();
			if (health)
				health.ApplyDamage(damage, Health.DamageType.Energetic, Creator);
		}

		if (collider.gameObject.layer != LayerMask.NameToLayer("Vertical Energy Field") && collider.gameObject.layer != LayerMask.NameToLayer("Horizontal Energy Field"))
			Destroy(gameObject);
	}

	protected override float CollideX(Collider2D collider, float positionX)
	{
		Collide(collider);

		return 0;
	}

	protected override float CollideY(Collider2D collider, float positionY)
	{
		Collide(collider);

		return 0;
	}
}
