Ersilia
=======

Ersilia is a game environment with procedural sound design wherein the player interacts with an aeolian harp: a collection of strings played by the wind. It was inspired by Max Eastley's [aeolian harps](https://www.youtube.com/watch?v=yS7HU_hHwzU) and the description of the city of Ersilia in Italo Calvino's Invisible Cities. It was built in Unity, with sound design using Wwise, into which I imported Pure Data patches compiled by [Heavy](https://enzienaudio.com/) to implement the procedural parts of the sound.

#### [Video screengrab here](https://vimeo.com/256799232)

This repository contains the C# Unity scripts written for the game, mostly concerned with the manipulation of the strings. (In the code they are referred to as _Cords_ - not strings, for obvious reasons - and I will refer to them as such from now on.)

Structure
------
Cords are complex objects: an empty parent object to which are attached the _Cord.cs_ script and a _Line Renderer_ component; with 3 child objects, each having a _Capsule Collider_ and an instance of the _CordSeg.cs_ script. The _Line Renderer_ is solely responsible for rendering the cords, and is controlled in the _Cord.cs_ script. The _Capsule Colliders_ allow mouse interactions with the cord using `OnMouseEnter`, `OnMouseDown`, etc. These events depend on which portion of the cord is being manipulated - the middle, or either end - hence there are three separate _Capsule Colliders_, each with its own _Segment_ variable set to one of three values of an enumerator (defined in _Segment.cs_).

Cords are attached to _Hooks_. These can be any other gameObject with the _Hook.cs_ script attached. To initialise the positions of the cords in the scene, public `GameObject` variables for start and end are made available in the _Cord.cs_ script; hooks are assigned to each cord object by dragging the appropriate hook objects into the `start` and `end` fields in the properties panel. Since all rendering is done by script, the cords do not appear in the scene when editing, but rather only when the game is launched.

A _Wind_ object wanders around the scene simulating gusts of wind which trigger the aeolian harp sound of each cord. This is another empty gameObject with the _Wind.cs_ script attached.

Perusal
------
Look in _Cord.cs_ first. Most of the functionality is contained there.
