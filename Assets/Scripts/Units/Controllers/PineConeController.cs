using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineConeController : Controller
{
	public float moveUp;
	public float moveDown;

	private float positionTop;
	private float positionBot;

	public enum State { MovingUp, MovingDown, Falling }
	private State state = State.MovingUp;

	override public void Reset()
	{
		state = State.MovingUp;
	}

    // Start is called before the first frame update
    void Start()
    {
		positionTop = transform.position.y + moveUp;
		positionBot = transform.position.y - moveDown;
	}

    // Update is called once per frame
    void Update()
    {
        switch (state)
		{
			case State.MovingUp:
				unit.Move(Vector2.up);

				if (unit.Position.y > positionTop)
					state = State.MovingDown;
				break;
			case State.MovingDown:
				unit.Move(Vector2.down);

				if (unit.Position.y < positionBot)
					state = State.MovingUp;
				break;
			default:
				break;
		}
    }
}
