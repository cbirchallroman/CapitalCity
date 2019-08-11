using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour {

    public WorldController world;
    ModeController modeController;
    MoneyController moneyController;
    public MenuController constructionController;
    public MenuController editController;
	public ActionSelecterController selecterController;

    public Action currentAction { get; set; }
    public int timeToUndoMax;
    public int TimeToUndo { get; set; }

    public Dictionary<Node, Action> actionLocations = new Dictionary<Node, Action>();
    public Dictionary<Node, Action> undoActions = new Dictionary<Node, Action>();

    public Button undo;

    private void Start() {

        currentAction = null;
        modeController = world.modeController;
        moneyController = world.money;

    }

    float timeDelta = 0;

    public void Update() {

        timeDelta += Time.deltaTime;
        if (timeDelta >= 1) {

            timeDelta = 0;
            UndoCounter();

        }
            

        if (Input.GetMouseButton(1)) {
            currentAction = null;
			TooltipController.SetText("");
            constructionController.CloseMenu();
            editController.CloseMenu();
        }

        if (Input.GetKeyDown("space"))
            RotateStructure(90);

        if (currentAction != null) {
            UpdateTooltip();
        }

        
    }

    public float buildingRotation { get; set; }

    public void RotateStructure(float deg) {
		if (currentAction == null)
			return;
		if (currentAction.Do != "place")
			return;
		StructureData data = StructureDatabase.GetData(currentAction.What);
		//if (data.Sizex == data.Sizey && !data.HasWaterTiles && !data.HasRoadTiles)
		//	return;
		buildingRotation += deg;
        if (buildingRotation >= 360)
            buildingRotation = 0;
    }

    public void StopAction() {

        currentAction = null;
		TooltipController.SetText("");
        constructionController.CloseMenu();
        editController.CloseMenu();

    }

    //check if an action is possible
    public bool CanDo(Action act, int x, int y) {

        string d = act.Do;
        string w = act.What;

        if (d == "paint")
            return world.CanPaintTerrain(w, x, y);

        else if (w == "Fence" && world.Map.IsUnblockedRoadAt(x, y))
            return true;

        else if (d == "place")
            return world.CanSpawnStructure(w, x, y, buildingRotation);

        else if (d == "demolish" || d == "unplace")
            return world.Map.GetBuildingAt(x, y) != null;

        else if (d == "rebuild")
            return true;

        return false;
    }

    //do an action
    public void Do(Action act, int x, int y) {

        string d = act.Do;
        string w = act.What;

		//NON-BUILDING PLACEMENT COMMANDS
		if (d == "open")
			selecterController.ShowActions(w);

		else if (d == "paint")
			world.PaintTerrain(w, x, y);

		else if (d == "demolish" || d == "unplace")
			world.Demolish(x, y);

		else if (d == "rebuild")
			world.LoadMapObject(act.data);

		//EXCEPTIONS TO BUILDING PLACEMENT
		//if we're building a fence and there's a plain road here, build a gate instead
		if (w == "Fence" && world.Map.IsUnblockedRoadAt(x, y)) {

			//buildingroadblock = true;
			world.Demolish(x, y);

			if (world.Map.IsUnblockedRoadAt(x - 1, y) || world.Map.IsUnblockedRoadAt(x + 1, y))
				buildingRotation = 90;

			world.SpawnStructure("FenceGate", x, y, buildingRotation);
			return;	//don't keep going

		}

		//BASIC BUILDING PLACEMENT
		//switch to 'if' and not 'else if' because not mutually exclusive with placing a road
		if (d == "place")
			world.SpawnStructure(w, x, y, buildingRotation);

    }

    public void PerformActions(Dictionary<Node, Action> acts) {

        undoActions = new Dictionary<Node, Action>();

        foreach (Node n in acts.Keys) {

            int x = n.x;
            int y = n.y;

            Action act = acts[n];

            if (CanDo(act, x, y)) {

                if (modeController.currentMode == Mode.Construct && act.Do == "place")
                    moneyController.SpendOnConstruction(StructureDatabase.GetModifiedCost(act.What));

				else if(act.Do == "unplace")
                    moneyController.Money += StructureDatabase.GetModifiedCost(act.What);

				undoActions.Add(n, ReverseAction(act, n));
				Do(act, x, y);

			}

        }

    }

    public void PerformActions() {

        PerformActions(actionLocations);
        actionLocations = new Dictionary<Node, Action>();

    }

    public void UndoCounter() {

        if (undoActions.Count > 0)
            TimeToUndo--;

        else
            TimeToUndo = timeToUndoMax;

        if (TimeToUndo <= 0)
            undoActions = new Dictionary<Node, Action>();

        //undo.interactable = undoActions.Count > 0;

    }

    public void UndoActions() {
        
        PerformActions(undoActions);
        undoActions = new Dictionary<Node, Action>();

    }

	public int GetConstructCost(Dictionary<Node, Action> acts, Action a) {

		int count = acts.Count;

		int singleCost = StructureDatabase.GetModifiedCost(a.What);

		if (count == 0)
			return singleCost;

		foreach (Node n in acts.Keys)
			if (!CanDo(acts[n], n.x, n.y))
				count--;
		return singleCost * count;

    }

    void UpdateTooltip() {

		int cost = 0;

        if (currentAction.Do == "paint") {
			TooltipController.SetText(currentAction.What);
			return;
		}

        else if (modeController.currentMode == Mode.Construct && currentAction.Do == "place")
            cost = GetConstructCost(actionLocations, currentAction);

		TooltipController.SetText(MoneyController.symbol + "" + cost);

		TooltipController.SetColor(moneyController.Money >= cost ? Color.white : Color.red);

	}

    //get preview object for an action
    public GameObject GetPreview(Action act) {

        if (act.Do == "paint" || act.Do == "demolish") {

            GameObject go = (GameObject)Instantiate(Resources.Load("Previews/Hover"));
            
            go.name = "preview";
            go.transform.SetParent(world.transform, true);

            return go;

        }

        if(act.Do == "place") {

            Object o = Resources.Load("Previews/" + act.What);
            if (o == null)
                o = Resources.Load("Structures/" + act.What);

            GameObject go = (GameObject)Instantiate(o);

            go.name = "preview";
            go.transform.SetParent(world.transform, true);
            go.tag = "Untagged";

            Structure s = go.GetComponent<Structure>();
            if(s != null)
            {
                s.IsPreview = true;
                Destroy(s);
            }
            

            return go;

        }

        return null;
    }

    //check if action can be dragged
    public bool CanDrag(Action act) {

        if (act == null)
            return false;

        if (act.Do == "paint" || act.Do == "demolish")
            return true;

        else if(act.Do == "place") {

            StructureData data = StructureDatabase.GetData(act.What);
            return data.canDrag;

        }

        return false;

    }

	public bool DoStraightLine(Action act) {

		if (!CanDrag(act))
			return false;

		else if (act.Do == "place") {

			StructureData data = StructureDatabase.GetData(act.What);
			return data.straightLineDrag;

		}

		return false;

	}

    public void SetAction(string s) {

        SetAction(new Action(s));

    }

    public void SetAction(Action a) {

        //if (Time.timeScale == 0 && modeController.currentMode != Mode.Edit)
        //    return;
        currentAction = a;
        buildingRotation = 0;

    }

    public Action ReverseAction(Action a, Node n) {

        if (a.Do == "place")
            return new Action("unplace", a.What);

        else if (a.Do == "demolish")
            return new Action("rebuild", world.Map.GetBuildingAt(n));

        return null;

    }
    
}

[System.Serializable]
public class Action {

    public string Do;
    public string What;
    public StructureSave data;

    public Action(string a, string b) {
        Do = a;
        What = b;
    }

    public Action(string a, GameObject str) {

        Do = a;

        if (str.GetComponent<StorageBuilding>() != null)
            data = new StorageBuildingSave(str);

        else if (str.GetComponent<Stable>() != null)
            data = new StableSave(str);

		else if (str.GetComponent<Crop>() != null)
			data = new CropSave(str);

		else if (str.GetComponent<Generator>() != null)
            data = new GeneratorSave(str);

        else if (str.GetComponent<WTP>() != null)
            data = new WTPSave(str);

		else if (str.GetComponent<Canal>() != null)
			data = new CanalSave(str);

		else if (str.GetComponent<House>() != null)
            data = new HouseSave(str.gameObject);

		else if (str.GetComponent<Farmhouse>() != null)
			data = new FarmhouseSave(str);

		else if (str.GetComponent<Workplace>() != null)
            data = new WorkplaceSave(str.gameObject);

        else if (str.GetComponent<Structure>() != null)
            data = new StructureSave(str.gameObject);

    }

    public Action(string a) {

        string[] s = a.Split(' ');
        if (s.Length != 2)
            return;
        Do = s[0];
        What = s[1];

    }

    public override string ToString() {
        return Do + " " + What;
    }

}