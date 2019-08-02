using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseWindow : ObjWindow {

    public Text residents;

    public override void Open() {

        base.Open();

    }

	public override void UpdateOverviewPage() {

		base.UpdateOverviewPage();

		House h = (House)obj;
		if (h.Residents.Count == 0)
			residents.text = "Vacant Home";
		else if (h.Residents.Count == 1)
			residents.text = h.Residents[0].surname + " Residence";
		else
			residents.text = "Shared Home";

	}

}
