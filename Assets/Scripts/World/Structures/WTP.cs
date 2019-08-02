using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WTPSave : WorkplaceSave {

    public int WaterFill;

    public WTPSave(GameObject go) : base(go) {

        WTP w = go.GetComponent<WTP>();

        WaterFill = w.WaterFill;


    }

}

public class WTP : Workplace {

    [Header("Water Treatment Plant")]
    public GameObject water;
    public int WaterFill { get; set; }
    public float WaterBaseDepth { get { return -0.3f; } }
    public float WaterDepth { get { return 0.3f; } }
    public bool FullWater { get { return WaterFill >= maxAmount; } }
    public int fillPerDay;
    public int maxAmount;

    public override void Load(ObjSave o) {
        base.Load(o);

        WTPSave w = (WTPSave)o;
        WaterFill = w.WaterFill;

    }

    public override void DoEveryDay() {

        if (!ActiveRandomWalker) {

            if (Operational && FullWater) {
                SpawnRandomWalker();

                if(ActiveRandomWalker)
                    WaterFill = 0;
            }

            else if (!EnoughWorkers && ActiveBuilding)
                SpawnLaborSeeker();

        }

        if(!ActiveRandomWalker && Operational)
            FillWater();
        UpdateWaterModel();

    }

    void FillWater() {

        if (WaterFill < maxAmount)
            WaterFill += fillPerDay;

        if (WaterFill > maxAmount)
            WaterFill = maxAmount;

    }

    void UpdateWaterModel() {

        float newheight = water.transform.localPosition.y;
        newheight = (float)WaterFill / maxAmount * WaterDepth + WaterBaseDepth;
        Vector3 newvec = new Vector3(0, newheight, 0);
        water.transform.localPosition = newvec;

    }

}
