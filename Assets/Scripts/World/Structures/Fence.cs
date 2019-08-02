using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : TiledStructure {

    public override int FindNeighbors() {

        int n = 0;

        //X - 1, Y
        //1
        if(world.Map.IsBuildingAt(X - 1, Y))
            if (world.Map.GetBuildingNameAt(X - 1, Y).Contains("Fence_"))
                n += 1;

        //X, Y + 1
        //2
        if (world.Map.IsBuildingAt(X, Y + 1))
            if (world.Map.GetBuildingNameAt(X, Y + 1).Contains("Fence_"))
            n += 2;

        //X + 1, Y
        //4
        if (world.Map.IsBuildingAt(X + 1, Y))
            if (world.Map.GetBuildingNameAt(X + 1, Y).Contains("Fence_"))
            n += 4;

        //X, Y - 1
        //8
        if (world.Map.IsBuildingAt(X, Y - 1))
            if (world.Map.GetBuildingNameAt(X, Y - 1).Contains("Fence_"))
            n += 8;

        return n;

    }

}
