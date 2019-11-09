using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
	public Player player;
	protected PixelPerfectCamera pixelCamera;

	private Vector3 velocity = Vector3.zero;
	public float smoothTime;

	// Start is called before the first frame update
	void Awake()
    {
		pixelCamera = GetComponent<PixelPerfectCamera>();
	}

    // Update is called once per frame
    void Update()
    {
		if (player == null || player.Room == null)
			return;
			

		float resolutionX = pixelCamera.refResolutionX / pixelCamera.assetsPPU;
		float resolutionY = pixelCamera.refResolutionY / pixelCamera.assetsPPU;

		transform.position = Vector3.MoveTowards(transform.position,
			new Vector3(
				Mathf.Clamp(player.transform.position.x, player.Room.Left + (resolutionX / 2), player.Room.Right - (resolutionX / 2)),
				Mathf.Clamp(player.transform.position.y, player.Room.Bottom + (resolutionY / 2), player.Room.Top - (resolutionY / 2)), 
				-10),
			100 * Time.deltaTime);
    }
}
