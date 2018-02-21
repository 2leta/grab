/****************
Script for 'grabber' (empty GameObject that strings will be attached to when held) to follow the camera's view
*****************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCam : MonoBehaviour {

	public float displace; // z-displacement: how far in front of the camera to hold strings? (set in properties panel)
	Camera cam;
	Vector3 camPoint;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		camPoint = new Vector3 (0.5f, 0.5f, displace); // centre of the screen, and slightly in front
	}
	
	// Update is called once per frame
	void Update () {
		if (Cord.grabbed) {
			transform.position = cam.ViewportToWorldPoint(camPoint);
		}
	}
}
