/****************
To be added to objects to which cords can be hooked
*****************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class Hook : MonoBehaviour {

	List<Cord> attached = new List<Cord>(); // lists all Cords attached to this Hook
	GameObject grabber;

	void Start()
	{
		grabber = GameObject.FindWithTag("grabber"); // will be used for calculating player distance
	}

	// manage attached cords
	void Attach(Cord c)
	{
		attached.Add(c);
	}

	void Detach(Cord c)
	{
		attached.Remove(c);
	}

	// Click on this object to hook a cord to it, if one is being held
	void OnMouseDown()
	{ 	
		// test if a cord is being held, if player is close enough to interact,
	  	// and that hooking the Cord here will be 'legal' (see ConflictTest() below)
		if (Cord.grabbed && Vector3.Distance(grabber.transform.position,transform.position) < Cord.dist && !ConflictTest()) {
			// Cord.inHand is a static variable logging which instance of Cord is currently being held
			Cord.inHand.Hook(gameObject);
		}
	}
	
	// 
	void OnMouseEnter()
	{
		if (Cord.grabbed && Vector3.Distance(grabber.transform.position,transform.position) < Cord.dist && !ConflictTest()) {
			Cord.inHand.MouseOver(Segment.End);
		}
	}

	void OnMouseExit()
	{ // not testing distance here because otherwise it's possible to 'freeze' the hover colour of the Cord by backing away
		if (Cord.grabbed && !ConflictTest()) {
			Cord.inHand.MouseOff(Segment.End);
		}
	}

	// prevent 'disappearing' cords:
	// you don't want to hook a Cord here if another Cord follows the same path - they will merge - trouble!
	// the test also forbids the case of hooking both ends of a Cord to the same place
	bool ConflictTest()
	{
		// this linear search is performed frequently and could be improved
		// (perhaps with a Hashset or by memoizing)
		// however, in current use the List is seldom more than 3 elements, so it's not instrumental
		foreach (Cord c in attached) {
			// check if its other end is on the same hook as the hooked end of the Cord that the player is holding
			if (c.start == Cord.inHand.start || c.end == Cord.inHand.start) {
				return true;
			}
		}
		return false;
	}
}
