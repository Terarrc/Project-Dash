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

		layerAggro = unit.LayerGround;
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
				if (CheckForWall(Vector2.left))
					state = State.PatrolRight;
				if (CheckForCliff(Vector2.left))
					state = State.PatrolRight;
				LookForEnemy(Vector2.left);
				break;
			case State.PatrolRight:
				unit.Move(new Vector2(0.5f, 0));
				if (CheckForWall(Vector2.right))
					state = State.PatrolLeft;
				if (CheckForCliff(Vector2.right))
					state = State.PatrolLeft;
				LookForEnemy(Vector2.right);
				break;
			case State.Aggro:
				if (!AttackEnemy())
				{
					if (CheckForWall(unit.GetDirection()))
						unit.Move(Vector2.zero);
				}
				break;
			case State.LostAggro:
				if (unit.GetDirection() == Vector2.right)
					state = State.PatrolRight;
				else
					state = State.PatrolLeft;
				break;
			default:
				break;
		}
	}
/*
	protected void OnCollisionEnter2D(Collision2D collision)
	{
		ColliderDistance2D colliderDistance = collision.collider.Distance(boxCollider);

		// Check if the collision is horizontal
		if ((Vector2.Angle(colliderDistance.normal, Vector2.up) > 45) && (Vector2.Angle(colliderDistance.normal, Vector2.up) < 135))
		{
			switch (state)
			{
				case State.PatrolLeft:
					state = State.PatrolRight;
					break;
				case State.PatrolRight:
					state = State.PatrolLeft;
					break;
				case State.Aggro:
					unit.Move(Vector2.zero);
					break;
				default:
					break;
			}
		}

	}
	*/
	private bool CheckForWall(Vector2 direction)
	{
		RaycastHit2D hit = Physics2D.Raycast(unit.Position + (direction * ((unit.Size.x / 2) + 0.1f)) + (Vector2.up * (unit.Size.y / 2)), Vector2.down, unit.Size.y - 0.1f, unit.LayerGround + (1 << unit.gameObject.layer));

		if (hit.collider != null && hit.collider.gameObject != unit.gameObject)
			return true;
		
		return false;
	}

	private bool CheckForCliff(Vector2 direction)
	{
		if (!unit.IsGrounded)
			return false;

		Vector2 origin = unit.Position + (direction * ((unit.Size.x / 2) + 0.1f)) + (Vector2.down * (unit.Size.y / 2));

		RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, unit.LayerGround);

		if (hit.collider == null)
			return true;


		return false;
	}

	private bool AttackEnemy()
	{
		if (aggro == null)
		{
			state = State.LostAggro;
			return false;
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
			return true;
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

		if (CheckForCliff(unit.GetDirection()))
			unit.Move(Vector2.zero);

		else if (distance.x < 0)
			unit.Move(new Vector2(-1, 0));
		else
			unit.Move(new Vector2(1, 0));

		return false;
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
