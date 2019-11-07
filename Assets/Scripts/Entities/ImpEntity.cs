using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpEntity : Entity
{
	public float meleeDamages;
	public float attackRate;

	private float timerAttack;

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

		if (timerAttack > 0)
			timerAttack -= Time.deltaTime * 1000;
	}

	public override bool Action(int index)
	{
		switch (index)
		{
			case 1:
				return Attack();
			default:
				break;
		}
		return false;
	}

	private bool Attack()
	{
		if (timerAttack > 0)
			return false;

		float height = boxCollider.bounds.size.y;
		float halfWidth = (boxCollider.bounds.size.x / 2);

		Vector2 point = new Vector2(transform.position.x, transform.position.y + (height * 0.5f));

		Collider2D[] colliders = Physics2D.OverlapCircleAll(point, 1.2f, (1 << LayerMask.NameToLayer("Robot")) + (1 << LayerMask.NameToLayer("Player")));

		// Move the body the furthest possible without collision
		foreach (Collider2D collider in colliders)
		{
			Health targetHealth = collider.GetComponent<Health>();

			if (targetHealth)
				targetHealth.ApplyDamage(meleeDamages, Health.DamageType.Physic, gameObject);
		}

		timerAttack = attackRate;
		animator.SetTrigger("Attack");

		return true;
	}
}
