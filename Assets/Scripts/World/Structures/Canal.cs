using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CanalSave : StructureSave {

    public bool WaterAccess;
    public CanalSave(GameObject go) : base(go) {

        Canal c = go.GetComponent<Canal>();
        WaterAccess = c.WaterAccess;

    }

}

public class Canal : TiledStructure {

    [Header("Canal")]
    public int maxDistance = 10;
    public Material dry;
    public Material wet;
    public List<MeshRenderer> meshRenderers;
    public bool WaterAccess { get; set; }
    public override bool NeighborCondition(int a, int b) {
        Structure str = world.GetBuildingAt(a, b);
        if (str == null)
            return world.Map.terrain[a, b] == (int)Terrain.Water;
        return str.name.Contains("Canal");
    }

    public override void Load(ObjSave o) {

        base.Load(o);

        CanalSave c = (CanalSave)o;
        WaterAccess = c.WaterAccess;

    }

    public new void Update() {

        UpdateTiling();

        TimeDelta += Time.deltaTime;
        if (time == null)
            return;
        if (TimeDelta >= TimeController.DayTime) {

            TimeDelta = 0;
            Days++;
            DoEveryDay();

        }

    }

    public override void DoEveryDay() {

        WaterAccess = StartFindWater();
        //if(!WaterAccess)
        //    Debug.Log(name + " has no access to water");
        foreach (MeshRenderer mr in meshRenderers)
            mr.materials[1].color = WaterAccess ? wet.color : dry.color;

    }

    bool StartFindWater() {

        Node[] visitedSpots = new Node[maxDistance];
        return FindWater(X, Y, visitedSpots, 0);

    }

    bool FindWater(int a, int b, Node[] visitedSpots, int index) {

        if (world.Map.OutOfBounds(a, b))
            return false;

        Node here = new Node(a, b);

        foreach (Node n in visitedSpots)
            if (n != null)
                if (n.Equals(here))
                    return false;

        visitedSpots[index] = here;
        index++;
        if (world.Map.terrain[a, b] == (int)Terrain.Water)
            return true;
        else if (index == visitedSpots.Length)
            return false;
        else if (world.IsBuildingAt(a,b)) {

            if (world.GetBuildingNameAt(a, b).Contains("Canal")) {
                bool up = FindWater(a, b - 1, visitedSpots, index);
                bool down = FindWater(a, b + 1, visitedSpots, index);
                bool left = FindWater(a - 1, b, visitedSpots, index);
                bool right = FindWater(a + 1, b, visitedSpots, index);
                return up || down || left || right;
            }

        }

        return false;
    }

}
