using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CropSave : StructureSave
{

    public int GrowTimer;

    public CropSave(GameObject go) : base(go) {

        Crop c = go.GetComponent<Crop>();
        GrowTimer = c.GrowTimer;

    }

}

public class Crop : Structure {

    [Header("Crop")]
    public List<Month> startTimes;
    public GameObject plant;
    public int BaseDaysToGrow { get; set; }
    public int amountToGrowPerFertility = 2;
    public int AmountToGrow { get { return amountToGrowPerFertility * (world.Map.Fertility(X, Y) + 1); } }
    public string cropType = "Grain";
    public int GrowTimer { get; set; }
    public bool planted;
    public bool ReadyForHarvest { get { return planted && GrowTimer == 0; } }
    public float PercentDone { get { return (int)((float)(ActualDaysToGrow - GrowTimer) / ActualDaysToGrow * 100); } }
	public float Productivity { get { return (float)BaseDaysToGrow / ActualDaysToGrow; } }
	public int ActualDaysToGrow { get { return BaseDaysToGrow / amountToGrowPerFertility; } }
    public float startHeight = -0.36f;

	public override void Load(ObjSave o) {

		base.Load(o);

		BaseDaysToGrow = ResourcesDatabase.GetBaseDays(cropType);

		CropSave c = (CropSave)o;
		GrowTimer = c.GrowTimer;
		JoinProductivityList(cropType);

	}

	public override void Activate() {

        base.Activate();

		BaseDaysToGrow = ResourcesDatabase.GetBaseDays(cropType);

		JoinProductivityList(cropType);

	}

	public override float GetActualProductivity(string item) {

		return Productivity;

	}

	public override float GetAutomationValue(string item) {

		return 0;

	}

	public override void DoEveryDay() {

        if (planted && !ReadyForHarvest)
            GrowTimer--;
        else if (ReadyForHarvest)
            Debug.Log(name + " ready for harvest!");

        float newheight = (100.0f - PercentDone) / 100 * startHeight;
        plant.transform.localPosition = new Vector3(0, newheight, 0);

    }

    public void BeginGrow() {

        planted = true;
        GrowTimer = BaseDaysToGrow;
        plant.SetActive(true);
        plant.transform.localPosition = new Vector3(0, startHeight, 0);

    }

    public void Harvest() {

        planted = false;
        plant.SetActive(false);

    }

	public override void UponDestruction() {

		base.UponDestruction();


	}

}
