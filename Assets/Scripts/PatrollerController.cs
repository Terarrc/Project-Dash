using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerController : Controller
{
	public enum State { PatrolLeft, PatrolRight, Aggro, LostAggro }

	private BoxCollider2D boxCollider;

	public float aggroRange;

	private State state;
	private Unit aggro;
	private int layerAggro;

	private new void Awake()
	{
		base.Awake();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		state = State.PatrolLeft;

		layerAggro = unit.LayerGround - (1 << unit.gameObject.layer);
		switch (unit.faction)
		{
			case Unit.Faction.Robot:
				layerAggro += 1 << LayerMask.NameToLayer("Demon");
				break;

			case Unit.Faction.Demon:
				layerAggro += 1 << LayerMask.NameToLayer("Robot");
				layerAggro += 1 << LayerMask.NameToLayer("Player");
				break;

			default:
				break;
		}
	}


    // Update is called once per frame
    void Update()
    {
		switch (state)
		{
			case State.PatrolLeft:
				unit.Move(new Vector2(-0.5f, 0));
				CheckForWall(Vector2.left);
				CheckForCliff(Vector2.left);
				LookForEnemy(Vector2.left);
				break;
			case State.PatrolRight:
				unit.Move(new Vector2(0.5f, 0));
				CheckForWall(Vector2.right);
				CheckForCliff(Vector2.right);
				LookForEnemy(Vector2.right);
				break;
			case State.Aggro:
				AttackEnemy();
				break;
			case State.LostAggro:
				switch (unit.GetDirectionX())
				{
					case 1:
						state = State.PatrolRight;
						break;
					default:
						state = State.PatrolLeft;
						break;
				};
				break;
			default:
				break;
		}
	}

	private void CheckForWall(Vector2 direction)
	{
		RaycastHit2D hit = Physics2D.Raycast(unit.Position + (direction * ((unit.Size.x / 2) + 0.1f)), direction, 0.1f, unit.LayerGround);

		if (hit.collider != null && hit.collider.gameObject != unit.gameObject)
		{
			switch (state)
			{
				case State.PatrolLeft:
					state = State.PatrolRight;
					break;
				case State.PatrolRight:
					state = State.PatrolLeft;
					break;
				default:
					break;
			}
		}
	}

	private void CheckForCliff(Vector2 direction)
	{
		Vector2 origin = unit.Position + (direction * ((unit.Size.x / 2) + 0.1f)) + (Vector2.down * unit.Size.y / 2);

		RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, unit.LayerGround);

		if (hit.collider == null)
		{
			switch (state)
			{
				case State.PatrolLeft:
					state = State.PatrolRight;
					break;
				case State.PatrolRight:
					state = State.PatrolLeft;
					break;
				default:
					break;
			}
		}
	}

	private void AttackEnemy()
	{
		if (aggro == null)
		{
			state = State.LostAggro;
			return;
		}
	
		Vector2 distance = aggro.Position - unit.Position;

		if (distance.x < 0)
			unit.SetDirectionX(-1);
		else
			unit.SetDirectionX(1);

		if (distance.magnitude <= unit.AttackRange)
		{
			unit.Move(Vector2.zero);
			unit.Action(1);
			return;
		}
		else
		{
			RaycastHit2D hit = Physics2D.Raycast(unit.Position, distance, aggroRange, layerAggro);

			if (hit)
			{
				Unit unitSeen = hit.collider.gameObject.GetComponent<Unit>();

				if (unitSeen != null)
				{
					if (unitSeen.faction != unit.faction)
					{
						aggro = unitSeen;
					}
				}
				else
				{
					state = State.LostAggro;
				}
			}
			else
			{
				state = State.LostAggro;
			}
		}

		if (distance.x < 0)
			unit.Move(new Vector2(-1, 0));
		else
			unit.Move(new Vector2(1, 0));
	}

	private void LookForEnemy(Vector2 direction)
	{
		RaycastHit2D hit = Physics2D.Raycast(unit.Position, direction, aggroRange, layerAggro);

		if (hit.collider != null)
		{
			Unit unitSeen = hit.collider.gameObject.GetComponent<Unit>();

			if (unitSeen != null)
			{
				if (unitSeen.faction != unit.faction)
				{
					aggro = unitSeen;
					state = State.Aggro;
				}
			}
		}
	}


}
