using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class ImmigrationSave {

    public float timeDelta;
    public int immigrantsThisMonth, immigrantsWaiting, physicalPref, intellectualPref, emotionalPref;
	public List<Adult> Proles;


	public ImmigrationSave(ImmigrationController ic) {

        timeDelta = ic.TimeDelta;
		immigrantsWaiting = ic.immigrantsWaiting;
		immigrantsThisMonth = ic.immigrantsThisMonth;

		physicalPref = ic.physicalPref;
		intellectualPref = ic.intellectualPref;
		emotionalPref = ic.emotionalPref;

	}

}

public class ImmigrationController : MonoBehaviour {

    public WorldController worldController;
	public List<Structure> Requests { get; set; }
    public float TimeDelta { get; set; }
	public int immigrantsThisMonth, immigrantsWaiting = 25, physicalPref = 60, intellectualPref = 20, emotionalPref = 20;

	public int TotalPref { get { return physicalPref + intellectualPref + emotionalPref; } }
	
    public float ImmigrationRate { get { return (TimeController.WeekTime * TimeController.MonthTime) / immigrantsThisMonth; } }

    public void Load(ImmigrationSave ic) {

        TimeDelta = ic.timeDelta;
		immigrantsWaiting = ic.immigrantsWaiting;
        immigrantsThisMonth = ic.immigrantsThisMonth;

		physicalPref = ic.physicalPref;
		intellectualPref = ic.intellectualPref;
		emotionalPref = ic.emotionalPref;

	}

    public void Awake() {

        Requests = new List<Structure>();

	}

	public bool Contains(Structure s) {

        return Requests.Contains(s);

    }

    
    private void Update() {

		if(immigrantsWaiting != 0)
			TimeDelta += Time.deltaTime;

        if (TimeDelta >= ImmigrationRate) {

            TimeDelta = 0;
            if (Requests.Count > 0)
                NextImmigrant();

		}

	}

    public void NextImmigrant() {

        Structure s = Requests[Random.Range(0, Requests.Count)];
        if(s != null)
            SpawnFreshImmigrant(s);
        Requests.Remove(s);

	}

	public LaborType GetRandomLaborPrefFromOutsideWorld() {

		int roll = Random.Range(1, TotalPref);

		if (roll <= physicalPref)
			return LaborType.Physical;

		else if (roll <= physicalPref + intellectualPref)
			return LaborType.Intellectual;

		return LaborType.Emotional;

	}

    public void SpawnFreshImmigrant(Structure requester) {

        Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();

        if (mapEntrance == null)
            return;

		//CREATE NEW IMMIGRANT WITH RANDOM LABOR PREF
		Adult newInTown = new Adult(true, true, GetRandomLaborPrefFromOutsideWorld());
		SpawnImmigrant(new Node(mapEntrance), requester, newInTown);
		immigrantsWaiting--;

	}

	public void SpawnImmigrant(Node start, Structure house, Adult immigrant) {
		
		Node end = new Node(house);

		Queue<Node> path = new Pathfinder(worldController.Map).FindPath(start, end, "Immigrant");
		if (path.Count == 0)
			return;

		GameObject go = worldController.SpawnObject("Walkers", "Immigrant", start);

		Walker w = go.GetComponent<Walker>();
		w.world = worldController;
		w.Destination = house;
		w.SetPath(path);
		w.SetPersonData(immigrant);       //data for immigrant moving into house
		w.Activate();

		immigrant.EvictHouse(false);  //quit current house if they have one, but don't quit work because they're still here

	}

	public void TryEmigrant(Adult emigrant) {

		//GameObject go = worldController.Map.GetBuildingAt(emigrant.homeNode);
		//if(go != null) {

		//	House h = go.GetComponent<House>();
		//	h.ActiveSmartWalker = true;

		//}

		//if there are other houses to move into, go to the closest one
		if (Requests.Count > 0) {

			SimplePriorityQueue<Structure, float> houses = new SimplePriorityQueue<Structure, float>();

			int tries = 15;		//number of houses total we'll enqueue
			foreach (Structure s in Requests) {
				if (s == null) {
					Requests.Remove(s);
					continue;
				}

				houses.Enqueue(s, emigrant.homeNode.DistanceTo(new Node(s)));
				tries--;
				if (tries <= 0)
					break;
			}

			Structure str = houses.Dequeue();
			SpawnImmigrant(emigrant.homeNode, str, emigrant);
			Requests.Remove(str);
			return;

		}

		//otherwise leave the map
		SpawnEmigrant(emigrant);

		//evict house at the very end to remove homeNode and remove prole data from house
		//quit job too because they're leaving the city

	}

	public void SpawnEmigrant(Adult emigrant) {

		GameObject mapExit = GameObject.FindGameObjectWithTag("MapExit");

		if (mapExit == null)
			return;
		
		Node end = new Node(mapExit.GetComponent<Structure>());

		Queue<Node> path = new Pathfinder(worldController.Map).FindPath(emigrant.homeNode, end, "Emigrant");
		if (path.Count == 0)
			return;

		GameObject go = worldController.SpawnObject("Walkers", "Emigrant", emigrant.homeNode);
		//GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Emigrant"));
		//go.transform.position = mapExit.transform.position;
		//go.name = "Emigrant";

		Walker w = go.GetComponent<Walker>();
		w.world = worldController;
		w.Destination = mapExit.GetComponent<Structure>();
		w.SetPersonData(emigrant);
		w.SetPath(path);
		w.Activate();

		emigrant.EvictHouse(false);
		emigrant.QuitWork();
		//RemoveResidents(ExcessResidents);

	}

}
