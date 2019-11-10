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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null)
		{
			player.Room = this;

			Transform closestSpawn = null;

			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(true);
				if (child.CompareTag("Spawn"))
				{
					if (closestSpawn == null)
						closestSpawn = child;
					else if ((player.transform.position - child.position).magnitude < (player.transform.position - closestSpawn.position).magnitude)
						closestSpawn = child;
				}
			}

			if (closestSpawn != null)
			{
				player.transform.position = closestSpawn.position;
				player.SetRespawnPoint(closestSpawn.position);
			}
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
