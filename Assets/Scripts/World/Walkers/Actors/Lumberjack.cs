using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Actor {

    public override void OnceAtDestination() {

        world.Destroy(Destination.X, Destination.Y);
        Workplace w = Origin.GetComponent<Workplace>();
        yield += 5 * w.WorkerCount;
        if (yield >= 100)
            ReturnHome();

    }

    public override void DestroySelf() {
        base.DestroySelf();

        Generator g = (Generator)Origin;
        g.AmountStored += yield;

    }

    public override void OnceBackHome() {

        Origin.GetComponent<Generator>().AmountStored = yield;
        base.OnceBackHome();

    }

}
