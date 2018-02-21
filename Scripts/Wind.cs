/****************
For sound purposes:
The wind object wanders around; as it approaches a cord, it whistles according to its tuning
It is not itself a sound emitter
*****************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

	public float bounds = 10f; // dimensions of square defining the area wind is limited to
	Vector3 centre; // centrepoint of that square

	// movement parameters adjustable in properties panel
	public float maxSpeed = 0.3f;
	public float minSpeed = 0.05f;
	public float turnSpeed = 1f; // speed of rotation to look at target
	public float acceleration = 1f;
	public float fixTime = 4f;

	// internal states
	float speed;
	float nextSpeed;

	Vector3 target;

	void Start()
	{
		centre = transform.position; // define centrepoint of bounding square as starting position
		speed = Random.Range(minSpeed, maxSpeed);
		transform.position = NewTarget();
		FixTarget();
	}

	// turn towards target and head forward, while varying speed
	void Update()
	{
		speed = Mathf.Lerp(speed, nextSpeed, Time.deltaTime * acceleration);
		var targetRotation = Quaternion.LookRotation(target-transform.position); // always look towards target
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed * speed);
		transform.position += transform.forward * speed;
		// new target is set every [fixTime] seconds (via Invoke) or when wind gets close enough
		if (Vector3.Distance(transform.position, target) < 2) {
			CancelInvoke("FixTarget");
			FixTarget();
			Invoke("FixTarget", fixTime);
		}
	}

	// randomise next target position and speed
	Vector3 NewTarget()
	{
		float x = centre.x + Random.Range(-bounds, bounds);
		float z = centre.z + Random.Range(-bounds, bounds);
		Vector3 tar = new Vector3(x, centre.y, z);
		nextSpeed = Random.Range(minSpeed, maxSpeed);
		return tar;
	}

	void FixTarget()
	{
		target = NewTarget();
	}
}