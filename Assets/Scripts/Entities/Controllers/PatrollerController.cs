using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerController : Controller
{
	private BoxCollider2D boxCollider;

	public bool fallCliff;
	public bool startLeft;
	public float attackRange;
	public float aggroRange;
	public float cancelAggroRange;

	public Entity.Faction faction;

	private GameObject aggro;
	private int layerAggro;
	// Timer when we want to move after an attack to prevent animator bug
	private float timerMoveAgain;

	private new void Awake()
	{
		base.Awake();
		boxCollider = GetComponent<BoxCollider2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		if (startLeft)
			controls.Move(new Vector2(-1, 0));
		else
			controls.Move(new Vector2(1, 0));

		layerAggro = controls.GetLayerBlock();
		switch (faction)
		{
			case Entity.Faction.Robot:
				layerAggro += 1 << LayerMask.NameToLayer("Demon");
				break;

			case Entity.Faction.Demon:
				layerAggro += 1 << LayerMask.NameToLayer("Robot");
				break;

			default:
				break;
		}
	}

    // Update is called once per frame
    void Update()
    {
		float height = boxCollider.bounds.size.y;
		if (aggro != null)
		{
			float distanceX = transform.position.x - aggro.transform.position.x;
			float distanceY = transform.position.y - aggro.transform.position.y;

			if (Mathf.Abs(distanceX) > cancelAggroRange || Mathf.Abs(distanceY) > 5)
				aggro = null;
			else
			{
				if (transform.position.x < aggro.transform.position.x && Mathf.Abs(distanceX) > attackRange)
				{
					RestartMove(new Vector2(1, 0));
				}
				else if (transform.position.x > aggro.transform.position.x && Mathf.Abs(distanceX) > attackRange)
				{
					RestartMove(new Vector2(-1, 0));
				}
				else
				{
					controls.Move(Vector2.zero);
					// Don't move immediatly after an attack
					timerMoveAgain = 200;

					if (transform.position.x < aggro.transform.position.x)
						controls.Turn(1);
					if (transform.position.x > aggro.transform.position.x)
						controls.Turn(-1);
					controls.Action(1);
				}

				return;
			}
		}

		if (controls.GetDirectionX() < 0)
		{

			float halfWidth = (boxCollider.bounds.size.x / 2);
			Vector2 pointA;
			RaycastHit2D hit;


			pointA = new Vector2(transform.position.x, transform.position.y + 0.1f);
				
			hit = Physics2D.Raycast(pointA, Vector2.left, aggroRange, layerAggro);

			if (hit.collider != null)
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Demon") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
				{
					aggro = hit.collider.gameObject;
				}
			}
			else
			{
				RestartMove(new Vector2(-0.5f, 0));
			}		

			pointA = new Vector2(transform.position.x - halfWidth - 0.1f, transform.position.y + height);

			hit = Physics2D.Raycast(pointA, Vector2.down, height - 0.1f, controls.GetLayerBlock());

			if (hit.collider != null)
			{
				RestartMove(new Vector2(0.5f, 0));
				return;
			}

			if (!fallCliff)
			{
				pointA = new Vector2(transform.position.x - halfWidth - 0.1f, transform.position.y);

				hit = Physics2D.Raycast(pointA, Vector2.down, 0.1f, controls.GetLayerBlock());

				if (hit.collider == null)
				{
					RestartMove(new Vector2(0.5f, 0));
					return;
				}

			}
		}

		else if (controls.GetDirectionX() > 0)
		{
			float halfWidth = (boxCollider.bounds.size.x / 2);

			Vector2 pointA;
			RaycastHit2D hit;

	
			pointA = new Vector2(transform.position.x, transform.position.y + 0.1f);

			hit = Physics2D.Raycast(pointA, Vector2.right, aggroRange, layerAggro);

			if (hit.collider != null)
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Demon") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Robot"))
				{
					aggro = hit.collider.gameObject;
				}
			}
			else
			{ 
				RestartMove(new Vector2(0.5f, 0));
			}
	

			pointA = new Vector2(transform.position.x + halfWidth + 0.1f, transform.position.y + height);

			hit = Physics2D.Raycast(pointA, Vector2.down, height - 0.1f, controls.GetLayerBlock());

			if (hit.collider != null)
			{
				RestartMove(new Vector2(-0.5f, 0));
				return;
			}

			if (!fallCliff)
			{
				pointA = new Vector2(transform.position.x + halfWidth + 0.1f, transform.position.y);

				hit = Physics2D.Raycast(pointA, Vector2.down, 0.1f, controls.GetLayerBlock());

				if (hit.collider == null)
				{
					RestartMove(new Vector2(-0.5f, 0));
					return;
				}
			}
		}
	}
	private void RestartMove(Vector2 direction)
	{
		if (timerMoveAgain > 0)
			timerMoveAgain -= Time.deltaTime * 1000;
		if (timerMoveAgain <= 0)
			controls.Move(direction);
	}
}
