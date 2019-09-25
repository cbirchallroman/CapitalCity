using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FarmhouseSave : WorkplaceSave {

    public string CurrentlyStoring;
    public int Yield;

    public FarmhouseSave(GameObject go) : base(go) {

        Farmhouse f = go.GetComponent<Farmhouse>();

        CurrentlyStoring = f.CurrentlyStoring;
        Yield = f.Yield;

    }

}

public class Farmhouse : Workplace {
    
    public int TilesThatCanBeVisited { get { return tilesPerWorker * WorkerList.Count; } }
    public int tilesPerWorker = 5;
    public string CurrentlyStoring { get; set; }
    public int Yield { get; set; }

    public override void Load(ObjSave o) {

        base.Load(o);

        FarmhouseSave f = (FarmhouseSave)o;

        CurrentlyStoring = f.CurrentlyStoring;
        Yield = f.Yield;

    }

    public override void DoEveryMonth() {

        base.DoEveryMonth();

        if (!Operational)
            return;
        
        VisitPlots();

        if (Yield > 0 && !ActiveSmartWalker)
            ExportProduct();

    }

    void VisitPlots() {

        int numPlots = TilesThatCanBeVisited;

        for (int a = X - radiusOfInfluence; a < X + Sizex + radiusOfInfluence && numPlots > 0; a++)
            for (int b = Y - radiusOfInfluence; b < Y + Sizey + radiusOfInfluence; b++)
                if (world.IsBuildingAt(a, b) && !world.IsRoadAt(a, b)) {

                    if (world.GetBuildingAt(a, b) == this)
                        continue;
                    VisitBuilding(a, b);
                    numPlots--;

                }

    }

    public override void VisitBuilding(int a, int b) {

        Crop c = world.Map.GetBuildingAt(a, b).GetComponent<Crop>();
        if (c == null)
            return;

        if (c.planted && !c.ReadyForHarvest)
            return;

        if (!c.planted && c.startTimes.Contains(time.CurrentMonth))
            c.BeginGrow();

        if (c.ReadyForHarvest && Yield < stockpile)
            HarvestCrop(c);

    }

    void HarvestCrop(Crop c) {

        if (!string.IsNullOrEmpty(CurrentlyStoring) && !CurrentlyStoring.Equals(c.cropType))
            return;
        
        CurrentlyStoring = c.cropType;
        Yield += c.AmountToGrow;
        c.Harvest();
        
    }

    void ExportProduct() {

        ItemOrder io = new ItemOrder(Yield, CurrentlyStoring);
		
        SpawnGiverToStorage(io);
		if (!ActiveSmartWalker)
			return;
        CurrentlyStoring = null;
        Yield = 0;

		//if (!string.IsNullOrEmpty(byproduct))
		//    ByproductsMade += ByProductStorage;

	}

}
