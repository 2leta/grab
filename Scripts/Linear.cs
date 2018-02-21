using System;
using UnityEngine;

// Static class for vector calculation functions

namespace AssemblyCSharp
{
	public static class Linear
	{
		// test if a point lies between two points that define a bounded line (such as a Cord)
		// 'point' is assumed to already lie on the infinite line projected through the points given by 'start' and 'end' arguments
		// return Mid if yes, Start if it is beyond the 'start' point, and End if beyond the 'end' point
		public static Segment WithinBounds(Vector3 start, Vector3 end, Vector3 point)
		{
			float dot = Vector3.Dot(end-start, point-start);
			// if dot-product is -ve, point lies before start of line
			if (dot < 0)
				return Segment.Start;
			// when line is between start and end, dot product is < length^2
			float sqLen = (end-start).magnitude;
			sqLen *= sqLen;
			if (dot < sqLen)
				return Segment.Mid;
			else
				return Segment.End;
		}

		// return the nearest point on a bounded line
		public static Vector3 NearPointOnLine(Vector3 start, Vector3 end, Vector3 point)
		{
			// Vector3.Project() gives us the answer for an unbounded line
			Vector3 proj = end + Vector3.Project(point-end, start-end);
			// So final step is to test if it's WithinBounds and return appropriate point
			switch (WithinBounds(start, end, proj)) {
			case Segment.Mid:
				return proj;
			case Segment.Start:
				return start;
			default:
				return end;
			}

		}
	}
}

