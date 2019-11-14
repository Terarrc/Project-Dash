using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoomController : Controller
{
	private float timerTransition;
	private Player player;
	private Vector2 entryDirection;
	public Vector2 EntryDirection { 
		get
		{
			return entryDirection;
		}
		set
		{
			entryDirection = value;
			timerTransition = 0.2f;

			if (entryDirection == Vector2.up)
			{
				player.SetVelocity(new Vector2(0, 20));
				timerTransition = 0.6f;
			}
			else if (entryDirection == Vector2.down)
				player.SetVelocity(new Vector2(0, -20));
			else
				player.SetVelocity(Vector2.zero);
		}
 }

    // Start is called before the first frame update
    new void Awake()
    {
		base.Awake();
		player = unit as Player;
	}

    // Update is called once per frame
    void Update()
    {
		if (timerTransition > 0)
		{
			timerTransition -= Time.deltaTime;
			if (timerTransition <= 0)
				player.EnterRoomOver();
			if (EntryDirection == Vector2.left)
				player.Move(Vector2.left);
			else if (EntryDirection == Vector2.right)
				player.Move(Vector2.right);
		}
    }
}
