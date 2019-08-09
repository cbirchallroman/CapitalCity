using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JobcentreSave : StructureSave {

	public List<Prospect> Prospects;

	public JobcentreSave(GameObject go) : base(go) {

		Jobcentre j = go.GetComponent<Jobcentre>();

		Prospects = j.Prospects;

	}

}

public class Jobcentre : Structure {

	[Header("Jobcenter")]
	public int maxProspects = 4;
	public List<Prospect> Prospects { get; set; }
	public int[] ProspectCounters { get; set; }
	public int ProspectsExpecting { get; set; }
	public int ProspectWaitTime { get { return TimeController.DaysInASeason * 3; } }

	public override void Load(ObjSave o) {

		base.Load(o);
		JobcentreSave j = (JobcentreSave)o;

		Prospects = j.Prospects;

	}

	public override void Activate() {

		base.Activate();
		Prospects = new List<Prospect>();
		ProspectCounters = new int[maxProspects];

	}

	public override void DoEveryDay() {

		base.DoEveryDay();

		//TEMPORARY TEST FOR MAKING SURE THAT PROSPECTS WILL GO FROM HERE TO A NEW HOUSE IF THERE IS A HOUSE TO GO TO
		//if we have any prospects waiting for a job
		
		for(int i = Prospects.Count - 1; i >= 0; i--) {

			Prospect prospect = Prospects[i];

			if (prospect.rejected)  //send prospect out of it is rejected or now too old to work or has waited for too long (regardless of hire status)
				RejectProspect(prospect);
			else if (prospect.hired)
				HireProspect(prospect);

			prospect.UpdateAge();   //here we can have the prospect age and also countdown to leaving if they waited for too long

		}

	}

	public override void DoEveryWeek() {
		
		//if there's any space for prospects left
		if(Prospects.Count + ProspectsExpecting < maxProspects) {

			Prole prospect = immigration.GetRandomImmigrant();

			//get immigrant to this building from outside
			immigration.SpawnImmigrant(this, prospect);

			//add prospect to queue
			ProspectsExpecting++;

		}
		
	}

	public override void ReceiveImmigrant(Prole immigrant) {

		Prospect prospect = new Prospect(immigrant, immigrant.laborPref);
		prospect.hired = true;	//JUST FOR NOW, WE'RE GOING TO ASSUME THAT THEY'RE GOING TO ALWAYS BE HIRED UNLESS TOO OLD; add player control later
		Prospects.Add(prospect);
		ProspectsExpecting--;

		//if we traverse the whole array without there being an empty space
		if (Prospects.Count > maxProspects)
			Debug.LogError(name + " received an immigrant when it didn't need one");

	}

	void HireProspect(Prospect prospect) {

		if (immigration.Requests.Count == 0)
			return;     //if there's no house looking for a resident, don't keep looking

		//get start node, an adjacent road tile
		List<Node> entrances = GetAdjRoadTiles();
		if (entrances.Count == 0)
			return;
		Node start = entrances[0];

		//get closest requester from immigration controller
		//	we know we'll get at least one result bc we already checked that there's more than zero houses in line
		Structure requester = immigration.FindClosestHouses(start).Dequeue();

		//send immigrant from here to that house
		immigration.SpawnImmigrant(start, requester, prospect);
		immigration.Requests.Remove(requester);	//don't forget to remove requester from list

		//remove prospect from array
		RemoveProspectFromArray(prospect);

		return;

	}

	void RejectProspect(Prospect prospect) {

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

	void RemoveProspectFromArray(Prospect prospect) {

		Prospects.Remove(prospect);

	}

}
