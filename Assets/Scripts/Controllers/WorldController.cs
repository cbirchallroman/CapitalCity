using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulThings;

public class WorldController : MonoBehaviour {

    public Camera objViewCamera;
	public MeshCombiner meshCombiner;
    public WorldGenerator mapGenerator;
    public GameObject objectMenus;
    public GameObject structures;
    public GameObject walkers;
    public GameObject followers;

    [Header("Controllers")]
    public ActionSelecterController actionSelecter;
    public CameraController cameraController;
    public DiplomacyController diplomacy;
    public ImmigrationController immigration;
    public MouseController mouse;
    public ModeController modeController;
    public MoneyController money;
    public NotificationController notifications;
	public PopulationController population;
	public ResearchController research;
    public ScenarioController scenario;
    public TileMap tilemap;
    public TimeController timeController;
    public TradeController trade;

    public World Map { get; set; }
    public ObjWindow objWindow { get; set; }

    public bool PlaceOnRoads { get; set; }

	List<Walker>[,] WalkerGrid;

    private void Start() {
        Enums.LoadDictionaries();

        BasicWorldSave save = SaveLoadMap.GetGameData();

        //load world if it exists
        if (save != null) {

            if (save is WorldProgressSave)
                LoadSavedGame((WorldProgressSave)save);
            else
                LoadScenario(save);

        }
        else
            CreateWorld();

        GenerateWorld();

    }

    public void CreateWorld() {

        Node size = SaveLoadMap.newWorldSize;
        int szx = size.x;
        int szy = size.y;
        
        Map = new World(size);
        Map.terrain = mapGenerator.GetRandomTerrain(size);
		Map.elevation = mapGenerator.GetRandomElevation(size);
		//Map.elevation = new float[szx, szy];

		//until we've successfully placed the map entrance/exit, keep trying
		bool success = false;
		do { success = CreateMapEntrance(szx, szy); } while (!success);	

        money.Money = 10000;
        
        actionSelecter.FreshActions();

        if (notifications != null)
            notifications.FreshEvents();
        ProductivityController.CreateProductivities();
		ResourcesDatabase.CreateWhitelist();
		money.FreshStartingQuarter((int)timeController.CurrentSeason, timeController.CurrentYear);
		timeController.finances.LoadFinancialReports();

	}

	bool CreateMapEntrance(int szx, int szy) {

		//try to find a place for the mapentrance until it is found
		Node entranceSpot = new Node(0, Random.Range(2, szx - 2));
		Structure m1 = SpawnStructure("MapEntrance", entranceSpot.x, entranceSpot.y, 0);

		if (m1 == null)
			return false;
		
		//make roads out from the mapentrance as far as possible
		for(int i = 1; i < 11; i++) {

			if (SpawnStructure("Road", i, entranceSpot.y, 0) == null)
				break;

		}

		cameraController.MoveCameraTo(m1.X, m1.Y);
		return true;

	}

	GameObject[,] tiles;

    public void GenerateWorld() {

        GameObject worldObj = new GameObject();
        worldObj.name = "WorldMap";

		//List<GameObject> tiles = new List<GameObject>();
		tiles = new GameObject[Map.size.x, Map.size.y];

        for (int a = 0; a < Map.size.x; a++) {

            GameObject row = new GameObject();
            row.transform.parent = worldObj.transform;
            row.name = "Row_" + a;

            for (int b = 0; b < Map.size.y; b++) {

				GameObject tile = GenerateTile(a, b, row);
				tiles[a, b] = tile;

            }

        }

		meshCombiner.CreateTilemapMesh(tiles);

		//actionSelecter.LoadActionButtons();
		actionSelecter.LoadActionEnablers();

        ProductivityController.LoadProductivityLists();
        ProductivityController.LoadCompetitors(diplomacy.cities);
        ProductivityController.UpdateProductivities();

		immigration.LoadImmigrantData();

		WalkerGrid = Map.size.CreateArrayOfSize < List<Walker> > ();

	}

    public GameObject GenerateTile(int x, int y, GameObject row) {

		//create new tile
		GameObject newTile = SpawnObject("Tiles", (Terrain)Map.terrain[x, y] + "", x, y);
        newTile.transform.parent = row.transform;
        newTile.name = "Tile_" + x + "_" + y;

		return newTile;

    }

    public void LoadScenario(BasicWorldSave w) {

        Map = w.world;
        money.Load(w.money);
        actionSelecter.Load(w.actionSelecter);
        diplomacy.Load(w.diplomacy);
        scenario.Load(w.scenario);

        //GO THROUGH LISTS OF OBJECTS AND ACTIVATE THEM USING THE LOADMAPOBJECT() FUNCTION
        //structures
        foreach (ObjSave save in w.structures)
            LoadMapObject(save).transform.parent = structures.transform;
		
        if (notifications != null)
            notifications.FreshEvents();

		//load whitelist for items
		ResourcesDatabase.LoadWhitelist(w.Whitelist);

    }

    public void LoadSavedGame(WorldProgressSave w) {

        Map = w.world;
        timeController.Load(w.time);
		population.Load(w.population);
        money.Load(w.money);
        cameraController.Load(w.camera);
        actionSelecter.Load(w.actionSelecter);
        immigration.Load(w.immigration);
        trade.Load(w.trade);
        diplomacy.Load(w.diplomacy);
        scenario.Load(w.scenario);
		research.Load(w.research);

		ProductivityController.LoadProductivities(w.productivities, w.automation);
		notifications.LoadEvents(w.Events);

		//load whitelist for items
		ResourcesDatabase.LoadWhitelist(w.Whitelist);

		//GO THROUGH LISTS OF OBJECTS AND ACTIVATE THEM USING THE LOADMAPOBJECT() FUNCTION
		//structures
		foreach (ObjSave save in w.structures)
            LoadMapObject(save).transform.parent = structures.transform;
		foreach (ObjSave save in w.jobcentres)
			LoadMapObject(save).transform.parent = structures.transform;
		foreach (ObjSave save in w.workplaces)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.storagebuildings)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.generators)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.stables)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.houses)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.wtps)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.canals)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.crops)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.farmhouses)
            LoadMapObject(save).transform.parent = structures.transform;

        //walkers
        foreach (ObjSave save in w.animals)
            LoadMapObject(save).transform.parent = walkers.transform;
        foreach (ObjSave save in w.walkers)
            LoadMapObject(save).transform.parent = walkers.transform;

    }

    public GameObject LoadMapObject(ObjSave save) {

        //load object
        GameObject go = Instantiate(Resources.Load<GameObject>(save.resourcePath));

        //activate object
        Obj o = go.GetComponent<Obj>();
        o.world = this;
        o.Load(save);

        return go;
    }


    public bool CanSpawnStructure(string type, int x, int y, float buildingRotation) {

        if (!StructureDatabase.HasData(type))
            Debug.LogError("Structure databse does not contain " + type);
        StructureData data = StructureDatabase.GetData(type);

        int sizex = data.sizex;
        int sizey = data.sizey;

        //switch size dimensions if building is rotated
        if (buildingRotation % 180 != 0) {
            int tempszx = sizex;
            int tempszy = sizey;
            sizex = tempszy;
            sizey = tempszx;
        }

		float elevation = Map.elevation[x, y];

        for (int a = x; a < x + sizex; a++)
            for (int b = y; b < y + sizey; b++) {

                //coordinates within array
                int m = a - x;
                int n = b - y;

				if (Map.OutOfBounds(a, b))
					return false;

				//if building has road tiles beneath
				else if (data.hasRoadTiles) {

					int[,] roads = ArrayFunctions.RotatedArray(data.roadTiles, buildingRotation);

					//if there should be a road at this tile
					if (roads[m, n] == 1) {

						//if no structure at all, return false
						if (string.IsNullOrEmpty(Map.structures[a, b]))
							return false;

						//else if there's not a road here or it does not contain "Road", return false
						else if (Map.roads[a, b] != 2 || !Map.GetBuildingNameAt(a, b).Contains("Road_"))
							return false;

					}

				}

				//else if there's a structure on this tile when there shouldn't be
				else if (!string.IsNullOrEmpty(Map.structures[a, b])) {
					return false;
				}

				//else if there's a walker on this tile (make sure that walker grid exists before checking for a list on it)
				else if (WalkerGrid != null) {
					if (WalkerGrid[a, b] != null && !type.Contains("House") && !type.Contains("Rubble"))
						return false;
				}

				//else if the elevation is different
				if (Map.elevation[a, b] != elevation)
					return false;
				
				if (data.hasWaterTiles) {

                    int[,] water = ArrayFunctions.RotatedArray(data.waterTiles, buildingRotation);

                    if (Map.terrain[a, b] != (int)Terrain.Water && water[m, n] == 0 || Map.terrain[a, b] == (int)Terrain.Water && water[m, n] != 0)
                        return false;

                }

                //else return false if there's water here and there shouldn't be
                else if (Map.terrain[a, b] == (int)Terrain.Water)
                    return false;

                

            }

        if (data.buildNearWater)
            if (!Map.IsNearWater(x, y, sizex, sizex))
                return false;

        if (!string.IsNullOrEmpty(data.placeOnTerrain))
            if (!Map.IsOnTile(x, y, sizex, sizey, Enums.terrainDict[data.placeOnTerrain]))
                return false;

        if (!string.IsNullOrEmpty(data.placeNearby))
            if (!Map.IsNearStructure(x, y, sizex, sizey, data.placeNearby))
                return false;

        return true;
    }

    //place a building
    public Structure SpawnStructure(string type, int x, int y, float buildingRotation) {

        //don't do if not possible
        if (!CanSpawnStructure(type, x, y, buildingRotation))
            return null;

        //retrieve data about structure
        StructureData data = StructureDatabase.GetData(type);

        int sizex = data.sizex;
        int sizey = data.sizey;
        float alignx = data.Alignx;
        float aligny = data.Aligny;

        //if placing a mapentrance or mapexit, remove existing one
        if (data.builtOnce) {

            //find object
            GameObject g = GameObject.Find(type);

            //only try to delete it if it exists
            if (g != null) {

                Structure m = g.GetComponent<Structure>();
                Demolish(m.X, m.Y);

            }

        }

        //destroy roads beneath area if they're there
        else if (data.hasRoadTiles) {

            for (int a = x; a < sizex + x; a++) {
                for (int b = y; b < sizey + y; b++) {
                    PlaceOnRoads = true;
                    Demolish(x, y);
                }
            }

        }

        //switch size dimensions if building is rotated
        if (buildingRotation % 180 != 0) {
            int tempszx = sizex;
            int tempszy = sizey;
            sizex = tempszy;
            sizey = tempszx;

            float tempalignx = alignx;
            float tempaligny = aligny;
            alignx = tempaligny;
            aligny = tempalignx;

        }

		GameObject go = SpawnObject("Structures", type, x, y, buildingRotation);
        go.transform.position += new Vector3(alignx, 0, aligny);

        //unless object is a mapentrance or mapexit, put its coords in its name
        go.name = type + "_" + x + "_" + y;
        if (data.builtOnce)
            go.name = type;

        //rename the area it takes up to its name
        Map.RenameArea(go.name, x, y, sizex, sizey);

		Structure s = go.GetComponent<Structure>();
        if (s == null)
            Debug.LogError(type + " has no structure component");
        s.X = x;
        s.Y = y;
        s.Sizex = sizex;
        s.Sizey = sizey;
        s.DisplayName = data.displayName;
        s.world = this;
        s.Activate();

		return s;
    }

    public void Demolish(int x, int y) {

        GameObject go = Map.GetBuildingAt(x, y);
        if (go == null)
            return;

        Structure s = go.GetComponent<Structure>();

        if (!s.canBeDemolished && modeController.currentMode != Mode.Edit)
            return;

        Destroy(x, y);
        
    }

    public void Destroy(int x, int y) {

        GameObject go = Map.GetBuildingAt(x, y);
        if (go == null)
            return;

        Structure s = go.GetComponent<Structure>();

        int sizex = s.Sizex;
        int sizey = s.Sizey;
        x = s.X;
        y = s.Y;

        s.DeleteDesirability();
        Map.RenameArea(null, x, y, sizex, sizey);
        Destroy(go);

    }

    //check if a tile can be painted
    public bool CanPaintTerrain(string type, int x, int y) {

        //if out of range, don't try it
        if (Map.OutOfBounds(x, y))
            return false;

        //if terrain is already this, don't change
        if (Map.terrain[x, y] == (int)Enums.terrainDict[type])
            return false;

        if (Enums.terrainDict[type] == Terrain.Water && Map.IsBuildingAt(x, y))
            return false;

        return true;
    }

    //paint a tile in the world map
    public void PaintTerrain(string type, int x, int y) {

        if (!CanPaintTerrain(type, x, y))
            return;

		//get old tile if it exists
		GameObject oldTile = tiles[x, y];

		//set new terrain
		Map.terrain[x, y] = (int)Enums.terrainDict[type];

		//make new tile
		GameObject newTile = GenerateTile(x, y, GameObject.Find("Row_" + x));
		tiles[x, y] = newTile;  //put tile into tile grid
		Debug.Log(newTile);

		//replace tile in mesh
		meshCombiner.ReplaceTile(oldTile, newTile);

    }
	
	public void CatchOnFire(int x, int y, int sizex, int sizey) {

		string str = Map.GetBuildingNameAt(x, y);
		Destroy(x, y);
		for (int a = x; a < x + sizex; a++)
			for (int b = y; b < y + sizey; b++) {
				SpawnStructure("RubbleOnFire", a, b, 0);
			}
		
		Notification n = new Notification(NotificationType.Issue, str + " caught on fire!", x, y, timeController);
		notifications.NewNotification(n);

	}

	public void CollapseStructure(int x, int y, int sizex, int sizey) {

		string str = Map.GetBuildingNameAt(x, y);
		Notification n = new Notification(NotificationType.Issue, str + " collapsed!", x, y, timeController);
		notifications.NewNotification(n);

		TurnToRubble(x, y, sizex, sizey);

	}

	public void TurnToRubble(int x, int y, int sizex, int sizey) {

		Destroy(x, y);
		for (int a = x; a < x + sizex; a++)
			for (int b = y; b < y + sizey; b++) {
				if(SpawnStructure("Rubble", a, b, 0) == null)
					Debug.LogError("Couldn't create rubble");
			}

	}

	public bool HasGood(int num, int item, ItemType type) {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("StorageBuilding");

        if (objs.Length == 0 && num != 0)
            return false;
        else if (objs.Length == 0)
            return true;
        
        int sum = 0;

        foreach (GameObject go in objs) {

            StorageBuilding strg = go.GetComponent<StorageBuilding>();

            //if storage building does not store that type of item, continue
            if (strg.typeStored != type)
                continue;

            sum += strg.Inventory[item];

        }

        Debug.Log(sum + " " + num);

        return sum >= num;

    }

    public bool HasGood(ItemOrder io) { return HasGood(io.amount, io.item, io.type); }

	public GameObject SpawnObject(string path, string name) {

		return SpawnObject(path, name, 0, 0);

	}

	public GameObject SpawnObject(string path, string name, Node pos) {

		return SpawnObject(path, name, pos.x, pos.y);

	}

	public GameObject SpawnObject(string path, string name, int x, int y) {

		return SpawnObject(path, name, x, y, 0);

	}

	public GameObject SpawnObject(string path, string name, int x, int y, float r) {

		//modify elevation
		Vector3 pos = new Vector3(x, 0, y);
		pos.y = GetObjectFloat(x, y);

		Vector3 rot = new Vector3(0, r, 0);
		GameObject go = Resources.Load<GameObject>(path + "/" + name);
		GameObject clone = Instantiate(go, pos, Quaternion.Euler(rot));
		clone.name = go.name;

		return clone;

	}

    public void MoveObjViewCamera(Obj o) {

		Transform tr = o.gameObject.transform;
        Vector3 pos = tr.position;
        float rot = tr.rotation.eulerAngles.y;
        objViewCamera.transform.position = pos;
        objViewCamera.transform.rotation = Quaternion.Euler(new Vector3(30, 45 + rot, 0));

    }

	public void MoveMainCameraTo(Obj o) {

		Transform tr = o.gameObject.transform;
		Vector3 pos = tr.position;
		cameraController.MoveCameraTo(pos.x, pos.z);

	}

	public void EnterSquare(Walker w, int x, int y) {

		if (WalkerGrid[x, y] == null)
			WalkerGrid[x, y] = new List<Walker>();

		WalkerGrid[x, y].Add(w);

	}

	public void LeaveSquare(Walker w, int x, int y) {

		if (WalkerGrid[x, y] == null)
			Debug.LogError(w + " trying to leave tile " + x + "_" + y + " when it was not there before");

		WalkerGrid[x, y].Remove(w);

		//if no walkers on this tile, set the list to null
		if(WalkerGrid[x, y].Count == 0)
			WalkerGrid[x, y] = null;

	}

	public List<Walker> GetWalkersOnTile(int x, int y) {

		return WalkerGrid[x, y];

	}

	public void PrintWalkersOnTile(int x, int y) {

		if (WalkerGrid[x, y] == null) {
			Debug.Log("no walkers on tile " + x + "_" + y);
			return;
		}

		string walkers = "Walkers on tile " + x + "_" + y + ": ";
		foreach (Walker w in WalkerGrid[x, y])
			walkers += w + " ";

		Debug.Log(walkers);
	}

	public float GetObjectFloat(int x, int y) {

		return Map.elevation[x, y] * 0.5f;

	}

}
