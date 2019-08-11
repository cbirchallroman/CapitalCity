using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneratorSave : WorkplaceSave {

    public int ByproductsMade, Deterioration;
    public bool Producing;
    public int[] IngredientAmount;

    public GeneratorSave(GameObject go) : base(go) {

        Generator g = go.GetComponent<Generator>();
        Producing = g.Producing;
        ByproductsMade = g.ByproductsMade;
        Deterioration = g.Deterioration;
        IngredientAmount = g.IngredientsStored;

    }

}


public class Generator : Workplace {
    
    public int ByproductsStored { get { return stockpile / 4; } }
	[Header("Generator")]
	public string product;
    public string byproduct;
	
	public override bool Operational { get { return base.Operational && !BrokenDown; } }
	public virtual bool StartConditions { get { return HaveEnoughTotal(); } }
	public virtual bool ProductionComplete { get { return AmountStored >= stockpile; } }
	public virtual float PercentDone { get { return (int)(AmountStored / stockpile * 100); } }

	//PRODUCTIVITY
	public float BaseProductivity { get; set; }     //determined by machinery or lack thereof
	public float ActualProductivity { get { return BaseProductivity * WorkerEffectiveness * RelativeWorkingDay * MachineryPerformance; } }
	public int BaseProductionCycle { get { return Mathf.RoundToInt(ResourcesDatabase.GetBaseDays(product) / BaseProductivity * RelativeStockpile * RelativeWorkerStrength); } }    //time to produce product without taking variables into account
	public int ActualProductionCycle { get { return Mathf.RoundToInt(ResourcesDatabase.GetBaseDays(product) / ActualProductivity * RelativeStockpile * RelativeWorkerStrength); } }    //actual time to produce product
	public float SocialProductivity { get { return ProductivityController.GetAverageProductivityEverywhere(product); } }    //social average time to produce
	
	//PRODUCTION
	//	the number of hours that it take something to be produced assumes an average workday of 8 hr and 4 workers
	public float ProductPerWorker { get { return (float)stockpile / (BaseProductionCycle * BaseWorkingDay * BaseWorkers); } }   //amount of product produced by a worker each day
	public float ProductsPerDay { get { return ProductPerWorker * WorkingDay * NumWorkers() * MachineryPerformance; } }

	//RELATIVE STATS
	public float RelativeWorkingDay { get { return ((float)WorkingDay) / BaseWorkingDay; } }
	public float RelativeStockpile { get { return stockpile / 100f; } }
	public int RelativeProductivity { get { return (int)((ActualProductivity > 0 ? ActualProductivity : BaseProductivity) / SocialProductivity * 100); } }  //percent efficiency relative to social average time
	public float RelativeWorkerStrength { get { return (float)workersMax / BaseWorkers; } }

	//ECONOMICS
	public float ValueProduced { get { return ResourcesDatabase.GetBasePrice(product, stockpile); } }
    public float SurplusValue { get { return ValueProduced - WagesOverall * ActualProductionCycle / TimeController.DaysInAMonth; } }
    public float SuperProfits { get { return ValueProduced * (ActualProductivity - SocialProductivity) / SocialProductivity;} }

	//DELIVERY
	public bool DontSendItemsToGenerators { get; set; }

    //INGREDIENTS
    public int[] IngredientsPer100 { get; set; }
    public int[] IngredientsStored { get; set; }
    public string[] Ingredients { get; set; }

    //PRODUCTS
    public int ByproductsMade { get; set; }
    public bool Producing { get; set; }

	//DETERIORATION
	public Machine MachineData { get; set; }
	public MachineType machineryType = MachineType.END;
    public int MachineryValue { get { return DeteriorationPerCycle * 10; } }	//total amount of machinery in the generator; can last for 10 cycles before needing to be replaced
    public ResourceType MachineryResource { get; set; }		//determined by machinery
	public ResourceType Fuel { get; set; }		//determined by machinery
	public int Deterioration { get; set; }
    public int DeteriorationPerCycle { get { return MachineData.GetDeteriorationPerCycle(stockpile); } }		//amount of constant capital consumed each cycle
	public float MachineryPerformance { get { if (MachineData == null) return 1f; return 1f - (float)Deterioration / MachineryValue; } }	//what % of the machinery needs to be replaced
	public bool BrokenDown { get { return MachineryPerformance == 0; } }
	public int RepairsNeeded { get { return MachineryValue - Deterioration; } }

    public override void Load(ObjSave o) {
        base.Load(o);

		//LOAD MACHINERY BEFORE INGREDIENTS
		LoadMachinery();
		LoadIngredients();

		GeneratorSave g = (GeneratorSave)o;
        Producing = g.Producing;
        ByproductsMade = g.ByproductsMade;
        Deterioration = g.Deterioration;
        IngredientsStored = g.IngredientAmount;
		JoinProductivityLists();
		
	}

    public override void Activate() {

        base.Activate();

		//LOAD MACHINERY BEFORE INGREDIENTS
		LoadMachinery();
		LoadIngredients();
		
		JoinProductivityLists();

	}

	public void JoinProductivityLists() {

		JoinProductivityList(product);
		if(!string.IsNullOrEmpty(byproduct))
			JoinProductivityList(byproduct);

	}

	public void LeaveProductivityLists() {

		LeaveProductivityList(product);
		if (!string.IsNullOrEmpty(byproduct))
			LeaveProductivityList(byproduct);

	}

	public override float GetActualProductivity(string item) {

		if (!item.Equals(product) && !item.Equals(byproduct))
			Debug.LogError(item + " is not produced by " + this);

		return ActualProductivity;
		//return baseProductivity;

	}

	public override float GetAutomationValue(string item) {

		if (!item.Equals(product) && !item.Equals(byproduct))
			Debug.LogError(item + " is not produced by " + this);

		if (MachineData == null)
			return 0;

		return MachineData.ValueAddedToProduction(stockpile);
		//return baseProductivity;

	}

	public override void DoEveryDay() {

        base.DoEveryDay();

        if (this is Stable)
            return;

        //restart production if building is inactive or not sending out cart
        if (Operational && !ActiveSmartWalker) {

            if (Producing)
                ProductionTimer();

            else if (StartConditions)
                BeginProduction();

        }
        
        if (!Producing && !StartConditions && Operational && !ActiveSmartWalker && !ActiveRandomWalker)
            GetIngredients();

		//Debug.Log(RelativeEmployment + " " + BaseProductionCycle + " " + ActualProductionCycle);

    }

    public bool HaveEnough(int a) { return IngredientNeeded(a) <= 0; }
    public int IngredientNeeded(int a) { return IngredientPerStorage(a) - IngredientsStored[a] - ((int)AmountStored * IngredientsPer100[a] / 100); }
	public int IngredientPerStorage(int a) { return IngredientsPer100[a] * stockpile / 100; }

    public bool HaveEnoughTotal() {

        for (int a = 0; a < IngredientsPer100.Length; a++)
            if (!HaveEnough(a))
                return false;
		// is this needed?
            //else if (IngredientsStored[a] == 0)
            //    return false;
        return true;

    }

    public virtual void BeginProduction() {
        for (int a = 0; a < IngredientsPer100.Length; a++)
            IngredientsStored[a] -= IngredientPerStorage(a) - ((int)AmountStored * IngredientsPer100[a] / 100);
        Producing = true;
    }

    public virtual void ProductionTimer() {

        if (ProductionComplete && !ActiveSmartWalker)
            ExportProduct();

        //byproduct
        else if (!ActiveSmartWalker && ByproductsMade > 0 && !string.IsNullOrEmpty(byproduct))
            ExportByproduct();

        else
            AmountStored += ProductsPerDay;

        if (AmountStored > stockpile)
            AmountStored = stockpile;

    }

    public virtual void ExportProduct() {

        ItemOrder io = new ItemOrder(stockpile, product);

		//try to send carryer to other generator first; then try to storage
		if(SpawnGiverToGenerator(io) == null || DontSendItemsToGenerators)
			SpawnGiverToStorage(io);

		if (!ActiveSmartWalker)
			return;
        Producing = false;
        AmountStored -= stockpile;
        if(!string.IsNullOrEmpty(byproduct))
            ByproductsMade += ByproductsStored;

		//only deteriorate machinery once this production cycle is finished
        Deteriorate();

	}

	public void ExportByproduct() {

		ItemOrder io = new ItemOrder(ByproductsMade, byproduct);

		if (SpawnGiverToGenerator(io) == null || DontSendItemsToGenerators)
			SpawnGiverToStorage(io);
		
		if (!ActiveSmartWalker)
			return;
		ByproductsMade = 0;

	}

	void Deteriorate() {

		if (MachineData == null)
			return;

        Deterioration += DeteriorationPerCycle;
        if (Deterioration > MachineryValue)
            Deterioration = MachineryValue;

    }

    public void MaintainFactory(ItemOrder io) {

        if (io.type != ItemType.Resource)
            Debug.LogError("Tried to receive a non-resource for maintenance");
        if (io.item != (int)MachineryResource)
            Debug.LogError("Tried to receive " + (ResourceType)io.item + " instead of " + MachineryResource);
        Deterioration -= io.amount;

    }

    public override void OpenWindow() {
		
		OpenWindow(UIObjectDatabase.GetUIElement("GeneratorWindow"));

	}

    public override string GetDescription() {
        if (!ActiveBuilding)
            return "This building is closed.";
        if (!EnoughWorkers)
            return "This building is understaffed.";
        if (ActiveSmartWalker)
            return "This building's cart is transporting goods.";
        if (ProductionComplete)
            return "This building is waiting for a building to transport its goods to.";
        if (Producing)
            return "This building is producing " + product + "!";
        return "Production has ceased for now.";
    }

    void LoadIngredients() {

        string[] i = ResourcesDatabase.GetIngredients(product);

		//if this generator requires fuel to operate, add +1 to the length of the ingredients list
		int length = Fuel != ResourceType.END ? i.Length + 1 : i.Length;
        IngredientsPer100 = new int[length];
        Ingredients = new string[length];
		IngredientsStored = new int[length];
		int index = 0;

        foreach (string s in i) {
            
            string[] split = s.Split(' ');
            if (split.Length != 2)
                Debug.LogError("Bad info for " + product + " ingredient(s)");
            IngredientsPer100[index] = int.Parse(split[0]);
            Ingredients[index] = split[1];

            index++;

        }

		if(Fuel != ResourceType.END) {
			int fuelIndex = length - 1;
			Ingredients[fuelIndex] = Fuel + "";
		}

		if(Fuel != ResourceType.END)
			IngredientsPer100[index] = DeteriorationPerCycle;

	}

	void LoadMachinery() {
		
		if(machineryType == MachineType.END) {

			MachineData = null;
			MachineryResource = ResourceType.END;
			Fuel = ResourceType.END;
			BaseProductivity = 1;

		}
		else {

			MachineData = MachineDatabase.GetMachineData(machineryType);

			//CHANGE TO USE DEFINED HOURS CONTRIBUTED BY MACHINE
			if (MachineData == null)
				Debug.LogError("Data does not exist for " + machineryType + " machinery");


			MachineryResource = MachineData.material;
			Fuel = MachineData.fuel;
			//BaseProductivity = MachineData.improvement;
			int baseDays = ResourcesDatabase.GetBaseDays(product);
			//BaseProductivity = (float)(baseDays + MachineData.socialDays) / baseDays;
			BaseProductivity = (float)baseDays / MachineData.socialDays;
		}

	}

    public void GetIngredients() {

        for (int a = 0; a < IngredientsPer100.Length; a++)
            if (!HaveEnough(a)) {

                if (IngredientNeeded(a) == 0)
                    continue;

                ItemOrder io = new ItemOrder(IngredientNeeded(a), Ingredients[a]);

                SpawnGetterToStorage(io);

                return;

            }

    }

    public override void ReceiveItem(ItemOrder io) {

        for (int a = 0; a < IngredientsPer100.Length; a++)
            if (Ingredients[a] == io.GetItemName())
                IngredientsStored[a] += io.amount;
            else
                Debug.LogError(name + " erroneously received " + io.ToString());

    }

	public override void UponDestruction() {

		base.UponDestruction();

		LeaveProductivityLists();

	}

	public int NeedsIngredient(string item) {

		for (int index = 0; index < Ingredients.Length; index++)
			if (Ingredients[index].Equals(item))
				return index;
		return -1;

	}

}
