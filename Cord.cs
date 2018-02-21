/****************
To be added to parent cord object
*****************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class Cord : MonoBehaviour {

	// for smoothing of cord movement (Vector3.SmoothDamp())
	static Vector3 velocity = Vector3.zero;
	const float smoothing = 0.1f; // speed of smoothing
	static bool doSmooth; // to enable/disable SmoothDamp

	// for sound
	float tuning; // cord's tuning, calculated from its length
	const float tuningFactor = 6f; // how length influences tuning

	// static states
	public static bool grabbed; // is player holding a cord?
	public static Cord inHand; // which cord is being held?

	// instance-specific states
	public GameObject start; // GameObjects the line is hooked to...
	public GameObject end; // ...(initially set in properties panel)
	Vector3 startPos;
	Vector3 endPos;

	// convenience object references
	LineRenderer line;
	CapsuleCollider[] colliders; // colliders to detect interaction with end sections and middle of line
	public static GameObject grabber; // GameObject in front of camera that holds cord when grabbed
	static Transform player;
	static Vector3 playerPos; // logs player position to determine if it changes
	GameObject soundEmitter; // sound emitter for this cord
	static Transform wind; // position of wind (its distance from cord modulates sound)

	// colours
	Color32 c_idle = new Color32(200,111,120,255); // red
	Color32 c_grab = new Color32(64,255,255,255); // blue
	Color32 c_hook = new Color32(45,255,110,255); // green

	// hover colours defined by game state
	Color32 C_Active {
		get {
			if (grabbed)
				return c_hook;
			else
				return c_grab;
		}
	}

	Color32 C_Idle {
		get {
			if (grabbed)
				return c_grab;
			else
				return c_idle;
		}
	}

	void Start()
	{
		// initialise object references
		line = gameObject.GetComponent<LineRenderer>();
		colliders = gameObject.GetComponentsInChildren<CapsuleCollider>();
		grabber = GameObject.FindWithTag("Grabber");
		player = GameObject.FindWithTag("Player").transform;
		playerPos = player.position;
		soundEmitter = gameObject.GetComponentInChildren<AkAmbient>().gameObject;
		wind = GameObject.FindWithTag("Wind").transform;

		startPos = start.transform.position;
		endPos = end.transform.position;

		// the Hook script belongs to objects the cord can be attached to
		// tell Hooks that this cord is attached to them
		start.GetComponent<Hook>().Attach(this);
		end.GetComponent<Hook>().Attach(this);

		// configure colliders
		for (int i = 0; i < 3; i++) {
			colliders[i].radius = 0.2f;
			colliders[i].direction = 2; // make sure capsule's long vector is Z-axis
			colliders[i].center = Vector3.zero; // local position
			colliders[i].isTrigger = true;
			colliders[i].gameObject.GetComponent<CordSeg>().mySeg = (Segment)i-1;
		}

		line.startColor = c_idle;
		line.endColor = c_idle;

		Render();
	}
	
	// Update is called once per frame
	void Update()
	{
		// render when necessary; cord only changes when:
		// 	smoothing is happening, this instance is grabbed, and the end position has changed
		if (doSmooth && inHand == this && end.transform.position != endPos) {
			endPos = Vector3.SmoothDamp(endPos, end.transform.position, ref velocity, smoothing);
			Render();
			//AkSoundEngine.SetRTPCValue("Tuning", tuning, grabber);
		}

		if (playerPos != player.position) {
			playerPos = player.position;
			// to get the effect that the sound is distributed along the length of the cord
			// place sound emitter at the point on the line nearest the player
			soundEmitter.transform.position = Linear.NearPointOnLine(startPos, endPos, playerPos);
		}

		// the wind object moves stochastically around the area (see Wind.cs)
		// wind position determines 
		Vector3 nearToWind = Linear.NearPointOnLine(startPos, endPos, wind.position);
		float windIntensity = 1/(Vector3.Distance(nearToWind, wind.position));
		AkSoundEngine.SetRTPCValue("Wind", windIntensity, soundEmitter);
	}

	// 
	void Render()
	{
		float len = Vector3.Distance(startPos, endPos); // cord length

		// update transform - this determines positions of colliders
		transform.position = startPos + (endPos-startPos)/2f; // place midway between endpoints
		transform.LookAt(startPos); // orientate along line

		// colliders are only used when cord not grabbed, so only update them then
		if (!grabbed) UpdateColliders();

		// update LineRenderer positions - **correct rendering of the line happens here**
		if (len < 4f) {
			line.positionCount = 2;
			line.SetPosition(0, startPos);
			line.SetPosition(1, endPos);
		} else { // if cord is long enough...
			// extra points in the LineRenderer act as bounds on the colour gradient at either end
			line.positionCount = 4;
			line.SetPosition(0, startPos);
			line.SetPosition(1, startPos - Vector3.Normalize(startPos-endPos)*2f); // 2 units along from start
			line.SetPosition(2, endPos - Vector3.Normalize(endPos-startPos)*2f); //   "    "     "       end  
			line.SetPosition(3, endPos);
		}

		// update tuning and change sound parameter accordingly
		tuning = Vector3.Distance(startPos, endPos)*tuningFactor;
		AkSoundEngine.SetRTPCValue("Tuning", tuning, soundEmitter);
	}

	// manage the 3 CapsuleColliders
	void UpdateColliders()
	{
		// collider [0] is 1 unit long near the start point of the cord
		// collider [1] is in the middle, stretching to 1 unit from either end
		// collider [2] is 1 unit long at end point
		colliders [0].center = Vector3.forward * (Vector3.Distance(startPos, endPos) / 2f - 0.5f);
		colliders [2].center = -Vector3.forward * (Vector3.Distance(startPos, endPos) / 2f - 0.5f);
		colliders [0].height = 1f;
		colliders [1].height = Vector3.Distance(startPos, endPos) - 2f;
		colliders [2].height = 1f;

		// collider updates its own start and end point variables, for its own purposes
		foreach (CapsuleCollider c in colliders)
			c.GetComponent<CordSeg>().KnowBounds(c);
	}

	// collider OnMouse events call these methods (passing the Segment of the triggered collider)
	public void MouseOver(Segment seg)
	{
		// set colours depending on whether you're hovering over the start, mid, or end of line
		if (seg < Segment.End)
			line.startColor = C_Active;
		if (seg > Segment.Start)
			line.endColor = C_Active;
		if (seg == Segment.Mid) {
			// when hovering over line, we want to highlight the whole line
			// in that case, we don't want extra points that define gradients (see Render())
			line.positionCount = 2;
			line.SetPosition(1, endPos);
		}
	}

	public void MouseOff(Segment seg)
	{
		if (seg < Segment.End)
			line.startColor = C_Idle;
		if (seg > Segment.Start)
			line.endColor = C_Idle;
		if (seg == Segment.Mid)
			Render(); // to reset gradient-related positions on the line
	}

	// placeholder for what happens when middle of cord is clicked (it's 'plucked')
	public void Pluck()
	{
		Color colour = Random.ColorHSV(0f, 1f, 0.3f, 0.5f, 0.7f, 0.9f);
		line.startColor = colour;
		line.endColor = colour;
	}

	// remove cord from its hook and hold it
	public void Grab(Segment seg)
	{
		grabbed = true;
		doSmooth = true;
		inHand = this; // this instance of Cord is currently being held
		CancelInvoke("StopSmooth"); // in case Cord is grabbed again immediately after being hooked (see Hook())

		// for simplicity, end is always the one that's grabbed
		// start and end are swapped if necessary to make this the case
		if (seg == Segment.Start) {
			start.GetComponent<Hook>().Detach(this);
			// swap start and end
			GameObject temp = start;
			start = end;
			end = temp;
		} else
			end.GetComponent<Hook>().Detach(this);

		startPos = start.transform.position;
		endPos = end.transform.position;
		// transfer end from hook to 'grabber' (point in front of camera)
		// (this happens after updating startPos & endPos so that SmoothDamp can interpolate from old end position)
		end = grabber;
		line.endColor = c_grab;

		// colliders should only respond to mouse events when cord isn't grabbed: disable colliders
		foreach (CapsuleCollider c in colliders) c.enabled = false;

		// grab sound effect
		AkSoundEngine.PostEvent("Grab", grabber);
	}

	// attach grabbed cord to hook
	public void Hook(GameObject target)
	{
		line.startColor = c_idle;
		line.endColor = c_idle;
		target.GetComponent<Hook>().Attach(this); // tell hook it's got a new cord attached
		end = target;
		grabbed = false;

		foreach (CapsuleCollider c in colliders) // reenable colliders so they detect mouse events
			c.enabled = true;
		Invoke("StopSmooth", smoothing); // give SmoothDamp time to come to rest, then stop calculating it

		AkSoundEngine.PostEvent("Hook", grabber); // hook sound effect
	}

	// to be invoked just after Cord is hooked, to give smoothing animation time to complete
	void StopSmooth()
	{
		doSmooth = false;
	}
}
