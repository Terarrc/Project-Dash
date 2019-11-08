using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody2D body;
	protected SpriteRenderer sprite;

	public float damage;
	public Health.DamageType type;
	public Unit.Faction faction;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		sprite = GetComponent<SpriteRenderer>();
	}

	public void SetVelocity(Vector2 velocity)
	{
		body.velocity = velocity;
		if (sprite != null)
			sprite.flipX = velocity.x < 0;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Unit unit = collision.gameObject.GetComponent<Unit>();

		if (unit != null)
		{
			if (unit.faction != faction)
			{
				unit.ApplyDamage(damage, type, gameObject);
				Destroy(gameObject);
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
