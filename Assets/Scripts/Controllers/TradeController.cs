using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

[System.Serializable]
public class TradeSave {

    public List<ItemOrder> tradeOrders;
    public float timeDelta;
	public int caravanIndex;


	public TradeSave(TradeController tc) {

        tradeOrders = tc.TradeOrders;
        timeDelta = tc.TimeDelta;
		caravanIndex = tc.CaravanIndex;


	}

}

public class TradeController : MonoBehaviour {

    //public Dictionary<ItemOrder, bool> TradeOrders { get; set; }
	public List<ItemOrder> TradeOrders { get; set; }
    public WorldController worldController;
	
	public int CaravanIndex { get; set; }
	public float CaravanRate { get { if (TradeOrders.Count == 0) return 0; return (TimeController.DaysInAMonth) / TradeOrders.Count; } }
    public float TimeDelta { get; set; }

    public void Load(TradeSave tc) {

        TradeOrders = tc.tradeOrders;
        TimeDelta = tc.timeDelta;
		CaravanIndex = tc.caravanIndex;

	}

	// Use this for initialization
	void Start () {

        TradeOrders = new List<ItemOrder>();

    }

    private void Update() {

        if (TradeOrders.Count == 0)
            return;

        TimeDelta += Time.deltaTime;

        if (TimeDelta >= CaravanRate && CaravanRate != 0) {

            TimeDelta = 0;
            NextCaravan();

        }

    }

    public void NextCaravan() {

		int currentMonth = (int)worldController.timeController.CurrentMonth;

		if (TradeOrders.Count > CaravanIndex) {

			ItemOrder deal = TradeOrders[CaravanIndex];
			if (deal.monthsActive[currentMonth])
				SpawnCaravan(deal);

			CaravanIndex++;
			if (TradeOrders.Count == CaravanIndex)
				CaravanIndex = 0;
		}
		else
			CaravanIndex = 0;

    }

	public void SpawnCaravan(ItemOrder co) {
		
		bool import = co.direction == TradeDirection.Import;

		//check if map entrance
		Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();
		if (mapEntrance == null)
			Debug.LogError("No map entrance for caravan found");

		//make queue of storage buildings
		SimplePriorityQueue<StorageBuilding> queue = import ? 
			mapEntrance.FindStorageBuildingToAccept(co) : 
			mapEntrance.FindStorageBuildingThatHas(co);

		for (int i = 0; queue.Count > 0 && i < 5; i++) {

			Node start = new Node(mapEntrance);

			//pop storage building, make sure it has an exit
			Structure strg = queue.Dequeue();
			List<Node> exits = strg.GetAdjRoadTiles();
			if (exits.Count == 0)
				continue;

			//find a path, continue only if it exists
			string caravanName = import ? "Importer" : "Exporter";
			Queue<Node> path = new Pathfinder(worldController.Map).FindPath(start, exits, caravanName);
			if (path.Count == 0)
				continue;

			GameObject go = worldController.SpawnObject("Walkers", caravanName, start);

			Walker w = go.GetComponent<Walker>();
			w.Order = co;
			w.world = worldController;
			w.Origin = mapEntrance;
			w.Destination = strg;
			w.SetPath(path);
			w.Activate();
			break;

		}

		//List<Node> entrances = target.GetAdjRoadTiles();
		//if (entrances.Count == 0)
		//	return;

		//Node start = new Node(mapEntrance.X, mapEntrance.Y);
		//Node end = entrances[0];
		//if(tradeDirection == TradeDirection.Import)
		//	target.GetComponent<StorageBuilding>().Queue[co.item] += co.amount;

		//string caravanName = tradeDirection == TradeDirection.Import ? "Importer" : "Exporter";
		//Stack<Node> path = worldController.Map.pathfinder.FindPath(start, end, WalkerDatabase.GetData(caravanName));
		//if (path.Count == 0)
		//	return;

		//GameObject go = worldController.SpawnObject("Walkers", caravanName, start);

		//Walker w = go.GetComponent<Walker>();
		//w.Order = co;
		//w.world = worldController;
		//w.Origin = mapEntrance;
		//w.Destination = target;
		//w.Path = path;
		//w.Activate();

	}

    public bool ContainsDeal(ItemOrder io) {

        foreach (ItemOrder deal in TradeOrders)
			if(deal.item == io.item && deal.type == io.type && deal.city == io.city && deal.direction == io.direction)
                return true;
        return false;

    }

	public void OpenDeal(ItemOrder io) {

		if (ContainsDeal(io))
			Debug.LogError(io + " is already open");
		TradeOrders.Add(io);

	}

    public void PrintOrders() {

        Debug.Log("PRINTING " + TradeOrders.Count + " TRADE DEALS");
        foreach (ItemOrder io in TradeOrders)
            Debug.Log(io);

    }

}
