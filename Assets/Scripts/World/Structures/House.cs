using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulThings;

[System.Serializable]
public class HouseSave : StructureSave {
    
    public int HouseSize, Prosperity, Culture, MonthsLeftDiseased, Corpses;
    public float CasualtyRisk, Savings;

    public List<Adult> Residents;

    public int[] Water, Food, Goods;
    public Quality WaterQual { get; set; }

    public DictContainer<string, int> VenueAccess { get; set; }

    public bool Diseased;

    public HouseSave(GameObject go) : base(go) {

        House h = go.GetComponent<House>();
        
        //Residents = h.Residents;
        HouseSize = h.HouseSize;
        Prosperity = h.prosperityRating;
        Savings = h.Savings;
        MonthsLeftDiseased = h.MonthsLeftDiseased;
		Corpses = h.Corpses;

		CasualtyRisk = h.CasualtyRisk;

        Water = h.Water;
        WaterQual = h.WaterQual;

        Food = h.Food;

        Goods = h.Goods;

        VenueAccess = new DictContainer<string, int>(h.VenueAccess);
        Culture = h.Culture;

        Diseased = h.Diseased;

        Residents = h.Residents;

    }

}

public class House : Structure {

    [Header("House")]
    public int level;
    //public int Residents { get; set; }
    public int residentsMax = 1;
    public PeopleType peopleType = PeopleType.Proles;
    public int HouseSize { get; set; }
    public int prosperityRating;
    public int desirabilityNeeded;
    public int desirabilityWanted;
    public float Savings { get; set; }
    public List<Adult> Residents { get; set; }
	public int Corpses { get; set; }

    public string evolvesTo;
    public string devolvesTo;
    public string biggerHouse;
    
    //public int ExcessResidents { get { return Residents - residentsMax; } }

    public override void Load(ObjSave o) {

        base.Load(o);

        HouseSave h = (HouseSave)o;

        //Residents = h.Residents;
        HouseSize = Sizex;
        prosperityRating = h.Prosperity;
        Savings = h.Savings;
        MonthsLeftDiseased = h.MonthsLeftDiseased;
		Corpses = h.Corpses;

		Water = h.Water;
        WaterQual = h.WaterQual;

        CasualtyRisk = h.CasualtyRisk;

        Food = h.Food;

        Goods = h.Goods;

        VenueAccess = h.VenueAccess.GetDictionary();
        Culture = h.Culture;

        Diseased = h.Diseased;

        Residents = h.Residents;

    }

    public override void Activate() {

        base.Activate();

        float rotation = Random.Range(0, 4) * 90;
        transform.eulerAngles = new Vector3(0, rotation, 0);

        //add population to world
        scenario.AddHouseLevel(level - 1);

        //housesize is equal to the size of the structure
        HouseSize = Sizex;

    }

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (!ActiveSmartWalker && !immigration.Contains(this) && Residents.Count < residentsMax && !Diseased)
            RequestImmigrant();
		
		//proles who move out receive a fraction of the house's total savings to take with them
		if (Residents.Count > residentsMax) {

			Adult mover = Residents[Residents.Count - 1];

			mover.personalSavings += Savings / Residents.Count;
			Savings -= mover.personalSavings;

			immigration.TryEmigrant(mover);
			population.RemoveProle(mover);

		}

		if (CanEvolve())
            ChangeHouse(evolvesTo);
        if (CanDevolve())
            ChangeHouse(devolvesTo);

        CheckBiggerSize();
        cholera.SetActive(Diseased);
		UpdateResidentsAge();
		ThrowWaste();

	}
	
    public override void DoEveryMonth() {

        base.DoEveryMonth();

        ConsumeWater();
        ConsumeFood();
        ConsumeGoods();
        ConsumeCulture();
        SpreadDisease();

    }
	
    public bool WantsBetterWater { get { return WaterQual < waterQualWanted; } }
    public bool WantsBetterCulture { get { return Culture < cultureWant; } }
    public bool WantsBetterDesirability { get { return LocalDesirability < desirabilityWanted; } }

    public bool CanEvolve() {

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
        if (Diseased)
            return false;
        return true;

    }

    public bool CanDevolve() {

        if (WaterQual < waterQualNeeded)
            return true;
        if (NumOfFoods() < foodTypesNeeded)
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
        House h1 = null;
        House h2 = null;
        House h3 = null;

        //check X+1, Y for small house
        //if there is no house or the house there is too big, don't evolve it
        if (map.IsBuildingAt(X + 1, Y)) {

            House h = map.GetBuildingAt(X + 1, Y).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h1 = h;

        }

        //check X, Y+1 for small house
        if (map.IsBuildingAt(X, Y + 1)) {

            House h = map.GetBuildingAt(X, Y + 1).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h2 = h;

        }

        //check X+1, Y+1 for small house
        if (map.IsBuildingAt(X + 1, Y + 1)) {

            House h = map.GetBuildingAt(X + 1, Y + 1).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h3 = h;

        }

        //only proceed if all houses are same level
        if (h1 == null || h2 == null || h3 == null)
            return;

        if (h1.level != level || h2.level != level || h3.level != level)
            return;

        //combine arrays
        Water = ArrayFunctions.CombineArrays(Water, h1.Water, h2.Water, h3.Water);
        Food = ArrayFunctions.CombineArrays(Food, h1.Food, h2.Food, h3.Food);

        //combine stats
        foreach (Adult p in h1.Residents)
            Residents.Add(p);
        foreach (Adult p in h2.Residents)
            Residents.Add(p);
        foreach (Adult p in h3.Residents)
            Residents.Add(p);
        foreach (Adult p in Residents)
            Debug.Log(p.workNode);

        world.Demolish(h1.X, h1.Y);
        world.Demolish(h2.X, h2.Y);
        world.Demolish(h3.X, h3.Y);
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
        newHouse.Water = Water;
        newHouse.Savings = Savings;
        newHouse.WaterQual = WaterQual;
        newHouse.Food = Food;
        newHouse.Goods = Goods;
        newHouse.Culture = Culture;
        newHouse.VenueAccess = VenueAccess;
        newHouse.Diseased = Diseased;
        newHouse.MonthsLeftDiseased = MonthsLeftDiseased;
        newHouse.Residents = Residents;

		//USED TO TEST OVERCROWDING
		//if (newHouse.level > level)
		//	newHouse.ReceiveImmigrant(new Prole());

    }

    public void FreshHouse(Adult firstResident) {

        //Residents = res;
        Water = new int[(int)Quality.END];
        WaterQual = Quality.None;
        Food = new int[(int)FoodType.END];
        Goods = new int[(int)GoodType.END];
        VenueAccess = new Dictionary<string, int>();
        Residents = new List<Adult>();

		//CREATE NEW RESIDENT HAPPENS RIGHT HERE
        ReceiveImmigrant(firstResident);

    }

    public override void UponDestruction() {

        base.UponDestruction();
		scenario.RemoveHouseLevel(level - 1);

		if (changingHouse)
			return;

		//ONLY DO THIS IF HOUSE IS NOT EVOLVING OR DEVOLVING
		foreach (Adult mover in Residents) {

			//p.QuitWork();
			mover.personalSavings += Savings / Residents.Count;
			immigration.TryEmigrant(mover);
			population.RemoveProle(mover);

		}


    }

    public override void ReceiveItem(ItemOrder io) {

        if(io.type == ItemType.Food)
            Food[io.item] += io.amount;

        else if (io.type == ItemType.Good)
            Goods[io.item] += io.amount;


    }

	public override void ReceiveImmigrant(Adult p) {

		p.MoveIntoHouse(this);
		Residents.Add(p);
		//Debug.Log(p + " moved into " + this);
		if (Residents.Count > residentsMax)
			Debug.Log("Not enough room in " + name + " for " + p);
		Savings += p.personalSavings;
		p.personalSavings = 0;
		population.AddProle(p);		//add to prole list of town

	}

	void UpdateResidentsAge() {

		//iterate backwards to not have any problems with removing residents or children
		for(int i = Residents.Count - 1; i >= 0; i--) {

			Adult p = Residents[i];
			p.UpdateAge();

			for (int j = p.children.Count - 1; j >= 0; j--) {

				Child c = p.children[j];
				c.UpdateAge();
				if (c.markedForDeath) {
					//do if c is dead
					p.children.Remove(c);
					c.Kill();
				}
				else if (c.GrownUp) {

					ReceiveImmigrant(new Adult(c, p.laborPref));	//change to account for random labor pref based on social conditioning
					p.children.Remove(c);

				}

			}

			if (p.markedForDeath) {
				//do if p is about to die
				p.Kill();
			}

		}
		
	}

	public void CheckProleSpawn() {
		
		//spawn walker or laborseeker process
		for(int i = 0; i < Residents.Count && !ActiveRandomWalker; i++) {

			Adult res = Residents[i];
			if (res.workNode != null)
				continue;
			
		}

	}

    /*************************************
    HEALTH STATS
    *************************************/

    public float WaterModifier { get { return 1.6f - (float)WaterQual * .4f; } }
    public GameObject cholera;
    public bool Diseased { get; set; }
    public int Waste { get { return HouseSize; } }
    public int casualtyRiskMax { get { return (int)(20.0 * Difficulty.GetModifier()); } }
    public float CasualtyRisk { get; set; }
    public int diseaseLength = 12;
    public int MonthsLeftDiseased { get; set; }

    void SpreadDisease() {

        if (!Diseased)
            return;

        int roll = Random.Range(1, 100);

        if (CasualtyRisk == 0)
            roll = 100;

        IncreaseCasualtyRisk();
        MonthsLeftDiseased--;
        if (MonthsLeftDiseased == 0)
            Diseased = false;
        
    }

    public void StartDisease() {

        Diseased = true;
        MonthsLeftDiseased = diseaseLength;

    }

    void IncreaseCasualtyRisk() {

        if (CasualtyRisk < casualtyRiskMax)
            CasualtyRisk += casualtyRiskMax / 10.0f;

        if (CasualtyRisk > collapseRiskMax)
            CasualtyRisk = collapseRiskMax;

    }

    void ThrowWaste() {

        List<Node> roads = GetAdjRoadTiles();

        foreach(Node n in roads) {

            int x = n.x;
            int y = n.y;
            int mult = 1;

            if (Diseased)
                mult = 3;

            if(world.Map.cleanliness[x, y] < 100)
                world.Map.cleanliness[x, y] += Waste * mult;

            if (world.Map.cleanliness[x, y] > 100)
                world.Map.cleanliness[x, y] = 100;

        }

    }


    /*************************************
    WATER STATS
    *************************************/

    public int[] Water { get; set; }
    public Quality WaterQual { get; set; }
    public Quality waterQualNeeded;
    public Quality waterQualWanted;

    public int WaterMax { get { return HouseSize * 4; } }
    public int WaterNeeded(int q) { return WaterMax - Water[q]; }
    public int WaterNeeded(Quality q) { return WaterNeeded((int)q); }
    public int WaterToConsume { get { return HouseSize; } }

    public void AddWater(int num, Quality qual) {
        Water[(int)qual] += num;
        WaterQual = qual;
    }

    void ConsumeWater() {

        int consume = WaterToConsume;
        while (consume > 0 && WaterQual != 0) {

            //consume water from current quality level
            if (consume < Water[(int)WaterQual]) {
                Water[(int)WaterQual] -= consume;
                consume = 0;
            }
            else if (consume >= Water[(int)WaterQual]) {
                consume -= Water[(int)WaterQual];
                Water[(int)WaterQual] = 0;
            }

            //if quality level has 0 water or less, move quality level down
            if (Water[(int)WaterQual] <= 0) {
                Water[(int)WaterQual] = 0;
                WaterQual--;
            }
        }

    }

    /*************************************
    FOOD STATS
    *************************************/

    public int[] Food { get; set; }
    public int foodTypesNeeded;
    public int foodTypesWant;
	public bool WantsMoreFood { get { return NumOfFoods() < foodTypesWant; } }

	public int FoodMax { get { return 24 * FoodToConsume / foodTypesWant; } }	//amount of each type to store (for 2 years/24 months)
	public int FoodNeeded(int item) { return FoodMax - Food[item]; }			//amount needed per type
	public int FoodToConsume { get { return Residents.Count; } }	//amount of each type to consume

	public int NumOfFoods() {

        int s = 0;

        for (int b = 0; b < Food.Length; b++)
            if (Food[b] > 0)
                s++;

        return s;

    }

    void ConsumeFood() {

        for(int a = 0, b = 0; a < foodTypesNeeded && b < Food.Length; a++) {

			//amount of this type of food to consume
            int consume = FoodToConsume;

            for (; b < Food.Length && consume > 0; b++) {
                

                if (Food[b] >= consume) {
                    Food[b] -= consume;
                    consume = 0;
                }
                else {
                    consume -= Food[b];
                    Food[b] = 0;
                }

            }

        }
            
    }

    /*************************************
    GOODS STATS
    *************************************/

    public int[] Goods { get; set; }
	public GoodType[] goodsNeeded;
	public GoodType goodWanted = GoodType.END;

	public int GoodsMax { get { return 4 * HouseSize; } }
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

	public bool WantsMoreGoods { get {

			if (goodWanted == GoodType.END)
				return false;
			return Goods[(int)goodWanted] == 0;

		} }
	public bool MissingGoods { get {
			foreach (GoodType good in goodsNeeded) {
				
				if (Goods[(int)good] == 0)
					return true;

			}
			return false;
		} }

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

        if (Diseased)
            return "Infested with cholera";
        if (WantsBetterWater) {
            if (waterQualWanted == Quality.Poor)
                return "Needs water to evolve";
            else
                return "Needs filtered water to evolve";
        }
        if (WantsMoreFood) {
            if (foodTypesWant == 1)
                return "Needs 1 type of food to evolve";
            else
                return "Needs " + foodTypesWant + " types of food to evolve";
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
			case ItemType.Food:
				return WillBuyFood(item, amountStored);
			case ItemType.Good:
				return WillBuyGood(item, amountStored);
		}

		return null;

	}

	public ItemOrder WillBuyFood(int item, int amountStored) {

		//if you don't want anything, stop
		if (foodTypesWant == 0)
			return null;
		int amountMin = FoodMax / 6;
		int typesHave = NumOfFoods();
		//if you don't have this item and have the types you want, you don't want any
		if (Food[item] == 0 && typesHave == foodTypesWant)
			return null;
		//if you have more types than you need and you have this, don't buy any more types
		if (Food[item] > 0 && typesHave > foodTypesWant)
			return null;
		//if you have enough of this type, you don't want any more of this type
		if (Food[item] > amountMin && typesHave <= foodTypesWant)
			return null;

		int divisor = 1;
		//FIGURE OUT TO DECIDE HOW MUCH FOOD OF A TYPE THIS HOUSE WANTS
		//if want == need, divide by either
		if (foodTypesWant == foodTypesNeeded)
			divisor = foodTypesWant;
		else {
			//if we don't have any of this food, get as much that would take to evolve
			//	otherwise we want to fill it up to sustain the current house
			divisor = Food[item] == 0 ? foodTypesWant : foodTypesNeeded;
		}

		int delta = FoodMax / divisor - Food[item];

		//if we don't have that much food, sell as much as we can
		if (amountStored < delta)
			delta = amountStored;

		ItemOrder io = new ItemOrder(delta, item, ItemType.Food);

		//if house cannot afford, find the largest amount it can buy for the smallest price
		if (io.ExchangeValue() > Savings) {
			float priceOfOne = ResourcesDatabase.GetBasePrice(new ItemOrder(1, item, ItemType.Food));
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
			if((int)goodWanted == item)
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
