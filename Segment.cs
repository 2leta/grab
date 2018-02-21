/****************
Enumerator for the three different possible segments of a cord that CordSeg objects can represent
*****************/

using System;

namespace AssemblyCSharp
{
	public enum Segment
	{
		Start = -1, Mid = 0, End = 1 // values allow negation of Segment.Start to derive Segment.End
	}
}

