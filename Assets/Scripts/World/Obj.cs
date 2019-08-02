using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjSave {

    public Node location;
    public Float3d position, rotation;
    public string resourcePath, name, tag;
	public int model_num, model_rot;

    public ObjSave(GameObject go) {
        Vector3 pos = go.transform.position;
        Vector3 rot = go.transform.eulerAngles;

        position = new Float3d(pos);
        rotation = new Float3d(rot);

        tag = go.tag;
        name = go.name;

        Obj mo = go.GetComponent<Obj>();

        location = new Node(mo.X, mo.Y);

        resourcePath = mo.resourcePath;

		if (mo.randomModel != null) {
			model_num = mo.randomModel.Model;
			model_rot = mo.randomModel.Rotation;
		}

    }

}

public class Obj : MonoBehaviour {

    [Header("Object")]
    public string DisplayName;
    public string defaultDesc = "This is an object.";
    public string resourcePath;
	public int radiusOfInfluence;
	public bool openWindow = true;
	public RandomModel randomModel;
    //public bool rightclick;
	public int X { get; set; }
    public int Y { get; set; }
    
    public WorldController world { get; set; }
    public ModeController mode { get; set; }
    public TimeController time { get; set; }
    public MoneyController money { get; set; }
    public ImmigrationController immigration { get; set; }
    public TradeController trade { get; set; }
    public ScenarioController scenario { get; set; }
    public NotificationController notifications { get; set; }
	public PopulationController population { get; set; }
	public Pathfinder pathfinder { get; set; }

	bool WindowOpen;

    public virtual void VisitBuilding(int a, int b) {

        

    }

    public void Awake() {

        if (GetComponent<Obj>() != this)
            Debug.LogError("This has multiple obj components!");

	}

    //ADD TO SAVE
    public float TimeDelta { get; set; }

    public virtual void Activate() {

        LoadControllers();
		if (randomModel != null)
			randomModel.NewModel();


	}

    public virtual void Load(ObjSave o) {

        transform.position = o.position.GetVector3();
        transform.eulerAngles = o.rotation.GetVector3();

        LoadControllers();

        X = o.location.x;
        Y = o.location.y;
        tag = o.tag;
        name = o.name;
        //rightclick = o.rightclick;

		if (randomModel != null)
			randomModel.LoadModel(o.model_num, o.model_rot);

	}

    public void LoadControllers() {

        mode = world.modeController;
        time = world.timeController;
        immigration = world.immigration;
        money = world.money;
        trade = world.trade;
        notifications = world.notifications;
		scenario = world.scenario;
		population = world.population;
		pathfinder = new Pathfinder(world.Map);

	}

  //  private void OnMouseOver() {
		

		//if (InputController.GetPositiveInputDown("ClickObject"))
		//	Debug.Log("click");
		//if (InputController.GetPositiveInputDown("ClickObject") && !WindowOpen) {
		//	OpenWindow();

  //      }

  //  }

    public void OpenWindow(GameObject g) {

		if (!openWindow)
			return;

		if (WindowOpen)
			return;

		//if (world.timeController.IsPaused)
        //    return;

        if (world.mouse.actionController.currentAction != null)
            return;

		//if (world.objWindow != null)
		//    world.objWindow.Close();

		//instantiate object
		GameObject go = Instantiate(g);
        go.transform.SetParent(world.objectMenus.transform);
        go.transform.SetAsLastSibling();
        go.name = "ObjWindow";

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.localScale = new Vector3(1, 1, 1);

        //window vars
        ObjWindow objWindow = go.GetComponent<ObjWindow>();
        objWindow.obj = this;
        objWindow.WorldController = world;
        //world.objWindow = objWindow;

        //open window
        objWindow.Open();

		WindowOpen = true;

    }

    public virtual void OpenWindow() {

        OpenWindow(UIObjectDatabase.GetUIElement("ObjWindow"));

    }

	public void CloseWindow() {

		WindowOpen = false;

	}

    public SimplePriorityQueue<Structure, float> FindClosestStructureOfType(string type) {
		
        GameObject[] objs = GameObject.FindGameObjectsWithTag(type);

        Node pos = new Node(this);

        SimplePriorityQueue<Structure, float> queue = new SimplePriorityQueue<Structure, float>();
        foreach (GameObject go in objs) {

            Structure s = go.GetComponent<Structure>();

            if (s == null)
                Debug.LogError(go.name + " is not a structure");

            float distance = pos.DistanceTo(new Node(s));
            queue.Enqueue(s, distance);

        }

        return queue;

    }

    public virtual string GetDescription() {

        return defaultDesc;

    }

}
