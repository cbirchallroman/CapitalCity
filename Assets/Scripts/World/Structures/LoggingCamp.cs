using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingCamp : Generator {

    public override void BeginProduction() {

        if (!ActiveSmartWalker && Operational)
            SearchForTree();

    }

    public override void ProductionTimer() {

        if (ProductionComplete && !ActiveSmartWalker)
            ExportProduct();

    }

    void SearchForTree() {

		List<Node> entrances = GetAdjRoadTiles();
		if (entrances.Count == 0)
			return;
		Node start = entrances[0];

		SimplePriorityQueue<Structure, float> queue = FindClosestStructureOfType("Tree");

		for (int i = 0; queue.Count > 0 && i < 5 && !ActiveSmartWalker; i++) {

			Structure s = queue.Dequeue();
			Node end = new Node(s);

			Queue<Node> path = pathfinder.FindPath(start, end, "Lumberjack");
			if (path.Count == 0)
				continue;

			GameObject go = world.SpawnObject("Walkers", "Lumberjack", start);

			Walker c = go.GetComponent<Walker>();
			c.world = world;
			c.Origin = this;
			c.Destination = s;
			c.Activate();
			c.SetPath(path);

		}

	}
}
