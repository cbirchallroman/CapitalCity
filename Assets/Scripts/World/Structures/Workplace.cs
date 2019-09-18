using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkplaceSave : StructureSave {

    public int timeToSpawnWalker, access, workingDay;
    public bool activeBuilding, closedByPlayer;
    public bool[] activeSchedule;
    public List<Prole> WorkerList, WorkerSave;

    public WorkplaceSave(GameObject go) : base(go) {

        Workplace w = go.GetComponent<Workplace>();
		
        timeToSpawnWalker = w.TimeToSpawnWalker;
        access = w.Access;
        workingDay = w.WorkingDay;

        activeBuilding = w.ActiveBuilding;
        closedByPlayer = w.ClosedByPlayer;

		activeSchedule = w.ActiveSchedule;
        WorkerList = w.WorkerList;
        WorkerSave = w.WorkerSave;

    }

}


public class Workplace : Structure {

    [Header("Workplace")]
    public int timeToSpawnWalkerMax;
    public int workersMax = 1;
	public int minimumAbility = 10;
    public float baseWages = .4f;
	public LaborType laborType = LaborType.Physical;
	public string deathDesc = "died in a workplace accident.";

    public int TimeToSpawnWalker { get; set; }
    
    //public Prole[] WorkerList { get; set; }
	public List<Prole> WorkerList { get; set; }
	public List<Prole> WorkerSave { get; set; }

    public bool ActiveBuilding { get; set; }
    public bool ClosedByPlayer { get; set; }
	public bool HireRetiredProles { get; set; }

    public bool[] ActiveSchedule { get; set; }
    public int WorkingDay { get; set; } //from 1 to 16
	public int BaseWorkingDay { get { return 8; } }
	public int BaseWorkers { get { return 4; } }		//this is just how many workers a factory structure should be expected to have
														//	we won't actually need this so long as we keep 4 in mind
	public int FreeTime { get { return 16 - WorkingDay; } }

    public int Access { get; set; }

    //vars that come from other vars
    public virtual bool Operational { get { return EnoughWorkers && ActiveBuilding; } }
    public bool EnoughWorkers { get { return WorkerList.Count > 0; } }
    public float WagesOverall { get { return WagesPerWorker * WorkerList.Count; } }
    public float WagesPerWorker { get { return WorkingDay * baseWages; } }
    public float PercentEmployed { get { return (float)WorkerList.Count / workersMax; } }
	public float WorkerEffectiveness { get; set; }

	//UI thingy
	public event Action<Prole, Workplace> ProleEmploymentAction;

    public override void Load(ObjSave o) {
        base.Load(o);

        //load vars for workplaces
        WorkplaceSave w = (WorkplaceSave)o;
		
        TimeToSpawnWalker = w.timeToSpawnWalker;
        Access = w.access;
        WorkingDay = w.workingDay;
        
        ActiveBuilding = w.activeBuilding;
        ClosedByPlayer = w.closedByPlayer;

		ActiveSchedule = w.activeSchedule;

        //add workers back to laborcontroller
        ToggleLabor(ActiveBuilding);
        WorkerList = w.WorkerList;
        WorkerSave = w.WorkerSave;

		//labor.AddWorkplace(this);

	}

    public override void Activate() {
        base.Activate();
        
        TimeToSpawnWalker = timeToSpawnWalkerMax;
        ActiveSchedule = new bool[4];
        for (int a = 0; a < ActiveSchedule.Length; a++)
            ActiveSchedule[a] = true;
        ToggleLabor(true);
        WorkingDay = BaseWorkingDay;
        WorkerList = new List<Prole>();
		CalculateWorkerEffectiveness();

		//labor.AddWorkplace(this);

	}

    public override void UponDestruction() {

        base.UponDestruction();

        if (IsPreview)
            return;

        ToggleLabor(false);
		//labor.RemoveWorkplace(this);
        RemoveAllWorkers();

    }

    public override void DoEveryDay() {

        base.DoEveryDay();
        
        //spawn walker or laborseeker process
        if (!ActiveRandomWalker) {

            if (WorkerList.Count < workersMax && ActiveBuilding)
                SpawnLaborSeeker();
            else if (Operational && timeToSpawnWalkerMax != 0)
                RandomWalkerCounter();

        }

        if (Operational && radiusActive)
            VisitBuildings();
            

        //open/close labor if season changes and not closed by player
        if(!ClosedByPlayer && ActiveBuilding != ActiveSchedule[time.Seasons])
            ToggleLabor(ActiveSchedule[time.Seasons]);
    }

    public void RandomWalkerCounter() {

        TimeToSpawnWalker--;

        if (TimeToSpawnWalker <= 0) {

            SpawnRandomWalker();
            TimeToSpawnWalker = timeToSpawnWalkerMax;

        }

    }

    //ADD OVERALL LABOR STUFF
    public void ToggleLabor(bool b) {
        
        if (b && !ActiveBuilding) {
            ActiveBuilding = true;
            if(WorkerSave != null)
                RehireWorkers();
            //labor.AddLaborReq(laborDivision, workersMax);
        }
        
        else if (!b && ActiveBuilding) {
            ActiveBuilding = false;
            WorkerSave = WorkerList;
            RemoveAllWorkers();
            //labor.RemoveLaborReq(laborDivision, workersMax);
        }

        //if(labor != null)
            //labor.CalculateWorkers();
    }

    public override void OpenWindow() {

		OpenWindow(UIObjectDatabase.GetUIElement("WorkplaceWindow"));

	}

    public void SpawnLaborSeeker() {

        List<Node> entrances = GetAdjRoadTiles();

        //proceed only if there are available roads
        if (entrances.Count == 0)
            return;
		GameObject go = world.SpawnObject("Walkers/RandomWalkers", "LaborSeeker", entrances[0]);

        Walker w = go.GetComponent<Walker>();
        w.world = world;
        w.Origin = this;
        w.Activate();

    }

    public bool RemoveWorker(Prole p) {

		if (!WorkerList.Contains(p))
			Debug.LogError("Removing " + p + " who doesn't work at " + this);

		WorkerList.Remove(p);
		CalculateWorkerEffectiveness();
			
		return true;

    }

    public void RemoveAllWorkers() {

        for (int i = WorkerList.Count - 1; i >= 0; i--)
            RemoveWorker(WorkerList[i]);

    }

    public void RehireWorkers() {

        foreach(Prole p in WorkerSave) {

            //if null spot, continue
            if (p == null)
                continue;

			//if prole should be dead, remove from array and continue
			if (p.markedForDeath)
				continue;

            //if no longer existing, continue
            if (!p.CheckExistence())
                continue;

            //only employ if seeking work
            if (p.SeekingWork)
				AddWorker(p);


		}

    }

    public bool AddWorker(Prole p) {

        if (WorkerList.Count >= workersMax)
            return false;

		population.EmployProle(p);
		WorkerList.Add(p);
		p.JoinWork(this);

		CalculateWorkerEffectiveness();

		//update UI if possible
		Action<Prole, Workplace> act = ProleEmploymentAction;
		if(act != null)
			act.Invoke(p, this);

		return true;

    }

    public override void DoEveryMonth() {

        base.DoEveryMonth();

        PayWagesAndSpreadDisease();

    }

	//since we're iterating, might as well do both things at once
    void PayWagesAndSpreadDisease() {

		int numDiseased = 0;

        foreach(Prole p in WorkerList) {

            if (p == null)
                continue;

			if (p.diseased)
				numDiseased++;	//add to sum of diseased workers

			p.PayWages(WagesPerWorker); //pay wages
			money.SpendOnWages(WagesOverall);	//record this

		}
		
		if (numDiseased == 0 || numDiseased == WorkerList.Count)		//don't continue if chance is 0% or if all workers here are diseased
			return;

		int diseaseChance = numDiseased * 5;	//the chance for disease to spread

		//on a successful roll, spread disease to one other person here
		if (UnityEngine.Random.Range(1, 100) <= diseaseChance) {	//had to specify UnityEngine bc that and System are both here

			foreach (Prole p in WorkerList) {

				if (p == null)
					continue;

				if (p.diseased)		//don't turn diseased if already diseased
					continue;

				p.TurnDiseased();
				break;		//after finding someone to disease, break


			}

		}

    }

	void CalculateWorkerEffectiveness() {

		float sum = 0;
		foreach (Prole w in WorkerList) {

			if (w != null)
				sum += w.GetWorkerEffectiveness(laborType);

		}

		WorkerEffectiveness = sum / workersMax;

	}

}
