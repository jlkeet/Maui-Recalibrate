using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


	private Vector2 velocity;

	public float smoothTimeY;
	public float smoothTimeX;

	public GameObject player;

	public bool bounds;
	public Vector3 minCamPos;
	public Vector3 maxCamPos;


	// Use this for initialization
	void Start () {

		player = GameObject.FindGameObjectWithTag ("Player");

	}

	void FixedUpdate() {

		float posX = Mathf.SmoothDamp (transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
		float posY = Mathf.SmoothDamp (transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

		transform.position = new Vector3 (posX, posY, transform.position.z);

		if (bounds) {
			transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minCamPos.x, maxCamPos.x), 
				Mathf.Clamp (transform.position.y, minCamPos.y, maxCamPos.y),
				Mathf.Clamp (transform.position.z, minCamPos.z, maxCamPos.z)
			);
		}
	}

	public void SetMinCamPosition() {
		minCamPos = gameObject.transform.position;
	}

	public void SetMaxCamPosition() {
		maxCamPos = gameObject.transform.position;

	}

}
