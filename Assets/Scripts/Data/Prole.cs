using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Person {

	public string surname, name, ID;
	public int yearsOld, deltaDays;
	public bool diseased, markedForDeath;
	public Float3d skinColor;
	public World world;

	public readonly int ageInterval = TimeController.DaysInAYear;
	public readonly int comingOfAge = 16;
	public readonly int retirementAge = 65;
	
	public virtual int DeathChance { get { return diseased ? DeathChanceFromDisease : 0; } }
	public virtual int DeathChanceFromDisease { get { return 10; } }

	public string FullName { get { return name + " " + surname; } }

	public Person() : this(false) { }

	public Person(bool randomAge) {

		deltaDays = randomAge? Random.Range(0, ageInterval) : 0;

		surname = PopulationController.GetRandomSurname();
		name = PopulationController.GetRandomFirstName();
		skinColor = GetSkinColor();
		ID = surname + name + Random.Range(0, 100);

	}

	public void UpdateAge() {

		deltaDays++;
		if (deltaDays % TimeController.DaysInAMonth == 0)
			EveryMonth();

		if(deltaDays == ageInterval) {

			deltaDays = 0;
			yearsOld++;
			EveryBirthday();
			//Debug.Log("Happy birthday to " + this + "!");

		}

	}

	public virtual void EveryMonth() {

		//check for chance of death
		if (DeathChance > 0) {

			int roll = Random.Range(1, 100);
			Debug.Log("rolled " + roll);
			if (roll <= DeathChance)
				MarkForDeath();

		}

	}

	public virtual void EveryBirthday() {
		

	}

	Float3d GetSkinColor() {

		float start_r = 61f / 255f;
		float start_g = 28f / 255f;
		float start_b = 10f / 255f;
		float diff_r = (229f - 61) / 255f;
		float diff_g = (186f - 28) / 255f;
		float diff_b = (84f - 10) / 255f;

		float percent = Random.Range(0f, 1);
		diff_r *= percent;
		diff_g *= percent;
		diff_b *= percent;

		return new Float3d(start_r + diff_r, start_g + diff_g, start_b + diff_b);

	}

	public void MarkForDeath() {

		markedForDeath = true;
		Debug.Log(this + " " + GetCauseOfDeath());

	}

	public override string ToString() {

		return FullName;

	}

	public virtual void Kill() {



	}

	public virtual string GetCauseOfDeath() {

		return "died of drinking dirty water.";

	}

}

[System.Serializable]
public class Child : Person {

	bool GrownUp { get { return yearsOld >= comingOfAge; } }

	//add 20% death chance if diseased
	public override int DeathChanceFromDisease { get { return 20; } }

	public Child(bool randomAge, Adult parent) : base(randomAge) {

		yearsOld = randomAge ? Random.Range(0, comingOfAge - 1) : 0;
		surname = parent.surname;
		skinColor = parent.skinColor;
		ID = surname + name + Random.Range(0, 100);

	}

	public override void EveryBirthday() {

		base.UpdateAge();

		if (GrownUp)
			Debug.Log(this + " should be an adult now");

	}

}

[System.Serializable]
public class Adult : Person {
	
    public int workIndex;
    public Node homeNode, workNode;
	public float personalSavings;
	public List<Child> children;
	public LaborType laborPref;

	public bool SeekingWork { get { return !Employed && !Retired; } }
    public bool Employed { get { return !workNode.Equals(unemploymentNode); } }
	public bool Homeless { get { return homeNode.Equals(unemploymentNode); } }
	public bool Retired { get { return yearsOld >= retirementAge; } }

	public int BirthChance { get; set; }
	readonly int birthChanceMax = 10;
	readonly int childrenMax = 6;

	Node unemploymentNode = new Node(-1, -1);

	//add 10% death chance if diseased
	//add 15% death chance if retired
	//add X% death chance depending on work hours
	public override int DeathChance { get { return base.DeathChance + (Retired ? DeathChanceFromAge : 0) + (Employed ? WorkDeathRisk() : 0); } }
	public int DeathChanceFromAge { get { return 15; } }

	//constructor
	public Adult(bool randomAge, bool wChildren, LaborType pref) : base(randomAge) {

		workNode = unemploymentNode;
		homeNode = unemploymentNode;
		yearsOld = randomAge ? Random.Range(comingOfAge, retirementAge - 1) : 0;
		children = new List<Child>();

		//if prole moves into the city with children
		if (wChildren) {

			int numChildren = Random.Range(0, 3);	//max of 2 children
			for (int i = 0; i < numChildren; i++)
				CreateChild();  //create children with random age

		}

		laborPref = pref;

	}

	public Adult(Person person, LaborType pref) {

		//we want same name, same age, same skin color, same ID
		workNode = unemploymentNode;
		homeNode = unemploymentNode;

		yearsOld = person.yearsOld;
		deltaDays = person.deltaDays;
		surname = person.surname;
		name = person.name;
		skinColor = person.skinColor;
		ID = person.ID;
		laborPref = pref;

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
		if (Retired) {
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

			GameObject go = world.GetBuildingAt(workNode);

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

        Adult pr = (Adult)obj;
        return ID == pr.ID;

	}

	void CreateChild() {

		children.Add(new Child(true, this));

	}

	public Adult GrowUpChild(Child c) {

		if (!children.Contains(c))
			Debug.LogError(this + " trying to grow up child " + c + " which is not its own");

		//SOMEWHERE HERE DETERMINE RANDOM LABOR PREF FOR

		children.Remove(c);
		return new Adult(c, LaborType.Physical);

	}

	public bool CheckExistence() {

        bool kill = false;
        string h = world.GetBuildingNameAt(homeNode);

        if (h == null) {
            kill = true;
            return !kill;
        }

        if (!h.Contains("House")) {
            kill = true;
            return !kill;
        }

        House house = world.GetBuildingAt(homeNode).GetComponent<House>();

        if (!house.Residents.Contains(this))
            kill = true;

        if (kill)
            QuitWork();

        return !kill;

    }

    public void QuitWork() {

        if (!Employed)
            return;

        GameObject go = world.GetBuildingAt(workNode);
		workNode = unemploymentNode;

		if (go == null)
            Debug.LogError("Workplace at " + workNode + " for " + this + " does not exist");

        Workplace wrk = go.GetComponent<Workplace>();
        wrk.RemoveWorker(workIndex);

    }

	public void EvictHouse(bool separateChildren) {

		if (Homeless)
			return;

		GameObject go = world.GetBuildingAt(homeNode);
		homeNode = unemploymentNode;

		if (go == null)
			return;

		//cycle through house's residents and delete this one
		House h = go.GetComponent<House>();
		for (int i = 0; i < h.Residents.Count; i++)
			if (h.Residents[i] == this)
				h.Residents.RemoveAt(i);

		if (separateChildren) {

			Debug.Log("Children should go to orphanage");

		}

	}

	public override void Kill() {
		
		QuitWork();
		EvictHouse(true);

	}

    public void JoinWork(Workplace w, int i) {

        workNode = new Node(w);
        workIndex = i;

    }

	public void MoveIntoHouse(House h) {

		world = h.world.Map;
		homeNode = new Node(h);

	}

    public void PayWages(float wages) {

        GameObject go = world.GetBuildingAt(homeNode);

        if (go == null)
            Debug.LogError(surname + " does not have a house at " + homeNode + " anymore");

        House house = go.GetComponent<House>();
        house.Savings += wages;

    }

	public int WorkDeathRisk() {

		if (!Employed)
			return 0;

		GameObject go = world.GetBuildingAt(workNode);

		if (go == null)
			Debug.LogError("Workplace at " + workNode + " for " + this + " does not exist");

		Workplace wrk = go.GetComponent<Workplace>();

		if (wrk.laborType != LaborType.Physical)
			return 0;

		int workingDay = wrk.WorkingDay;
		int excess = workingDay - 8;
		return excess > 0 ? excess * (wrk.laborType != laborPref ? 2 : 1) : 0;	//multiple risk by 2 if this prole does not prefer physical labor

	}

}
