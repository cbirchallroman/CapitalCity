using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder {

    public World map;

    public Pathfinder(World w) {

        map = w;

    }

	public Queue<Node> FindPath(Node start, Node end, string name) {

		WalkerData walker = WalkerDatabase.GetData(name);
		return FindPath(start, end, walker);

	}

	public Queue<Node> FindPath(Node start, List<Node> ends, string name) {
		
		WalkerData walker = WalkerDatabase.GetData(name);
		return FindPath(start, ends, walker);

	}

	public Queue<Node> FindPath(Node start, Node end, WalkerData walker) {

		List<Node> ends = new List<Node>();
		ends.Add(end);

		return FindPath(start, ends, walker);

	}

	public Queue<Node> FindPath(Node start, List<Node> ends, WalkerData walker) {

		Queue<Node> path = new Queue<Node>();

		if (ends.Count == 0)
			return path;

		Dictionary<Node, float> G_scores = new Dictionary<Node, float>();
		Dictionary<Node, Node> nexts = new Dictionary<Node, Node>();
		SimplePriorityQueue<Node> queue = new SimplePriorityQueue<Node>();

		Node goal = start;
		Node current = ends[0];

		//add each possible exit into the open set with a 0 g score
		foreach(Node n in ends) {
			current = n;
			G_scores[current] = 0;
			nexts[current] = null;
			queue.Enqueue(current, current.DistanceTo(goal, true));
		}

		//create path of nodes and array of visited nodes
		bool[,] visited = map.size.CreateArrayOfSize<bool>();

		//while there are unvisited nodes in the queue
		while (queue.Count != 0) {

			//pop from queue
			current = queue.Dequeue();

			//if current is goal, return the path
			if (current.Equals(goal)) {

				for (Node c = current; c != null; c = nexts[c])
					path.Enqueue(c);

				path.Dequeue();
				break;

			}

			//mark node as visited
			visited[current.x, current.y] = true;

			//check neighbors
			foreach (Node neighbor in Neighbors(current, walker)) {

				//if already visited, do not proceed
				if (visited[neighbor.x, neighbor.y])
					continue;

				//g is equal to node's gScore plus neighbor's tile cost (Shortest distance from start)
				//h is estimated distance from neighbor to goal (heuristic)
				//f is g + h (priority)
				float g = G_scores[current] + map.TileCost(neighbor);
				float h = neighbor.DistanceTo(goal, true);
				float f = g + h;

				//add sqrt(2) if neighbor is diagonal to current
				if (neighbor.IsDiagonalTo(current))
					g += Node.sqrt2 - 1;

				//if open contains neighbor, only proceed if new gScore is shorter than current
				if (queue.Contains(neighbor)) {

					//if old distance is shorter than new, do not proceed
					if (g > G_scores[neighbor])
						continue;

					//else remove from queue to be replaced by the new instance
					else
						queue.Remove(neighbor);
				}

				G_scores[neighbor] = g;
				nexts[neighbor] = current;
				queue.Enqueue(neighbor, f);

			}
		}

		return path;

	}

	public List<Node> Neighbors(Node current, WalkerData walker) {
        int currx = current.x;
        int curry = current.y;

        List<Node> neighbors = new List<Node>();

		//look at adjacent neighbors
		for (int dx = -1; dx <= 1; dx++) {
			for (int dy = -1; dy <= 1; dy++) {

				//don't do current tile
				if (dx == 0 && dy == 0)
					continue;

				bool diagonal = dx != 0 && dy != 0;

				//don't do diagonal tiles if not traversable
				if (diagonal && !walker.CanGoDiagonal)
					continue;

				int a = currx + dx;
				int b = curry + dy;

				//don't do diagonal if can't pass through one of the tiles adjacent
				if (diagonal && (!CanGoTo(a, curry, currx, curry, walker) || !CanGoTo(currx, b, currx, curry, walker)))
					continue;


				if (CanGoTo(a, b, currx, curry, walker))
					neighbors.Add(new Node(a, b));
			}
		}

        return neighbors;
    }

    public bool CanGoTo(int a, int b, int currx, int curry, WalkerData walker) {

		//if out of bounds, can't go
		if (map.OutOfBounds(a, b))
			return false;
		
		//if road-only walker, check for roadblocks
		if (walker.RoadWalker)
			return RoadWalkerCanGoThrough(a, b, currx, curry, walker);
		
		//if water-only walker, check if on water
		else if (walker.WaterWalker)
			return WaterWalkerCanGoThrough(a, b, walker);

		//otherwise check if there is a building
		else if (map.IsBuildingAt(a, b))
			return CheckBuildingAt(a, b, walker);

		//return false if there's water
		return map.terrain[a, b] != (int)Terrain.Water;

	}

	bool RoadWalkerCanGoThrough(int a, int b, int currx, int curry, WalkerData walker) {

		if (map.IsRoadblockAt(a, b)) {

			//if this is a random walker, check if the roadblock will let them in
			if (walker.RandomWalker && !walker.ReturningHome)
				return map.GetBuildingAt(a, b).GetComponent<Roadblock>().AllowWalkerIn(walker);

			//otherwise let them pass
			return true;

		}

		//do not go through if there is a map entrance; we won't count these for roadwalker roads
		string target = map.GetBuildingNameAt(a, b);
		string current = map.GetBuildingNameAt(currx, curry);
		if (!string.IsNullOrEmpty(target)) {

			if (target.Contains("MapEntrance"))
				return false;

			//check whether we're about to go onto a ramp
			else if(target.Contains("Ramp"))
				return map.GetBuildingAt(a, b).GetComponent<Road>().NeighborCondition(currx, curry);

		}
		if (!string.IsNullOrEmpty(current)) {	//not "else if" because this isn't mutually exclusive

			//check whether we're on a ramp right now
			if(current.Contains("Ramp"))
				return map.GetBuildingAt(currx, curry).GetComponent<Road>().NeighborCondition(a, b);

		}

		return map.IsUnblockedRoadAt(a, b);

	}

	bool WaterWalkerCanGoThrough(int a, int b, WalkerData walker) {
		
		//bridges will count as roadblocks
		//	they will allow all non-water walkers in
		//	and allow water walkers in depending on height
		if (map.IsBuildingAt(a, b) && !map.IsRoadAt(a, b))
			return false;

		return map.terrain[a, b] == (int)Terrain.Water;

	}

	bool CheckBuildingAt(int a, int b, WalkerData walker) {

		string str = map.GetBuildingNameAt(a, b);

		if (str.Contains("MapEntrance"))
			return true;

		if (map.IsRoadAt(a, b))
			return true;

		string exception = walker.CanPassThrough;
		if (!string.IsNullOrEmpty(exception))
			if (str.Contains(exception))
				return true;

		return false;

	}

}
