using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cottage : Generator {

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (ProductionComplete && !ActiveRandomWalker)
            ExportProduct();

    }

    public override void ExportProduct() {

        SpawnRandomWalker();
        Producing = false;

    }

    public override void ReceiveItem(ItemOrder io) {

        base.ReceiveItem(io);

    }

}
