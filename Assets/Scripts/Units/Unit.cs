using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IControls
{
    //all of the Unit's component
    protected Rigidbody2D body;
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;

    //On wake, get Unit's components
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	// ========================================================================
	// Controls
	// ========================================================================
	public bool Move(Vector2 direction, float ratio = 1)
	{
		return false;
	}

	public bool Jump()
	{
		return false;
	}

	public bool StopJump()
	{
		return false;
	}

}
