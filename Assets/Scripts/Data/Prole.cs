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

	public readonly int ageInterval = TimeController.DaysInASeason;
	public readonly int comingOfAge = 16;
	public readonly int retirementAge = 65;
	
	public virtual int DeathChance { get { return 0; } }

	public string FullName { get { return name + " " + surname; } }

	public Person() : this(false) { }

	public Person(bool randomAge) {

		deltaDays = randomAge? Random.Range(0, ageInterval) : 0;

		surname = PopulationController.GetRandomSurname();
		name = PopulationController.GetRandomFirstName();
		skinColor = GetSkinColor();
		ID = surname + name + Random.Range(0, 100);

	}

	public virtual void UpdateAge() {

		deltaDays++;
		if(deltaDays == ageInterval) {

			deltaDays = 0;
			yearsOld++;
			//Debug.Log("Happy birthday to " + this + "!");

		}

		//check for chance of death
		if(DeathChance > 0) {

			int roll = Random.Range(0, 100);
			if (roll <= DeathChance)
				MarkForDeath();

		}

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

	}

	public override string ToString() {

		return FullName;

	}

	public virtual void Kill() {



	}

}

[System.Serializable]
public class Child : Person {

	bool GrownUp { get { return yearsOld >= comingOfAge; } }

	//add 20% death chance if diseased
	public override int DeathChance { get { return diseased ? 20 : 0; } }

	public Child(bool randomAge, Adult parent) : base(randomAge) {

		yearsOld = randomAge ? Random.Range(0, comingOfAge - 1) : 0;
		surname = parent.surname;
		skinColor = parent.skinColor;
		ID = surname + name + Random.Range(0, 100);

	}

	public override void UpdateAge() {

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
    public bool Employed { get { return workNode != null; } }
	public bool Retired { get { return yearsOld >= retirementAge; } }

	//add 10% death chance if diseased
	//add 15% death chance if retired
	//add X% death chance depending on work hours
	public override int DeathChance { get { return (diseased ? 10 : 0) + (Retired ? 15 : 0) + WorkDeathRisk(); } }

	//constructor
	public Adult(bool randomAge, bool wChildren, LaborType pref) : base(randomAge) {

		yearsOld = randomAge ? Random.Range(comingOfAge, retirementAge - 1) : 0;
		children = new List<Child>();

		//if prole moves into the city with children
		if (wChildren) {

			int numChildren = Random.Range(0, 3);	//max of 2 children
			for (int i = 0; i < numChildren; i++)
				children.Add(new Child(true, this));  //create children with random age

		}

		laborPref = pref;
		
	}

	public override void UpdateAge() {

		base.UpdateAge();

		if (Retired) {
			QuitWork();
		}

	}

	Adult(Person person, LaborType pref) {

		//we want same name, same age, same skin color, same ID
		yearsOld = person.yearsOld;
		deltaDays = person.deltaDays;
		surname = person.surname;
		name = person.name;
		skinColor = person.skinColor;
		ID = person.ID;
		laborPref = pref;

	}
    
    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object obj) {

        Adult pr = (Adult)obj;
        return ID == pr.ID;

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

        if (workNode == null)
            return;

        GameObject go = world.GetBuildingAt(workNode);
		workNode = null;

		if (go == null)
            Debug.LogError("Workplace at " + workNode + " for " + this + " does not exist");

        Workplace wrk = go.GetComponent<Workplace>();
        wrk.RemoveWorker(workIndex);

    }

	public void EvictHouse(bool separateChildren) {

		if (homeNode == null)
			return;

		GameObject go = world.GetBuildingAt(homeNode);
		homeNode = null;

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

		if (workNode == null)
			return 0;

		GameObject go = world.GetBuildingAt(workNode);
		workNode = null;

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
