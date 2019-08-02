using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roadblock : Structure {

    public override void Activate() {

        base.Activate();

        world.Map.roads[X, Y] = 1;

    }

    private void Update() {

        if (world.Map.roads[X, Y] != 1)
            world.Map.roads[X, Y] = 1;

    }

    public virtual bool AllowWalkerIn(WalkerData w) {

        return false;

    }

}
