using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
	protected BoxCollider2D boxCollider;
	protected SpriteRenderer sprite;

	public float Left {get { return transform.position.x + boxCollider.offset.x - (boxCollider.bounds.size.x / 2); } }
	public float Right {get { return transform.position.x + boxCollider.offset.x + (boxCollider.bounds.size.x / 2); } }
	public float Top {get { return transform.position.y + boxCollider.offset.y + (boxCollider.bounds.size.y / 2); } }
	public float Bottom { get { return transform.position.y + boxCollider.offset.y - (boxCollider.bounds.size.y / 2); } }

	// Start is called before the first frame update
	void Start()
    {
		boxCollider = GetComponent<BoxCollider2D>();
		sprite = GetComponent<SpriteRenderer>();

		sprite.enabled = false;

		foreach (Transform child in transform)
		{
			if (child.CompareTag("Spawn"))
			{
				sprite = child.GetComponent<SpriteRenderer>();
				sprite.enabled = false;
			}
			child.gameObject.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null)
		{
			foreach (Transform child in transform)
				child.gameObject.SetActive(true);


			ColliderDistance2D colliderDistance = collider.Distance(boxCollider);

			float angle = Vector2.Angle(colliderDistance.normal, Vector2.up);

			Vector2 direction;
			if (angle < 45)
				direction = Vector2.up;
			else if (angle > 135)
				direction = Vector2.down;
			else if (collider.transform.position.x < boxCollider.transform.position.x)
				direction = Vector2.right;
			else
				direction = Vector2.left;


			player.EnterRoom(this, direction);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null)
		{
			foreach (Transform child in transform)
				child.gameObject.SetActive(false);
		}
	}
}
