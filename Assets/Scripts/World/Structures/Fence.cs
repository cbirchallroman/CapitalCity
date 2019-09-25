using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : TiledStructure {

    public override int FindNeighbors() {

        int n = 0;

        //X - 1, Y
        //1
        if(world.IsBuildingAt(X - 1, Y))
            if (world.GetBuildingNameAt(X - 1, Y).Contains("Fence_"))
                n += 1;

        //X, Y + 1
        //2
        if (world.IsBuildingAt(X, Y + 1))
            if (world.GetBuildingNameAt(X, Y + 1).Contains("Fence_"))
            n += 2;

        //X + 1, Y
        //4
        if (world.IsBuildingAt(X + 1, Y))
            if (world.GetBuildingNameAt(X + 1, Y).Contains("Fence_"))
            n += 4;

        //X, Y - 1
        //8
        if (world.IsBuildingAt(X, Y - 1))
            if (world.GetBuildingNameAt(X, Y - 1).Contains("Fence_"))
            n += 8;

        return n;

    }

}
