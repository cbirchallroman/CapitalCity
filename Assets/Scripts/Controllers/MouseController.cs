using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public ActionController actionController;
    public CameraController cameraController;
    public ModeController modeController;
    public MoneyController moneyController;
    public TimeController timeController;
    public TooltipController tooltipController;
    public WorldController worldController;
	public LayerMask mapLayer;
    public Material canDo;
	
	GameObject preview;
	List<GameObject> dragPreviewGameObjects;
	Node dragStartPosition, mouseCoords, prevCoords;

	public bool UIhover { get; set; }

    public void MouseOverUI(bool b) {
        UIhover = b;
    }

    // Use this for initialization
    void Start() {

        dragPreviewGameObjects = new List<GameObject>();
		dragStartPosition = new Node(0, 0);

	}

    public void Update() {

        Destroy(preview);

        if (MouseOnMap())
            MapManipulation();

    }

    public bool dragEnabled = true;
    public void EnableDrag(bool b) {

        dragEnabled = b;

    }

    public bool MouseOnMap() {

        Vector3 pos = Input.mousePosition;
        if (pos == new Vector3(-1, -1, -1))
            return false;

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;
		
        bool mouseHittingMap = Physics.Raycast(ray, out hit, Mathf.Infinity, mapLayer);

        if (mouseHittingMap) {

			mouseCoords = new Node(hit.point);
			Debug.Log(mouseCoords);
            AlignMouse();

        }

		return mouseHittingMap;

    }

    public void MapManipulation() {

        if (UIhover)
            return;

        //if there is a current action, try construction
        Action act = actionController.currentAction;

		if (act != null) {

            if (actionController.CanDrag(act) && dragEnabled)
                UpdateDragging(actionController.DoStraightLine(act));
            else
                UpdateDropping();

        }
		else {

			UpdateObjWindow();

		}

    }

	void UpdateObjWindow() {

		//only proceed with right click
		if (!InputController.GetPositiveInputDown("ClickObject"))
			return;

		World world = worldController.Map;

		//if there's a building here with a window, open it
		if(world.IsBuildingAt(mouseCoords.x, mouseCoords.y)) {

			Structure str = world.GetBuildingAt(mouseCoords).GetComponent<Structure>();

			if (str.openWindow) {
				str.OpenWindow();
				return;
			}

		}

		//otherwise display whatever walkers are here
		//worldController.PrintWalkersOnTile(mouseCoords.x, mouseCoords.y);

	}

    void UpdateDragging(bool line) {
		
        Action act = actionController.currentAction;
        StructureData str = StructureDatabase.GetData(act.What);
        int sizex = str != null ? str.sizex : 1;
        int sizey = str != null ? str.sizey : 1;

		// Start Drag
		if (Input.GetMouseButtonDown(0))
			dragStartPosition = mouseCoords;

        int start_x = dragStartPosition.x;
        int end_x = mouseCoords.x;
        int start_y = dragStartPosition.y;
        int end_y = mouseCoords.y;

        if (line) {
            int dist_x = Math.Abs(start_x - end_x);
            int dist_y = Math.Abs(start_y - end_y);

            if (dist_x > dist_y)
                end_y = start_y;
            else
                end_x = start_x;
        }
		
        //if buildings cannot fit evenly, reduce the end_x by their modulo
        if ((end_x - start_x) % sizex != 0)
			end_x -= (end_x - start_x) % sizex;
		
		//if buildings cannot fit evenly, reduce the end_y by their modulo
		if ((end_y - start_y) % sizey != 0)
			end_y -= (end_y - start_y) % sizey;

		// We may be dragging in the "wrong" direction, so flip things if needed.
		if (end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}
		if (end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		if (Input.GetMouseButton(0)) {

            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();

            // Display a preview of the drag area
            for (int x = start_x; x <= end_x; x+=sizex)
                for (int y = start_y; y <= end_y; y+=sizey)
                    locs.Add(new Node(x, y), act);

            actionController.actionLocations = locs;

        }
        else
            DropPreview();

		// Clean up old drag previews
		ClearDragPreviews();

		//New previews
        DragPreview();

        // End Drag
        if (Input.GetMouseButtonUp(0))
            PerformSelectedActions();

        //UpdateTooltip();

    }

    void UpdateDropping() {

        DropPreview();

        Action act = actionController.currentAction;

        //UPDATE TOOLTIP
        //UpdateTooltip();

        // When mouse is clicked, drop a single building
        if (Input.GetMouseButtonUp(0)) {

            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();

            locs.Add(mouseCoords, act);

            actionController.actionLocations = locs;
            PerformSelectedActions();

        }
		
    }

    void DropPreview() {

        //grab preview object from actioncontroller
        preview = actionController.GetPreview(actionController.currentAction);
		Transform prevTrans = preview.transform;

        //if the object is grabbed, move it to the mouse coords
        if (preview != null) {

			//set position including elevation
			Vector3 pos = mouseCoords.GetVector3();
			pos.y = worldController.GetObjectFloat((int)pos.x, (int)pos.z);
			prevTrans.position = pos;

            //if action is a strucutre, align it
            Action act = actionController.currentAction;
            if (act.Do == "place") {

                StructureData data = StructureDatabase.GetData(act.What);

                int sizex = data.sizex;
                int sizey = data.sizey;
                float alignx = data.Alignx;
                float aligny = data.Aligny;

                //switch size dimensions if building is rotated
                if (actionController.buildingRotation % 180 != 0) {
                    int tempszx = sizex;
                    int tempszy = sizey;
                    sizex = tempszy;
                    sizey = tempszx;

                    float tempalignx = alignx;
                    float tempaligny = aligny;
                    alignx = tempaligny;
                    aligny = tempalignx;

                }

				//BELOW FOR WHEN WE WANT THE CAMERA FACING BUILDINGS
				if (Settings.oldGraphics) {
					if(sizex == sizey && !data.hasRoadTiles && !data.hasWaterTiles)
						prevTrans.eulerAngles = cameraController.transform.transform.eulerAngles;
					else
						prevTrans.eulerAngles = new Vector3(0, actionController.buildingRotation, 0);
				}
				//FOR NORMAL 3D
				else
					prevTrans.eulerAngles = new Vector3(0, actionController.buildingRotation, 0);

				prevTrans.position += new Vector3(alignx, 0, aligny);

            }

			Material mat = new Material(canDo);

            //if cannot do, make it red
            if (actionController.CanDo(act, mouseCoords.x, mouseCoords.y))
				mat.color = new Color(1, 1, 1, .5f);
            else
				mat.color = new Color(1, 0, 0, .5f);
			
            foreach (Transform child in prevTrans) {

                Material[] mats = child.GetComponent<MeshRenderer>().materials;
                for (int a = 0; a < mats.Length; a++)
                    mats[a] = mat;
                child.GetComponent<MeshRenderer>().materials = mats;

            }
			
        }

    }

    void DragPreview() {

        Dictionary<Node, Action> actionLocations = actionController.actionLocations;

		foreach (Node n in actionLocations.Keys) {

			int x = n.x;
			int y = n.y;

			Action act = actionLocations[n];

			//display preview of action at tile
			GameObject go = actionController.GetPreview(act);
			go.transform.position = new Vector3(x, worldController.GetObjectFloat(x, y), y);	//don't for
			dragPreviewGameObjects.Add(go);

			//align preview
			StructureData data = StructureDatabase.GetData(act.What);
			if(data != null) {

				float alignx = data.Alignx;
				float aligny = data.Aligny;
				go.transform.position += new Vector3(alignx, 0, aligny);
			}

			//change color of preview
			Material mat = new Material(canDo);

			//if can do, make it white
			if (actionController.CanDo(act, x, y) && act.Do != "demolish")
				mat.color = new Color(1, 1, 1, .5f);
			//else, if it's paint, make it invisible
			else if (act.Do == "paint")
                go.SetActive(false);
			//else if we're demolishing but we can't here
			else if (act.Do == "demolish" && !actionController.CanDo(act, x, y))
				go.SetActive(false);
			//else, make it red
			else
				mat.color = new Color(1, 0, 0, .5f);

            foreach (Transform child in go.transform) {

                Material[] mats = child.GetComponent<MeshRenderer>().materials;
                for (int a = 0; a < mats.Length; a++)
                    mats[a] = mat;
                child.GetComponent<MeshRenderer>().materials = mats;

            }

        }

    }

    void AlignMouse() {

		//containers for x and y coords
        int x = mouseCoords.x;
        int y = mouseCoords.y;

        //default mouse alignment is 1
        int sizex = 1;
        int sizey = 1;

        //if action is placing a structure, check its size
        Action act = actionController.currentAction;
        if (act == null)
            return;
        if (act.Do == "place")
        {
            StructureData data = StructureDatabase.GetData(act.What);

            sizex = data.sizex;
            sizey = data.sizey;

            //switch size dimensions if building is rotated
            if (actionController.buildingRotation % 180 != 0)
            {
                int tempszx = sizex;
                int tempszy = sizey;
                sizex = tempszy;
                sizey = tempszx;
            }
        }

        Vector3 rotation = cameraController.transform.eulerAngles;

        //ADJUST ALIGNMENT OF BUILDING IF MAPVIEW IS ROTATED
        if (rotation.y == 90 || rotation.y == 180)
            y -= sizey - 1;

        if (rotation.y == 270 || rotation.y == 180)
            x -= sizex - 1;

        //KEEP BUILDING PREVIEW WITHIN WORLD LIMITS
        World world = worldController.Map;
        if (world.OutOfBounds(x, y, sizex, sizey))
        {
            if (x + sizex > world.size.x || x < 0)
                x = prevCoords.x;
            if (y + sizey > world.size.y || y < 0)
                y = prevCoords.y;
        }

        //UPDATE MOUSE COORDS
        mouseCoords = new Node(x, y);
		
		//UPDATE PREV COORDS
		prevCoords = mouseCoords;

    }

    void ClearDragPreviews()
    {
        while (dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }
    }

    void PerformSelectedActions()
    {

        actionController.PerformActions();
        ClearDragPreviews();
        dragStartPosition = mouseCoords;

    }

}
