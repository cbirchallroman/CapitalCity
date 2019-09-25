using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[System.Serializable]
public class Person {

	public string surname, name, ID;
	public int yearsOld, deltaDays, diseaseLength;
	public bool diseased, markedForDeath;
	public Float3d skinColor;
	public World world;

	public readonly int ageInterval = TimeController.DaysInAYear;
	public readonly int subInterval = TimeController.DaysInAMonth;
	public readonly int comingOfAge = 16;
	public readonly int retirementAge = 65;
	public readonly int diseaseLengthMax = (int)(6.0 * Difficulty.GetModifier());
	
	public virtual int DeathChance { get { return diseased ? DeathChanceFromDisease : 0; } }
	public virtual int DeathChanceFromDisease { get { return 5; } }

	public string FullName { get { return name + " " + surname; } }

	public Person() : this(false) { }

	public Person(bool randomAge) {

		deltaDays = randomAge? Random.Range(0, ageInterval) : 0;

		surname = ImmigrationController.GetRandomSurname();
		name = ImmigrationController.GetRandomFirstName();
		skinColor = ImmigrationController.GetRandomSkinColor();
		ID = surname + name + Random.Range(0, 100);

	}

	public virtual void UpdateAge() {

		deltaDays++;
		if (deltaDays % subInterval == 0)	//basically, every month
			EveryMonth();

		if(deltaDays == ageInterval) {		//basically, every year

			deltaDays = 0;
			yearsOld++;
			EveryBirthday();
			//Debug.Log("Happy birthday to " + this + "!");

		}

	}

	public virtual void EveryMonth() {

		//check for chance of death
		if (DeathChance > 0) {
			
			if (Random.Range(1, 100) <= DeathChance)
				MarkForDeath();

		}

		//decrease disease duration
		if (diseased) {

			diseaseLength--;
			if (diseaseLength == 0)
				CureOfDisease();

		}

	}

	public virtual void EveryBirthday() {
		

	}

	public void MarkForDeath() {

		markedForDeath = true;
		Debug.Log(this + " " + GetCauseOfDeath());

	}

	public override string ToString() {

		return FullName;

	}

	public virtual string GetCauseOfDeath() {

		return "died of drinking dirty water.";

	}

	public void TurnDiseased() {

		diseased = true;
		diseaseLength = diseaseLengthMax;
		//Debug.Log(this + " turned diseased");

	}

	public void CureOfDisease() {

		diseased = false;
		Debug.Log(this + " cured of disease");

	}

}

[System.Serializable]
public class Child : Person {
	
	public bool GrownUp { get { return yearsOld >= comingOfAge; } }
	
	public override int DeathChanceFromDisease { get { return 16 - yearsOld + base.DeathChanceFromDisease; } }

	public Child(bool randomAge, Prole parent) : base(randomAge) {

		//if parent already has children, we want to be younger than the most recent child; otherwise we can be anywhere from 0 to 15 years old
		yearsOld = randomAge ? Random.Range(0, (parent.children.Count > 0 ? parent.children[parent.children.Count - 1].yearsOld : comingOfAge)) : 0;
		surname = parent.surname;
		skinColor = parent.skinColor;
		ID = surname + name + Random.Range(0, 100);

	}

	public override void EveryBirthday() {

		if (GrownUp)
			Debug.Log(this + " should be an adult now");

	}

}

[System.Serializable]
public class Prole : Person {

	//PERSONAL STUFF
	public Node homeNode;
	public List<Child> children;
	public float personalSavings;
	public int physique, intellect, empathy;

	//WAITING FOR ACCEPTANCE STUFF
	public int waitCountdown;
	public bool accepted, rejected;

	//WORK STUFF
	public Node workNode;

	public bool SeekingWork { get { return !Employed && !Retired; } }
    public bool Employed { get { if (workNode == null) return false; return !workNode.Equals(unemploymentNode); } }
	public bool Homeless { get { if (homeNode == null) return true; return homeNode.Equals(unemploymentNode); } }
	public bool Retired { get { return yearsOld >= retirementAge; } }

	public int BirthChance { get; set; }
	readonly int birthChanceMax = 10;
	readonly int childrenMax = 4;

	public Node unemploymentNode { get { return new Node(-1, -1); } }

	//add 10% death chance if diseased
	//add 15% death chance if retired
	//add X% death chance depending on work hours
	public override int DeathChance { get { return base.DeathChance + (Retired ? DeathChanceFromAge : 0) + (Employed ? WorkDeathRisk() : 0); } }
	public int DeathChanceFromAge { get { return 15; } }

	//constructor
	public Prole(bool randomAge, bool wChildren, LaborType pref) : base(randomAge) {

		//default status
		workNode = unemploymentNode;
		homeNode = unemploymentNode;

		//random years old (14 + 2d6) if we need
		yearsOld = randomAge ? comingOfAge - 2 + Random.Range(1,7) + Random.Range(1, 7) : comingOfAge;
		children = new List<Child>();

		//if prole moves into the city with children
		if (wChildren) {

			int numChildren = Random.Range(0, 3);	//max of 2 children
			for (int i = 0; i < numChildren; i++)
				CreateChild();  //create children with random age

		}

		//roll random stats, taking pref into account
		RollStats(pref);

	}

	public Prole(Person person, LaborType pref) {

		//default status
		workNode = unemploymentNode;
		homeNode = unemploymentNode;

		//if coming from a child, make a new children list
		if (person is Child)
			children = new List<Child>();

		//we want same name, same age, same skin color, same ID
		yearsOld = person.yearsOld;
		deltaDays = person.deltaDays;
		surname = person.surname;
		name = person.name;
		skinColor = person.skinColor;
		ID = person.ID;

		//roll random stats, taking pref into account
		RollStats(pref);

	}

	public void RollStats(LaborType pref) {

		physique = 10;
		intellect = 10;
		empathy = 10;
		physique = Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
		intellect = Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
		empathy = Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
		

		//TAKE PREF INTO ACCOUNT SOMEHOW

	}

	public override void UpdateAge() {

		base.UpdateAge();

		//only do if prole is waiting for a job at the job centre
		if (waitCountdown > 0) {
			waitCountdown--;
			if (waitCountdown <= 0 || Retired)     //if countdown is over or got too old, leave the city
				rejected = true;
		}

	}

	public override void EveryMonth() {

		base.EveryMonth();

		//THINGS RELATED TO BIRTH CHANCE
		//increase birth chance if less than max
		if (BirthChance < birthChanceMax)
			BirthChance++;

		//roll to see whether birth occurs (when chance > 0, have less than max children, and is not diseased)
		if (BirthChance > 0 && children.Count <= childrenMax && !diseased) {

			int roll = Random.Range(1, 100);

			if (roll <= BirthChance) {
				CreateChild();
				BirthChance = 0;
			}

		}

	}

	public override void EveryBirthday() {

		base.EveryBirthday();

		//quit work if too old
		if (Retired && Employed) {

			Structure go = world.GetBuildingAt(workNode);
			workNode = unemploymentNode;

			if (go == null)
				Debug.LogError("Workplace at " + workNode + " for " + this + " does not exist");

			Workplace wrk = go.GetComponent<Workplace>();
			if(!wrk.HireRetiredProles)
				QuitWork();
		}

	}

	public override string GetCauseOfDeath() {

		int roll = Random.Range(1, DeathChanceFromAge + DeathChanceFromDisease + WorkDeathRisk());

		//get death message for old age
		if (roll <= DeathChanceFromAge && Retired)
			return "passed away peacefully.";

		//get death message for disease
		else if (roll <= (DeathChanceFromAge + DeathChanceFromDisease) && diseased)
			return "died of drinking dirty water.";

		//get death message for workplace accident
		else if(roll <= DeathChanceFromAge + DeathChanceFromDisease + WorkDeathRisk() && Employed) {

			Structure go = world.GetBuildingAt(workNode);

			if (go == null)
				Debug.LogError("Workplace at " + workNode + " for " + this + " does not exist");

			Workplace wrk = go.GetComponent<Workplace>();
			return wrk.deathDesc;
		}

		return "died mysteriously.";

	}

	public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object obj) {

        Prole pr = (Prole)obj;
        return ID == pr.ID;

	}

	void CreateChild() {

		children.Add(new Child(true, this));

	}

	public Prole GrowUpChild(Child c) {

		if (!children.Contains(c))
			Debug.LogError(this + " trying to grow up child " + c + " which is not its own");

		//SOMEWHERE HERE DETERMINE RANDOM LABOR PREF FOR

		children.Remove(c);

		Prole grownup = new Prole(c, LaborType.Physical);

		return grownup;

	}

	public bool CheckExistence() {

        bool kill = false;
        Structure h = world.GetBuildingAt(homeNode);

        if (h == null) {
            kill = true;
            return !kill;
        }

        if (!h.name.Contains("House")) {
            kill = true;
            return !kill;
        }

        House house = h.GetComponent<House>();

        if (!house.Residents.Contains(this))
            kill = true;

        if (kill)
            QuitWork();

        return !kill;

    }

    public void QuitWork() {

        if (!Employed)
            return;

        Structure go = world.GetBuildingAt(workNode);

		if (go == null)
            return;

        Workplace wrk = go.GetComponent<Workplace>();
        wrk.RemoveWorker(this);

    }

	public void EvictHouse(bool separateChildren) {

		if (Homeless)
			return;

		Structure go = world.GetBuildingAt(homeNode);
		homeNode = unemploymentNode;

		if (go == null)
			return;

		//go through house's residents and delete this one
		House h = go.GetComponent<House>();
		h.RemoveResident(this, separateChildren);

	}

    public void JoinWork(Workplace w) {

        workNode = new Node(w);

    }

	public void MoveIntoHouse(House h) {

		world = h.world.Map;
		homeNode = new Node(h);

	}

    public void PayWages(float wages) {

        Structure go = world.GetBuildingAt(homeNode);

        if (go == null)
            Debug.LogError(this + " does not have a house at " + homeNode + " anymore");

        House house = go.GetComponent<House>();
        house.Savings += wages;

    }

	public int WorkDeathRisk() {

		if (!Employed)
			return 0;

		Structure go = world.GetBuildingAt(workNode);

		if (go == null)
			return 0;

		Workplace wrk = go.GetComponent<Workplace>();

		//workplace accidents only happen with physical jobs
		if (wrk.laborType != LaborType.Physical)
			return 0;
		
		int excess = wrk.WorkingDay - 8;
		int riskFromExcess = excess > 0 ? excess : 0;	//risk from working for too many hours

		int minus = GetLaborBonus(LaborType.Physical) * -1;
		int riskFromPhysique = minus < 0 ? (minus + 1) : 0;        //risk from having weaker physique

		int ageDiff = yearsOld - retirementAge;
		int riskFromAge = ageDiff > 0 ? (ageDiff + 1) : 0;		//risk from being too old to work physically

		return riskFromAge + riskFromExcess + riskFromPhysique;	//multiple total risk by 2 if this prole does not prefer physical labor

	}

	public LaborType HighestValue() {

		if (physique >= intellect && physique >= empathy)
			return LaborType.Physical;

		else if (intellect >= physique && intellect >= empathy)
			return LaborType.Intellectual;

		else if (empathy >= physique && empathy >= intellect)
			return LaborType.Emotional;

		return LaborType.END;

	}

	public int GetLaborScore(LaborType lt) {

		if (lt == LaborType.Physical)
			return physique;
		else if (lt == LaborType.Intellectual)
			return intellect;
		else if (lt == LaborType.Emotional)
			return empathy;

		return -1;

	}
	
	public int WaitTime { get { return TimeController.DaysInASeason; } }

	public void StartWaitCountdown() {
		
		waitCountdown = WaitTime;

	}

	public float WaitTimePercent() {

		return (float)(WaitTime - waitCountdown) / WaitTime;

	}

	public int GetLaborBonus(LaborType lt) {

		int score = GetLaborScore(lt);

		//exceptions to the rule for extremes
		if (score == 18)
			return 3;
		if (score == 1)
			return -4;

		return (int)((float)(score - 10) / 3);	//each point in bonus represents +5% in productivity

	}

	public float GetWorkerEffectiveness(LaborType lt) {
		
		float eff = Retired ? .75f : 1;   //retired status makes worker less effective
		float bonus = GetLaborBonus(lt);
		eff += bonus * .05f;  //each bonus point is equal to +5% productivity
		return eff;

	}

}