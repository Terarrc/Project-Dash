using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Unit
{
	public float meleeDamages;
	public float attackRate;

	private float timerAttack;

	private void Start()
	{
		AttackRange = 1;
	}

	new void Update()
	{
		base.Update();

		if (timerAttack > 0)
			timerAttack -= Time.deltaTime;
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

	public override bool Move(Vector2 input)
	{
		if (timerAttack > 0)
			return base.Move(Vector2.zero);

		return base.Move(input);
	}

	private bool Attack()
	{
		if (timerAttack > 0)
			return false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(Position, 1.2f, (1 << LayerMask.NameToLayer("Robot")) + (1 << LayerMask.NameToLayer("Player")));

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
