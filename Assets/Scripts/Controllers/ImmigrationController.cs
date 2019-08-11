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
	public List<Prole> Proles;
	public ImmigrantOrigin immigrantOrigin;

	public ImmigrationSave(ImmigrationController ic) {

        timeDelta = ic.TimeDelta;
		immigrantsWaiting = ic.immigrantsWaiting;
		immigrantsThisMonth = ic.immigrantsThisMonth;

		physicalPref = ic.physicalPref;
		intellectualPref = ic.intellectualPref;
		emotionalPref = ic.emotionalPref;

		immigrantOrigin = ic.immigrantOrigin;

	}

}

public class ImmigrationController : MonoBehaviour {

    public WorldController worldController;
	public List<Structure> Requests { get; set; }
    public float TimeDelta { get; set; }
	public int immigrantsThisMonth, immigrantsWaiting = 25, physicalPref = 60, intellectualPref = 20, emotionalPref = 20;
	public ImmigrantOrigin immigrantOrigin = ImmigrantOrigin.American;
	public string startingHouse = "House1s";

	public int TotalPref { get { return physicalPref + intellectualPref + emotionalPref; } }
	
    public float ImmigrationRate { get { return ((float)TimeController.DaysInAMonth) / immigrantsThisMonth; } }

	//immigration info
	public static string[] surnames, firstnames, skincolors;
	public static Float3d baseColor, maxColor;

	public void Load(ImmigrationSave ic) {

        TimeDelta = ic.timeDelta;
		immigrantsWaiting = ic.immigrantsWaiting;
        immigrantsThisMonth = ic.immigrantsThisMonth;

		physicalPref = ic.physicalPref;
		intellectualPref = ic.intellectualPref;
		emotionalPref = ic.emotionalPref;

		immigrantOrigin = ic.immigrantOrigin;

	}

    public void Start() {

        Requests = new List<Structure>();
		
	}

	//should take place after world creation or loading saved world, to make sure we have thr right immigrant origin
	public void LoadImmigrantData() {

		string origin = immigrantOrigin.ToString();

		//load names
		firstnames = ((TextAsset)Resources.Load("Text/Firstnames/" + origin)).text.Split(' ', '\n');
		surnames = ((TextAsset)Resources.Load("Text/Surnames/" + origin)).text.Split(' ', '\n');
		skincolors = ((TextAsset)Resources.Load("Text/Skincolors/" + origin)).text.Split(' ', '\n');

		if (skincolors.Length != 6)
			Debug.LogError("Skin color file for " + origin + " immigrants is incomplete");
		baseColor = new Float3d(float.Parse(skincolors[0]), float.Parse(skincolors[1]), float.Parse(skincolors[2]));
		maxColor = new Float3d(float.Parse(skincolors[3]), float.Parse(skincolors[4]), float.Parse(skincolors[5]));

	}

	public bool Contains(Structure s) {

        return Requests.Contains(s);

    }

    
    private void Update() {

		//if(immigrantsWaiting != 0)
		//	TimeDelta += Time.deltaTime;

  //      if (TimeDelta >= ImmigrationRate) {

  //          TimeDelta = 0;
  //          if (Requests.Count > 0)
  //              NextImmigrant();

		//}

	}

    public void NextImmigrant() {

		//only proceed if there's a map entrance
		Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();
		if (mapEntrance == null)
			Debug.LogError("Missing map entrance");

		//we're going to pass this data into SpawnImmigrant(,,)
		//	also, it may be later that we'll KNOW what immigrant is moving it, so it's good to have it separate and not call GetRandomImmigrant() as an argument itself
		Structure requester = GetRandomRequester();
		Prole immigrant = GetRandomImmigrant();
		
		//if requester still exists, send an immigrant to it from the map entrance
		if (requester != null) {
			SpawnImmigrant(requester, immigrant);
			immigrantsWaiting--;
		}

		//whether null or not, remove from list so it's not there anymore
		Requests.Remove(requester);

	}
	
	//NOTE: accent marks do not work!!!!
	public static string GetRandomSurname() {

		int rand = Random.Range(0, surnames.Length);
		return surnames[rand];

	}

	public static string GetRandomFirstName() {

		int rand = Random.Range(0, firstnames.Length);  //note: masc names are even, femme names are odd
		return firstnames[rand];

	}

	public static Float3d GetRandomSkinColor() {

		float random = Random.Range(0f, 1);

		//random colors: base color plus some % of darkening
		float r = baseColor.X + (maxColor.X - baseColor.X) * random;
		float g = baseColor.Y + (maxColor.Y - baseColor.Y) * random;
		float b = baseColor.Z + (maxColor.Z - baseColor.Z) * random;

		return new Float3d(r / 255f, g / 255f, b / 255f);

	}

	public LaborType GetRandomLaborPref() {

		int roll = Random.Range(1, TotalPref);

		if (roll <= physicalPref)
			return LaborType.Physical;

		else if (roll <= physicalPref + intellectualPref)
			return LaborType.Intellectual;

		return LaborType.Emotional;

	}

	public Prole GetRandomImmigrant() {

		Prole prospect = new Prole(true, true, GetRandomLaborPref());
		return prospect;

	}

	public Structure GetRandomRequester() {

		return Requests[Random.Range(0, Requests.Count)];

	}

	public void SpawnImmigrant(Structure requester, Prole immigrant) {
		
		//only proceed if there's a map entrance
		Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();
		if (mapEntrance == null)
			Debug.LogError("Missing map entrance");

		SpawnImmigrant(new Node(mapEntrance), requester, immigrant);

	}

	public void SpawnImmigrant(Node start, Structure requester, Prole immigrant) {
		
		//by the way: it's way easier to deal with the starting point as a node than a structure, especially because we don't need to know where the immigrant came from
		//	no matter how weird that we instantiate 'end' here but not 'start'
		
		Queue<Node> path = new Pathfinder(worldController.Map).FindPath(start, new Node(requester), "Immigrant");
		if (path.Count == 0)
			return;

		GameObject go = worldController.SpawnObject("Walkers", "Immigrant", start);

		Walker w = go.GetComponent<Walker>();
		w.world = worldController;
		w.Destination = requester;
		w.SetPath(path);
		w.SetPersonData(immigrant);       //data for immigrant moving into house
		w.Activate();

		immigrant.EvictHouse(false);  //quit current house if they have one, but don't quit work because they're still here

	}

	public SimplePriorityQueue<Structure> FindClosestHouses(Node start) {
		
		SimplePriorityQueue<Structure> queue = new SimplePriorityQueue<Structure>();

		if (Requests.Count == 0)
			return queue;

		foreach (Structure house in Requests) {

			float distance = start.DistanceTo(new Node(house));

			queue.Enqueue(house, distance);

		}

		return queue;

	}

	public void TryEmigrant(Prole emigrant) {

		//if there are other houses to move into, go to the closest one
		if (Requests.Count > 0) {

			SimplePriorityQueue<Structure> houses = FindClosestHouses(emigrant.homeNode);
			if (houses.Count == 0)
				return;
			Structure str = houses.Dequeue();
			SpawnImmigrant(emigrant.homeNode, str, emigrant);
			Requests.Remove(str);
			return;

		}

		//otherwise leave the map
		SpawnEmigrant(emigrant);

	}

	public void SpawnEmigrant(Prole emigrant) {

		SpawnEmigrant(emigrant.homeNode, emigrant);

	}

	public void SpawnEmigrant(Node start, Prole emigrant) {

		GameObject mapExit = GameObject.FindGameObjectWithTag("MapExit");
		if (mapExit == null)
			return;

		Node end = new Node(mapExit.GetComponent<Structure>());

		Queue<Node> path = new Pathfinder(worldController.Map).FindPath(start, end, "Emigrant");
		if (path.Count == 0)
			return;

		GameObject go = worldController.SpawnObject("Walkers", "Emigrant", start);
		//GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Emigrant"));
		//go.transform.position = mapExit.transform.position;
		//go.name = "Emigrant";

		Walker w = go.GetComponent<Walker>();
		w.world = worldController;
		w.Destination = mapExit.GetComponent<Structure>();
		w.SetPersonData(emigrant);
		w.SetPath(path);
		w.Activate();

		//evict house at the very end to remove homeNode and remove prole data from house
		//quit job too because they're leaving the city
		emigrant.EvictHouse(false);
		emigrant.QuitWork();
		//RemoveResidents(ExcessResidents);

	}

}
