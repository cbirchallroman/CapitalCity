using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulThings;

[System.Serializable]
public class HouseSave : StructureSave {

	public int Hygiene, Culture, Corpses, DiseasedResidents;
	public int Thirst, Hunger;
	public float Savings;

	public List<Prole> Residents;

	public int[] Goods;
	public Quality WaterQualCurrent, FoodQualCurrent;

	public DictContainer<string, int> VenueAccess { get; set; }

	public bool Diseased;

	public HouseSave(GameObject go) : base(go) {

		House h = go.GetComponent<House>();

		Hygiene = h.Hygiene;
		Savings = h.Savings;
		Corpses = h.Corpses;
		DiseasedResidents = h.DiseasedResidents;

		//thirst
		Thirst = h.Thirst;
		WaterQualCurrent = h.WaterQualCurrent;

		//hunger
		Hunger = h.Hunger;
		FoodQualCurrent = h.FoodQualCurrent;

		Goods = h.Goods;

		VenueAccess = new DictContainer<string, int>(h.VenueAccess);
		Culture = h.Culture;

		Residents = h.Residents;

	}

}

public class House : Structure {

	[Header("House")]
	public int level;

	public string evolvesTo;
	public string devolvesTo;
	public string biggerHouse;
	public int residentsMax = 1;
	public int prosperityRating;
	public int desirabilityNeeded;
	public int desirabilityWanted;
	public int HouseSize { get { return Sizex; } }
	public float Savings { get; set; }
	public List<Prole> Residents { get; set; }
	public int Corpses { get; set; }

	//public int ExcessResidents { get { return Residents - residentsMax; } }

	public override void Load(ObjSave o) {

		base.Load(o);

		HouseSave h = (HouseSave)o;

		Hygiene = h.Hygiene;
		Savings = h.Savings;
		Corpses = h.Corpses;
		DiseasedResidents = h.DiseasedResidents;

		//thirst
		Thirst = h.Thirst;
		WaterQualCurrent = h.WaterQualCurrent;

		//hunger
		Hunger = h.Hunger;
		FoodQualCurrent = h.FoodQualCurrent;

		Goods = h.Goods;

		VenueAccess = h.VenueAccess.GetDictionary();
		Culture = h.Culture;

		Residents = h.Residents;

	}

	public override void Activate() {

		base.Activate();

		float rotation = Random.Range(0, 4) * 90;
		transform.eulerAngles = new Vector3(0, rotation, 0);

		//add population to world
		scenario.AddHouseLevel(level - 1);

	}

	public override void DoEveryDay() {

		base.DoEveryDay();

		if (!ActiveSmartWalker && !immigration.Contains(this) && Residents.Count < residentsMax && DiseasedResidents == 0)
			RequestImmigrant();

		//proles who move out receive a fraction of the house's total savings to take with them
		if (Residents.Count > residentsMax) {

			Prole mover = Residents[Residents.Count - 1];

			mover.personalSavings += Savings / Residents.Count;
			Savings -= mover.personalSavings;

			immigration.TryEmigrant(mover);
			population.RemoveProle(mover);

		}

		if (CanEvolve())
			ChangeHouse(evolvesTo);

		CheckBiggerSize();
		UpdateResidentsAge();
		cholera.SetActive(DiseasedResidents > 0);

	}

	public override void DoEveryWeek() {

		ThrowWaste();

		IncreaseThirst();
		IncreaseHunger();

	}

	public override void DoEveryMonth() {

		base.DoEveryMonth();

		//WE'RE CHECKING FOR DEVOLUTION HERE
		//	that way a house doesn't devolve immediately before consumption, whether checking every day or checking after consuming for the month
		if (CanDevolve())
			ChangeHouse(devolvesTo);

		//consume things
		ConsumeGoods();
		ConsumeCulture();
		SpreadDisease();

	}

	public bool WantsBetterWater { get { return WaterQualCurrent < waterQualWanted; } }
	public bool WantsMoreFood { get { return FoodQualCurrent < foodQualWant; } }
	public bool WantsBetterCulture { get { return Culture < cultureWant; } }
	public bool WantsBetterDesirability { get { return LocalDesirability < desirabilityWanted; } }

	public bool CanEvolve() {

		if (DiseasedResidents > 0)   //we're not going to evolve if the house is diseased
			return false;

		//now we check for actual conditions
		if (WantsBetterWater)
			return false;
		if (WantsMoreFood)
			return false;
		if (WantsMoreGoods)
			return false;
		if (WantsBetterCulture)
			return false;
		if (WantsBetterDesirability)
			return false;
		return true;

	}

	public bool CanDevolve() {

		if (WaterQualCurrent < waterQualNeeded)
			return true;
		if (FoodQualCurrent < foodQualNeeded)
			return true;
		if (MissingGoods)
			return true;
		if (Culture < cultureNeeded)
			return true;
		if (LocalDesirability < desirabilityNeeded)
			return true;
		return false;

	}

	//ADD CONDITIONS TO MAKE BIGGER
	public void CheckBiggerSize() {

		//if no bigger house to evolve to, don't continue
		if (string.IsNullOrEmpty(biggerHouse))
			return;

		//make containers for adjacent houses
		World map = world.Map;

		House[] neighbors = new House[3];
		Node[] checks = { new Node(X + HouseSize + 1, Y),
			new Node(X, Y + HouseSize + 1),
			new Node(X + HouseSize + 1, Y + HouseSize + 1) };

		//iterate through nearby houses
		//	if at any point conditions are not met for merge, don't continue at all
		for (int i = 0; i < neighbors.Length; i++) {

			House h = map.GetBuildingAt(checks[i]).GetComponent<House>();
			if (h == null)
				return;
			if (elevation != h.elevation)
				return;
			if (h.HouseSize != HouseSize)
				return;
			if (h.level != level)
				return;
			neighbors[i] = h;

		}

		//combine arrays
		Thirst = Thirst + neighbors[0].Thirst + neighbors[1].Thirst + neighbors[2].Thirst;
		Hunger = Hunger + neighbors[0].Hunger + neighbors[1].Hunger + neighbors[2].Hunger;

		//combine stats and destroy houses
		foreach (House h in neighbors) {

			foreach (Prole p in h.Residents)
				Residents.Add(p);
			world.Destroy(h.X, h.Y);

		}

		ChangeHouse(biggerHouse);

	}

	bool changingHouse;
	public void ChangeHouse(string s) {

		//if s is null, don't make the new house
		if (string.IsNullOrEmpty(s))
			return;

		//demolish this and build new house
		changingHouse = true;
		world.Demolish(X, Y);
		world.SpawnStructure(s, X, Y, transform.position.y);

		//set vars of new house to the ones from this one
		House newHouse = world.Map.GetBuildingAt(X, Y).GetComponent<House>();
		//newHouse.Residents = Residents;
		newHouse.Thirst = Thirst;
		newHouse.Hunger = Thirst;
		newHouse.Savings = Savings;
		newHouse.WaterQualCurrent = WaterQualCurrent;
		newHouse.FoodQualCurrent = FoodQualCurrent;
		newHouse.Goods = Goods;
		newHouse.Culture = Culture;
		newHouse.VenueAccess = VenueAccess;
		newHouse.DiseasedResidents = DiseasedResidents;
		newHouse.Residents = Residents;

		//USED TO TEST OVERCROWDING
		//if (newHouse.level > level)
		//	newHouse.ReceiveImmigrant(new Prole());

	}

	public void FreshHouse(Prole firstResident) {

		Thirst = 0;
		Hunger = 0;
		WaterQualCurrent = Quality.None;
		FoodQualCurrent = Quality.None;
		Goods = new int[(int)GoodType.END];
		VenueAccess = new Dictionary<string, int>();
		Residents = new List<Prole>();

		//CREATE NEW RESIDENT HAPPENS RIGHT HERE
		ReceiveImmigrant(firstResident);

	}

	public override void UponDestruction() {

		base.UponDestruction();
		scenario.RemoveHouseLevel(level - 1);

		//ONLY DO THIS IF HOUSE IS NOT EVOLVING OR DEVOLVING
		if (!changingHouse && !Disaster)
			foreach (Prole mover in Residents) {

				p.QuitWork();
				mover.personalSavings += Savings / Residents.Count;
				immigration.TryEmigrant(mover);
				population.RemoveProle(mover);

			}

		else if (!changingHouse && Disaster) {

			foreach (Prole p in Residents) {

				//don't need them to evict the house because the house won't exist anymore and they'll be gone from other references
				//	we can't call p.Kill() because that will try to spawn orphans and also try to search for the house through Find()
				p.QuitWork();
				population.RemoveProle(p);

			}

		}




	}

	public override void ReceiveItem(ItemOrder io) {

		if (io.type == ItemType.Meal) {

			Hunger -= io.amount;
			FoodQualCurrent = ResourcesDatabase.GetQuality(io.GetItemName());

		}

		else if (io.type == ItemType.Good)
			Goods[io.item] += io.amount;

	}

	public override void ReceiveImmigrant(Prole p) {

		p.MoveIntoHouse(this);
		Residents.Add(p);
		//Debug.Log(p + " moved into " + this);
		if (Residents.Count > residentsMax)
			Debug.Log("Not enough room in " + name + " for " + p);
		Savings += p.personalSavings;
		p.personalSavings = 0;
		population.AddProle(p);     //add to prole list of town

	}

	void UpdateResidentsAge() {

		//iterate backwards to not have any problems with removing residents or children
		for (int i = Residents.Count - 1; i >= 0; i--) {

			Prole p = Residents[i];
			bool diseased = p.diseased;
			p.UpdateAge();
			if (!p.diseased && diseased)
				RemoveDiseasedResident();   //if resident was diseased and is no longer bc time ran out, remove one from the diseased count

			//the reason we age children here and not in Prole.UpdateAge() is so we can detect death and coming of age
			for (int j = p.children.Count - 1; j >= 0; j--) {

				Child c = p.children[j];
				bool diseasedC = c.diseased;
				c.UpdateAge();
				if (!c.diseased && diseasedC)
					RemoveDiseasedResident();   //if the child was diseased and is no longer bc time ran out, remove one from the diseased count

				//do if c is dead
				if (c.markedForDeath) {

					//if c was diseased upon death, remove one from diseased count
					if (c.diseased) RemoveDiseasedResident();
					p.children.Remove(c);
					c.Kill();
					Corpses++;

				}

				//do if c is grown up
				else if (c.GrownUp) {

					ReceiveImmigrant(p.GrowUpChild(c));
					p.children.Remove(c);

				}

			}

			//do if p is about to die
			if (p.markedForDeath) {

				//if c was diseased upon death, remove one from diseased count
				if (p.diseased) RemoveDiseasedResident();

				p.Kill();
				population.RemoveProle(p);
				Corpses++;

			}

		}

	}

	/*************************************
    HYGIENE STATS AND DISEASE SPREAD
    *************************************/

	public GameObject cholera;
	public int Hygiene { get; set; }
	public int HygieneToConsume { get { return HouseSize; } }
	public int HygieneMax { get { return HouseSize * 3; } }
	public int Waste { get { return HouseSize; } }
	public int DiseasedResidents { get; set; }

	void ConsumeHygiene() {

		if (Hygiene > 0)
			Hygiene -= HygieneToConsume;

	}

	void SpreadDisease() {

		if (DiseasedResidents == 0)
			return;

		int chance = DiseasedResidents * 5;
		bool success = Random.Range(1, 100) + Hygiene * Residents.Count <= chance;

		if (!success)
			return;

		bool spreadDisease = false;

		//goal is to spread disease to one person
		foreach (Prole res in Residents) {

			//test children first, youngest to oldest
			bool diseasedChild = false;     //we can't break the outer loop from the child loop so we need this extra bool
			for (int i = res.children.Count - 1; i >= 0 && !diseasedChild; i--) {

				Child c = res.children[i];
				if (!c.diseased) {
					c.TurnDiseased();
					spreadDisease = true;
					diseasedChild = true;
				}

			}

			if (diseasedChild) break;   //so if child turned diseased, break right here

			//then test adults, which are less suspect to disease
			if (!res.diseased) {

				res.TurnDiseased();
				spreadDisease = true;
				break;

			}

		}

		//if we successfully spread disease, up count of diseased residents by 1
		if (spreadDisease)
			AddDiseasedResident();

	}

	public void AddDiseasedResident() {

		DiseasedResidents++;

	}

	public void RemoveDiseasedResident() {

		DiseasedResidents--;

	}

	void ThrowWaste() {

		List<Node> roads = GetAdjRoadTiles();

		foreach (Node n in roads) {

			int x = n.x;
			int y = n.y;
			int mult = DiseasedResidents + 1;

			if (world.Map.cleanliness[x, y] < 100)
				world.Map.cleanliness[x, y] += Waste * mult;

			if (world.Map.cleanliness[x, y] > 100)
				world.Map.cleanliness[x, y] = 100;

		}

	}

	/*************************************
    THIRST STATS
    *************************************/

	public int Thirst { get; set; }
	public int ThirstRate { get { return 1; } }
	public int MaxThirst { get { return ThirstRate * TimeController.MonthTime * 12; } }
	public int DesperateThirst { get { return ThirstRate * TimeController.MonthTime * 3; } } //if it's been 3 months

	public bool DesperatelyThirsty { get { return Thirst >= DesperateThirst; } }

	public Quality waterQualNeeded;
	public Quality waterQualWanted;
	public Quality WaterQualCurrent { get; set; }

	void IncreaseThirst() {

		Thirst += ThirstRate;
		if (Thirst > MaxThirst)
			Thirst = MaxThirst;

	}

	public bool WillAcceptWaterVisit(Quality visitingQual) {

		//if thirsty enough, we'll accept any amount of water
		if (DesperatelyThirsty)
			return true;

		return visitingQual >= WaterQualCurrent;

	}

	public void ReceiveWater(Quality qual, int water) {

		if (qual < WaterQualCurrent)
			Debug.Log(this + " getting lower quality water than before...");
		WaterQualCurrent = qual;
		Thirst -= water;
		if (Thirst < 0)
			Thirst = 0;

	}

	/*************************************
    HUNGER STATS
    *************************************/

	public int Hunger { get; set; }
	public int HungerRate { get { return Residents.Count; } }
	public int MaxHunger { get { return HungerRate * TimeController.MonthTime * 12; } }
	public int DesperateHunger { get { return HungerRate * TimeController.MonthTime * 3; } }

	public bool DesperatelyHungry { get { return Hunger >= DesperateHunger; } }

	public Quality foodQualNeeded;
	public Quality foodQualWant;
	public Quality FoodQualCurrent { get; set; }

	void IncreaseHunger() {

		Hunger += HungerRate;
		if (Hunger > MaxHunger)
			Hunger = MaxHunger;

	}

	public bool WillAcceptFoodTypes(Quality qual) {

		if (DesperatelyHungry)
			return true;

		return qual >= FoodQualCurrent;

	}

	public void ReceiveFood(Quality qual, int food) {

		if (qual < FoodQualCurrent)
			Debug.Log(this + " buying lower quality food than before...");
		FoodQualCurrent = qual;
		Hunger -= food;
		if (Hunger < 0)
			Hunger = 0;

	}

	/*************************************
    GOODS STATS
    *************************************/

	public int[] Goods { get; set; }
	public GoodType[] goodsNeeded;
	public GoodType goodWanted = GoodType.END;

	public int GoodsMax { get { return 3 * HouseSize; } }
	public int GoodsNeeded(int item) { return GoodsMax - Goods[item]; }
	public int GoodsToConsume { get { return HouseSize; } }

	void ConsumeGoods() {

		for (int b = 0; b < Goods.Length; b++) {


			if (Goods[b] >= GoodsToConsume)
				Goods[b] -= GoodsToConsume;
			else
				Goods[b] = 0;

		}

	}

	public bool WantsMoreGoods {
		get {

			if (goodWanted == GoodType.END)
				return false;
			return Goods[(int)goodWanted] == 0;

		}
	}
	public bool MissingGoods {
		get {
			foreach (GoodType good in goodsNeeded) {

				if (Goods[(int)good] == 0)
					return true;

			}
			return false;
		}
	}

	/*************************************
    CULTURE STATS
    *************************************/

	public Dictionary<string, int> VenueAccess { get; set; }
	public int Culture { get; set; }
	public int cultureNeeded;
	public int cultureWant;

	public void SetCulture(string venue, int amount) {

		VenueAccess[venue] = amount;

	}

	public void AddCulture(string venue, int amount) {

		if (!VenueAccess.ContainsKey(venue))
			SetCulture(venue, amount);
		else
			VenueAccess[venue] += amount;

	}

	void ConsumeCulture() {

		Culture = 0;

		if (VenueAccess == null)
			VenueAccess = new Dictionary<string, int>();

		if (VenueAccess.Count == 0)
			return;

		foreach (string venue in VenueAccess.Keys) {

			GameObject go = GameObject.Find(venue);
			if (go == null) {

				VenueAccess.Remove(venue);
				continue;

			}

			CulturalVenue c = go.GetComponent<CulturalVenue>();

			VenueAccess[venue]--;
			Culture += c.culturePoints;

			if (VenueAccess[venue] <= 0)
				VenueAccess.Remove(venue);

		}

	}

	public override void OpenWindow() {

		OpenWindow(UIObjectDatabase.GetUIElement("HouseWindow"));

	}

	public override string GetDescription() {

		if (DiseasedResidents > 0)
			return "Infested with cholera";
		if (WantsBetterWater) {
			if (waterQualWanted == Quality.Poor)
				return "Needs water to evolve";
			else
				return "Needs filtered water to evolve";
		}
		if (WantsMoreFood) {
			return "Needs " + foodQualWant + " type of food to evolve";
		}
		if (WantsMoreGoods) {

			return "Needs " + goodWanted + " to evolve";

		}
		if (WantsBetterCulture)
			return "Ahhh";
		if (WantsBetterDesirability)
			return "Needs better surroundings to evolve";
		return "Why haven't I evolved yet?";

	}

	public ItemOrder WillBuy(int item, ItemType type, int amountStored) {

		if (amountStored == 0)
			return null;

		//use the relevant variables for food or goods
		switch (type) {
			case ItemType.Meal:
				return WillBuyMeal(item, amountStored);
			case ItemType.Good:
				return WillBuyGood(item, amountStored);
		}

		return null;

	}

	public ItemOrder WillBuyMeal(int item, int amountStored) {

		//if you don't want anything, stop
		if (foodQualWant == Quality.None)
			return null;

		//if this food is not high enough quality and we're not desperate enough, don't buy

		if (!WillAcceptFoodTypes(ResourcesDatabase.GetQuality(Enums.GetItemName(item, ItemType.Meal))))
			return null;

		int delta = Hunger;

		//if we don't have that much food, sell as much as we can
		if (amountStored < delta)
			delta = amountStored;

		ItemOrder io = new ItemOrder(delta, item, ItemType.Meal);

		//if house cannot afford, find the largest amount it can buy for the smallest price
		if (io.ExchangeValue() > Savings) {
			float priceOfOne = ResourcesDatabase.GetBasePrice(new ItemOrder(1, item, ItemType.Meal));
			int smallestAmount = (int)(Savings / priceOfOne);
			int smallestAmountWantToBuy = (int)(0.25f * delta);

			if (smallestAmount <= smallestAmountWantToBuy)
				return null;

			io.amount = smallestAmount;
		}

		return io;

	}

	public ItemOrder WillBuyGood(int item, int amountStored) {

		bool wantsToBuy = false;

		//check the wanted good first if there is one
		if (goodWanted != GoodType.END) {

			//only proceed if the good considered is the good that this house wants
			if ((int)goodWanted == item)
				wantsToBuy = true;

		}

		//now we're going to check the same for each good that we need if we don't want it to evolve
		//	if we don't need this good, don't proceed
		if (!wantsToBuy) {
			foreach (GoodType good in goodsNeeded) {

				if ((int)good == item)
					wantsToBuy = true;

			}
		}

		//if we don't want this good, return null
		if (!wantsToBuy)
			return null;

		//the smallest amount we want to buy is 1 unit, and the most we can have is 2 * houseSize
		//	if the vendor has less goods than we want, match that
		int delta = GoodsMax - Goods[item];
		if (amountStored < delta)
			delta = amountStored;

		ItemOrder io = new ItemOrder(delta, item, ItemType.Good);

		//if house cannot afford, find the largest amount it can buy for the smallest price
		if (io.ExchangeValue() > Savings) {

			float priceOfOne = ResourcesDatabase.GetBasePrice(new ItemOrder(1, item, ItemType.Good));
			int smallestAmount = (int)(Savings / priceOfOne);
			int smallestAmountWantToBuy = (int)(0.25f * delta);

			if (smallestAmount <= smallestAmountWantToBuy)
				return null;

			io.amount = smallestAmount;

		}

		return io;

	}

}