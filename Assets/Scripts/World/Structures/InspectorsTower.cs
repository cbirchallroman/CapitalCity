using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorsTower : Workplace {

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (!ActiveSmartWalker && Operational)
            SearchForFire();

    }

    void SearchForFire() {

		List<Node> entrances = GetAdjRoadTiles();
		if (entrances.Count == 0)
			return;
		Node start = entrances[0];

		SimplePriorityQueue<Structure, float> queue = FindClosestStructureOfType("Fire");
		
		for (int i = 0; queue.Count > 0 && i < 5 && !ActiveSmartWalker; i++) {

			Structure s = queue.Dequeue();
			Node end = new Node(s);

			Queue<Node> path = pathfinder.FindPath(start, end, "Fireman");
			if (path.Count == 0)
				continue;

			GameObject go = world.SpawnObject("Walkers", "Fireman", start);

			Walker c = go.GetComponent<Walker>();
			c.world = world;
			c.Origin = this;
			c.Destination = s;
			c.Activate();
			c.SetPath(path);

		}

    }

}
