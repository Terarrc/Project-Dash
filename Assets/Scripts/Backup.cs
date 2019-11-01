using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		/*
// Overlap ground check
// Energy Field ground check
// Make a rectangle out of two edges (Vector 2) and if that rectangle overlaps with layer (8) it returns true
if (boxCollider)
{
var horizontalHitboxHalf = boxCollider.bounds.size.x / 2; 
var verticalHitboxQuarter = boxCollider.bounds.size.y / 4;


// Ground Check
isGrounded = Physics2D.OverlapArea(
	new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y),
	new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y), 1 << 8);

animator.SetBool("Grounded", isGrounded);

// Horizontal Energy Field Ground Check
isOnHorizontalEnergyField = Physics2D.OverlapArea(
	new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y),
	new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y), 1 << 9);

Debug.Log("Horizontal  : " + isOnHorizontalEnergyField);

if (!isGrounded)
{
	// Vertical Energy Field Check Right Side
	isOnVerticalEnergyField_Right = Physics2D.OverlapArea(
	   new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
	   new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3), 1 << 10);

	// Vertical Energy Field Check Left Side
	isOnVerticalEnergyField_Left = Physics2D.OverlapArea(
	   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
	   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3), 1 << 10);               

	Debug.Log("Vertical Right : " + isOnVerticalEnergyField_Right);
	Debug.Log("Vertical Left : " + isOnVerticalEnergyField_Left);
}  
}
	*/

		/* 
 * Display wall checkers as black lines
 * 
private void OnDrawGizmos()
{
	if (boxCollider)
	{
		var horizontalHitboxHalf = boxCollider.bounds.size.x / 2;
		var verticalHitboxQuarter = boxCollider.bounds.size.y / 4;

		Gizmos.color = Color.black;
		Gizmos.DrawLine(
			new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
			new Vector2(transform.position.x + horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3));
		Gizmos.DrawLine(
		   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter),
		   new Vector2(transform.position.x - horizontalHitboxHalf, transform.position.y + verticalHitboxQuarter * 3));
	}

}
*/

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
