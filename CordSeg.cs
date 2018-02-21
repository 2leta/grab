/****************
To be added to each of 3 child objects of the cord object
Each one has a CapsuleCollider that covers a different segment of the cord: the middle, and each end
*****************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class CordSeg : MonoBehaviour {

	const float distanceLim = 1.2f; // distance at which player can interact with cords
	Cord cord; // parent cord script
	public Segment mySeg; // to be set in properties panel: which part of the cord does the collider represent?
	static GameObject grabber; // reference for object that holds cord in front of camera

	// the endpoints of the capsule
	Vector3 start;
	Vector3 end;

	// Use this for initialization
	void Start()
	{
		cord = gameObject.GetComponentInParent<Cord>();
		grabber = GameObject.FindWithTag("grabber");
	}

	void OnMouseEnter() 
	{
		// only allow action when close enough - test for player distance from this segment of cord
		if (!Cord.grabbed && DistanceFromGrabber() < distanceLim)
			cord.MouseOver(mySeg);
	}

	void OnMouseExit() 
	{
			if (!Cord.grabbed)
				cord.MouseOff(mySeg);
	}

	void OnMouseDown() 
	{
		if (!Cord.grabbed && DistanceFromGrabber() < distanceLim) { 
			if (mySeg == Segment.Mid)
				cord.Pluck();
			else
				cord.Grab(mySeg);
		}
	}

	// measure distance of this segment of cord from the player
	float DistanceFromGrabber()
	{
		Vector3 pos = grabber.transform.position;
		// calculate the point on centre line of the capsule nearest to the player
		Vector3 near = Brainiac.NearPointOnLine(start, end, pos);
		return Vector3.Distance(near, pos);
	}

	// set start and end points, used to define centre line of capsule
	public void KnowBounds(CapsuleCollider me)
	{
		start = me.bounds.max;
		end = me.bounds.min;
	}
}
