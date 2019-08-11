using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JobcentreSave : StructureSave {

	public List<Prole> Prospects;

	public JobcentreSave(GameObject go) : base(go) {

		Jobcentre j = go.GetComponent<Jobcentre>();

		Prospects = j.Prospects;

	}

}

public class Jobcentre : Structure {

	[Header("Jobcenter")]
	public int maxProspects = 4;
	public List<Prole> Prospects { get; set; }
	public int[] ProspectCounters { get; set; }
	public int ProspectsExpecting { get; set; }

	public bool HireHighPhy { get; set; }
	public bool HireHighInt { get; set; }
	public bool HireHighEmo { get; set; }
	public bool Active { get { return HireHighEmo || HireHighInt || HireHighPhy; } }

	public override void Load(ObjSave o) {

		base.Load(o);
		JobcentreSave j = (JobcentreSave)o;

		Prospects = j.Prospects;

	}

	public override void Activate() {

		base.Activate();
		Prospects = new List<Prole>();
		ProspectCounters = new int[maxProspects];
		HireHighPhy = true;
		HireHighInt = true;
		HireHighEmo = true;

	}

	public override void DoEveryDay() {

		base.DoEveryDay();

		if (!Active) {

			if (Prospects.Count > 0)
				SendAwayAllProspects();	//if there's still people here, send them away
			return;

		}	//only proceed if the jobcentre is active

		SendOutProspects();

	}

	public override void DoEveryWeek() {

		base.DoEveryWeek();

		GetAnotherProspect();

	}

	void SendOutProspects() {
		
		//iterate backwards to avoid problems from removing prospects
		for (int i = Prospects.Count - 1; i >= 0; i--) {

			Prole prospect = Prospects[i];

			if (prospect.rejected)  //send prospect out of it is rejected or now too old to work or has waited for too long (regardless of hire status)
				SendAwayProspect(prospect);
			else if (prospect.accepted)
				HireProspect(prospect);

			prospect.UpdateAge();   //here we can have the prospect age and also countdown to leaving if they waited for too long

		}

	}

	void GetAnotherProspect() {

		if (!Active)
			return;

		//if there's any space for prospects left and if there's any empty houses
		if (Prospects.Count + ProspectsExpecting < maxProspects && immigration.Requests.Count > 0) {

			Prole prospect = immigration.GetRandomImmigrant();

			LaborType prospectPref = prospect.HighestValue();
			if (prospectPref == LaborType.Physical && !HireHighPhy)
				return;
			if (prospectPref == LaborType.Intellectual && !HireHighInt)
				return;
			if (prospectPref == LaborType.Emotional && !HireHighEmo)
				return;

			//get immigrant to this building from outside
			immigration.SpawnImmigrant(this, prospect);

			//add prospect to queue
			ProspectsExpecting++;

		}

	}

	public override void ReceiveImmigrant(Prole prospect) {

		prospect.StartWaitCountdown();
		Prospects.Add(prospect);
		ProspectsExpecting--;

		//if we traverse the whole array without there being an empty space
		if (Prospects.Count > maxProspects)
			Debug.LogError(name + " received an immigrant when it didn't need one");

	}

	public void AcceptProspect(Prole prospect) {

		prospect.accepted = true;
		prospect.rejected = false;

	}

	public void RejectProspect(Prole prospect) {

		prospect.accepted = false;
		prospect.rejected = true;

	}

	void HireProspect(Prole prospect) {

		if (immigration.Requests.Count == 0)
			return;     //if there's no house looking for a resident, don't keep looking

		//get start node, an adjacent road tile
		Node start = new Node(this);

		//get closest requester from immigration controller
		//	we know we'll get at least one result bc we already checked that there's more than zero houses in line
		SimplePriorityQueue<Structure> houses = immigration.FindClosestHouses(start);
		if (houses.Count == 0)
			return;
		Structure requester = houses.Dequeue();
		prospect.waitCountdown = 0;

		//send immigrant from here to that house
		immigration.SpawnImmigrant(start, requester, prospect);
		immigration.Requests.Remove(requester);	//don't forget to remove requester from list

		//remove prospect from array
		RemoveProspectFromArray(prospect);

		return;

	}

	void SendAwayProspect(Prole prospect) {

		//get start node, an adjacent road tile
		List<Node> entrances = GetAdjRoadTiles();
		if (entrances.Count == 0)
			return;
		Node start = entrances[0];

		//send emigrant from here
		immigration.SpawnEmigrant(start, prospect);

		//remove prospect from array
		RemoveProspectFromArray(prospect);

	}

	void SendAwayAllProspects() {

		for (int i = Prospects.Count - 1; i >= 0; i--)
			SendAwayProspect(Prospects[i]);

	}

	void RemoveProspectFromArray(Prole prospect) {

		Prospects.Remove(prospect);

	}

	public override void OpenWindow() {

		OpenWindow(UIObjectDatabase.GetUIElement("JobcentreWindow"));

	}

}
