using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic : RandomWalker {

	public override void VisitBuilding(int a, int b) {

		base.VisitBuilding(a, b);

		Distributer d = (Distributer)Origin;
		Generator g = world.Map.GetBuildingAt(a, b).GetComponent<Generator>();
		if (g == null)
			return;

		if (g.MachineryResource.ToString() != d.item)
			return;

		int needed = g.RepairsNeeded;
		if (d.AmountStored < needed)
			needed = (int)d.AmountStored;

		ItemOrder io = new ItemOrder(needed, d.item);

		if (g.MachineryResource != (ResourceType)io.item)
			return;

		g.MaintainFactory(io);
		d.AmountStored -= needed;

	}

}
